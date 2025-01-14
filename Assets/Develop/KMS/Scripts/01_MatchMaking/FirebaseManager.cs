using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System.Threading.Tasks;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance { get; private set; } // �̱���

    private FirebaseApp app;
    public static FirebaseApp App { get { return Instance.app; } }

    private Firebase.Auth.FirebaseAuth auth;
    public static Firebase.Auth.FirebaseAuth Auth { get { return Instance.auth; } }

    private FirebaseDatabase dataBase;
    public static FirebaseDatabase DataBase { get { return Instance.dataBase; } }

    private string userId; // Firebase UID

    public event Action OnFirebaseInitialized; // Firebase �ʱ�ȭ �Ϸ� �̺�Ʈ

    private string _highStage = "";

    private string _nickName;
    public class StageData
    {
        public int stars;
        public float playTime;

        public StageData(int stars, float playTime)
        {
            this.stars = stars;
            this.playTime = playTime;
        }
    }
    private Dictionary<int, StageData> cachedStageData = new Dictionary<int, StageData>();


    /// <summary>
    /// �÷��̾��� ����� UID�� ȣ��.
    /// </summary>
    /// <returns></returns>
    public string GetUserId()
    {
        if (string.IsNullOrEmpty(userId))
        {
            userId = PlayerPrefs.GetString("firebase_uid", string.Empty);
        }
        return userId;
    }

    private void Awake()
    {
        // �̱��� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        InitializeFirebase();
    }

    /// <summary>
    /// ���̾�̽� �ʱ�ȭ �޼���.
    /// </summary>
    private void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                app = FirebaseApp.DefaultInstance;
                auth = FirebaseAuth.DefaultInstance;
                dataBase = FirebaseDatabase.DefaultInstance;

                Debug.Log("Firebase �ʱ�ȭ �Ϸ�!");
                CheckAndInitializeUserData();
            }
            else
            {
                Debug.LogError("Firebase ������ �ذ� ����!");
                app = null;
                auth = null;
                dataBase = null;
            }
        });
    }

    /// <summary>
    /// �͸� �α��� ���� �޼���.
    /// </summary>
    private void AnonymouslyLogin()
    {
        ClearLocalUid();

        auth.SignOut();
        Debug.Log("���� ���� �α׾ƿ� �Ϸ�. ���ο� �͸� �α��� �õ�...");

        auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("�͸� �α��� �۾��� ��ҵǾ����ϴ�.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("�͸� �α��� �۾� �� ���� �߻�: " + task.Exception);
                return;
            }

            Firebase.Auth.AuthResult result = task.Result;
            if (result?.User == null)
            {
                Debug.LogError("�α��� ������� User ������ ������ �� �����ϴ�.");
                return;
            }

            userId = result.User.UserId; // Firebase UID ����
            SaveUidLocally(userId); // UID�� ���� ����
            Debug.LogFormat($"�͸� �α��� ����! UID: {userId}");

            // uid �� ������ ����.
            SaveUserData();
            // ������ �α��� �ð� ����.
            UpdateLastLogin();
        });
    }

    /// <summary>
    /// ���� UID �˻� �� �ش� �����Ͱ� ���� �� �͸� �α������� ����.
    /// </summary>
    private void CheckAndInitializeUserData()
    {
        string localUid = LoadUidFromLocal();

        if (!string.IsNullOrEmpty(localUid))
        {
            Debug.Log($"���� ����� UID �߰�: {localUid}");
            VerifyUserExistsInFirebase(localUid);
        }
        else
        {
            Debug.Log("���� UID�� �����ϴ�. ���� �α��� ����...");
            AnonymouslyLogin();
        }
    }

    /// <summary>
    /// ���̾�̽� �����Ͱ� ����� �ش� ���̵�� �α���
    /// ������ �͸� �α������� �����ϴ� �޼���.
    /// </summary>
    /// <param name="uid"></param>
    private void VerifyUserExistsInFirebase(string uid)
    {
        DatabaseReference userRef = dataBase.GetReference($"users/{uid}");

        userRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                // Permission Denied ó��
                Debug.LogError("Firebase ���� ����: " + task.Exception);
                Debug.Log("Permission Denied �Ǵ� �ٸ� ���� �߻�. ���� UID �ʱ�ȭ �� �� �α��� ����...");
                AnonymouslyLogin();
                return;
            }

            if (task.IsCompleted && task.Result.Exists)
            {
                Debug.Log($"Firebase���� UID {uid} �߰�!");
                userId = uid;
                UpdateLastLogin();
                LoadNickName();
                LoadHighStage();
                NotifyInitializationComplete();
            }
            else
            {
                Debug.Log("Firebase�� �ش� UID ����. ���� UID ���� �� �� �α��� ����...");
                AnonymouslyLogin();
            }
        });
    }

    /// <summary>
    /// ������ ���̾� ���̽��� ���� ������ ���� �޼���.
    /// </summary>
    private void SaveUserData()
    {
        DatabaseReference userRef = dataBase.GetReference($"users/{userId}");

        Dictionary<string, object> userData = new Dictionary<string, object>()
        {
            { "uid", userId },
            { "nickname", "" },
            { "createdAt", DateTime.Now.ToString("o") },
            { "lastLogin", DateTime.Now.ToString("o") },
            { "settings", new Dictionary<string, object>()
                {
                    { "sound", 0 }
                }
            },
            { "highStage", _highStage },
            { "achievements", new List<string>() { "first_login" } }
        };

        userRef.SetValueAsync(userData).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                Debug.Log("����� ������ ���� �Ϸ�!");
                UpdateLastLogin();
                NotifyInitializationComplete();
                return;
            }
            else
            {
                Debug.LogError("����� ������ ���� ����: " + task.Exception);
            }
        });
    }

    /// <summary>
    /// �������� ��� ���̾� ���̽��� ���� �޼���. 
    /// </summary>
    public void SaveStageResult(int stageID, float playedTime, int starCount)
    {
        if (string.IsNullOrEmpty(userId))
            return;

        int stageIDX = CSVManager.Instance.Stages[stageID].idx + 1;
        string highstageID = "Stage" + stageIDX;
        
        DatabaseReference stageRef = dataBase.GetReference($"users/{userId}/stageResults/{stageID}");

        Dictionary<string, object> resultData = new Dictionary<string, object>()
        {
            { "stageID", stageID },
            { "playTime", playedTime },
            { "stars" , starCount },
            {"timeStamp" , DateTime.Now.ToString("o") },
        };

        stageRef.SetValueAsync(resultData).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                Debug.Log("�������� ������ ���� �Ϸ�!");
                return;
            }
            else
            {
                Debug.LogError("�������� ������ ���� ����: " + task.Exception);
            }
        });

        dataBase.GetReference($"users/{userId}/highStage").SetValueAsync(highstageID).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                Debug.Log("�ְ��� ���� �Ϸ�!");
                return;
            }
            else
            {
                Debug.LogError("�ְ��� ���� ����: " + task.Exception);
            }
        });
    }

    /// <summary>
    /// Firebase���� HighStage�� ������ ĳ���մϴ�.
    /// </summary>
    public void LoadHighStage()
    {
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("���� ID�� �����ϴ�. HighStage�� ������ �� �����ϴ�.");
            return;
        }

        DatabaseReference highStageRef = dataBase.GetReference($"users/{userId}/highStage");

        highStageRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && task.Result != null && task.Result.Value != null)
            {
                _highStage = task.Result.Value.ToString();
                Debug.Log($"HighStage �ҷ����� ����: {_highStage}");

                // ĳ�� ����
                CacheStageData();
            }
            else
            {
                Debug.LogError("HighStage �ҷ����� ����: " + task.Exception);
                _highStage = "Stage1";
            }
        });
    }

    /// <summary>
    /// ���� ����� HighStage�� ��ȯ�մϴ�.
    /// </summary>
    public string GetHighStage()
    {
        if (string.IsNullOrEmpty(_highStage))
        {
            Debug.LogWarning("HighStage�� ���� �ε���� �ʾҽ��ϴ�.");
        }

        return _highStage;
    }

    /// <summary>
    /// stageID�� �̿��� stars, playTime�� ĳ���ϱ�.
    /// </summary>
    private void CacheStageData()
    {
        if (!Enum.TryParse(_highStage, out E_StageMode highStageEnum))
        {
            Debug.LogError("HighStage�� �Ľ��� �� �����ϴ�.");
            return;
        }

        // �񿬼����̱⿡ Enum.GetValues�� ����Ͽ� ��ȸ
        foreach (E_StageMode stage in Enum.GetValues(typeof(E_StageMode)))
        {
            // ��ȿ�� ���������� ó��
            if (stage == E_StageMode.None || stage == E_StageMode.SIZE_MAX || stage > highStageEnum)
            {
                continue;
            }

            int stageID = (int)stage;
            DatabaseReference stageRef = dataBase.GetReference($"users/{userId}/stageResults/{stageID}");

            stageRef.GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted && task.Result != null && task.Result.HasChildren)
                {
                    int stars = 0;
                    float playTime = 0f;

                    if (task.Result.Child("stars").Value != null)
                    {
                        stars = int.Parse(task.Result.Child("stars").Value.ToString());
                    }

                    if (task.Result.Child("playTime").Value != null)
                    {
                        playTime = float.Parse(task.Result.Child("playTime").Value.ToString());
                    }

                    cachedStageData[stageID] = new StageData(stars, playTime);
                    Debug.Log($"Stage {stage} ������ ĳ�� �Ϸ�: Stars {stars}, PlayTime {playTime}");
                }
                else
                {
                    Debug.LogWarning($"Stage {stage} �����Ͱ� �����ϴ�.");
                    cachedStageData[stageID] = new StageData(0, 0f);
                }
            });
        }
    }

    public StageData GetCachedStageData(int stageID)
    {
        if (cachedStageData.ContainsKey(stageID))
        {
            return cachedStageData[stageID];
        }
        Debug.LogWarning($"Stage {stageID} �����Ͱ� ĳ�̵��� �ʾҽ��ϴ�.");
        return null;
    }

    public void SaveNickName(string playerName)
    {
        if (string.IsNullOrEmpty(userId)) return;

        DatabaseReference nickNameRef = dataBase.GetReference($"users/{userId}/nickname");

        nickNameRef.SetValueAsync(playerName).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                Debug.Log($"�г��� '{playerName}' ���� �Ϸ�!");
                // �г��� ĳ��.
                _nickName = playerName;
            }
            else
            {
                Debug.LogError($"�г��� ���� ����: {task.Exception}");
            }
        });

    }

    /// <summary>
    /// Firebase���� �г����� ������ ĳ���մϴ�.
    /// </summary>
    public void LoadNickName()
    {
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("���� ID�� �����ϴ�. �г����� �ҷ��� �� �����ϴ�.");
            return;
        }

        DatabaseReference nickNameRef = dataBase.GetReference($"users/{userId}/nickname");

        nickNameRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && task.Result != null && task.Result.Value != null)
            {
                _nickName = task.Result.Value.ToString();
                Debug.Log($"�г��� �ҷ����� ����: {_nickName}");
            }
            else
            {
                Debug.LogError("�г��� �ҷ����� ����: " + task.Exception);
                _nickName = null;
            }
        });
    }

    /// <summary>
    /// ���� ����� �г����� ��ȯ�մϴ�.
    /// </summary>
    public string GetNickName()
    {
        if (string.IsNullOrEmpty(_nickName))
        {
            Debug.LogWarning("�г����� ���� �ε���� �ʾҽ��ϴ�.");
        }

        return _nickName;
    }

    /// <summary>
    /// �ֱ� �α����� ����� �����ϴ� �޼���.
    /// </summary>
    public void UpdateLastLogin()
    {
        Debug.Log("ĳ���� ������ �α��� ����!");
        Debug.Log("������ �����͸� ��� ���ž��� - (�ڵ� ���Ƶ�!)");

        DatabaseReference userRef = dataBase.GetReference($"users/{userId}/lastLogin");

        userRef.SetValueAsync(DateTime.Now.ToString("o")).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                Debug.Log("������ �α��� �ð� ������Ʈ ����!");
            }
            else
            {
                Debug.LogError("������ �α��� �ð� ������Ʈ ����: " + task.Exception);
            }
        });
    }

    /// <summary>
    /// �α��� �Ϸ� ������ �˸��� �޼���.
    /// </summary>
    public void NotifyInitializationComplete()
    {
        OnFirebaseInitialized?.Invoke();
    }

    /// <summary>
    /// ���ÿ� ������ UID�� ����.
    /// </summary>
    /// <param name="uid"></param>
    private void SaveUidLocally(string uid)
    {
        PlayerPrefs.SetString("firebase_uid", uid);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// ���ÿ� ����� UID�� �ҷ����� �޼���.
    /// ������ Empty�� ��ȯ.
    /// </summary>
    /// <returns></returns>
    private string LoadUidFromLocal()
    {
        return PlayerPrefs.GetString("firebase_uid", string.Empty);
    }

    /// <summary>
    /// ���ÿ� ����� UID�� �����ϴ� �޼���.
    /// </summary>
    private void ClearLocalUid()
    {
        PlayerPrefs.DeleteKey("firebase_uid");
        PlayerPrefs.Save();
        userId = string.Empty;
    }
}
