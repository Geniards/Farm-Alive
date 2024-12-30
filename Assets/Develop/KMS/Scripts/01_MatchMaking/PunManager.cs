using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class PunManager : MonoBehaviourPunCallbacks
{
    [Tooltip("�׽�Ʈ�� ���� �� �ѹ� ����.")]
    public int RoomNum = 0;

    // ��ư ������ ���� ����
    public GameObject RoomMakeButtonPrefab;
    private GameObject instantiatedRoomMakeButton;

    public static PunManager Instance { get; private set; }

    private void Awake()
    {
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

        // Resources �������� ������ ���� �ε�
        RoomMakeButtonPrefab = Resources.Load<GameObject>("Room Make Button");

        if (!RoomMakeButtonPrefab)
        {
            Debug.LogError("RoomMakeButtonPrefab�� Resources ������ �����ϴ�!");
        }
    }

    /// <summary>
    /// FirebaseManager�� �ʱ�ȭ �Ϸ�Ǹ� ConnectToPhoton() ȣ��.
    /// VR�� Ư���� Ű���带 ����ϱ⿡ ������� �ִٰ� ������ ��
    /// ������ �������ڸ��� �ٷ� �α����� ����.
    /// </summary>
    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        // ȣ���� ������ FirebaseManager�� �ʱ�ȭ �Ϸ�ǰ� ����
        // ConnectToPhoton() ȣ���ؾ��ϱ⿡ �̺�Ʈ�� ����.
        FirebaseManager.Instance.OnFirebaseInitialized += ConnectToPhoton;
    }

    /// <summary>
    /// Photon������ �����ϴ� �޼���.
    /// 
    /// UserId�� �ߺ����� �濡 ����� ��Ʈ��ũ���� �ι�°�� ���� �÷��̾�� ������ �����Ѵ�.
    /// ������ Id�̱⿡ �̿� ���ؼ� ��Ʈ��ũ���� ������ �Ǵ� �����ε��ϴ�.
    /// (�κ������ �������� �濡�� ������ �ȵȴ�.)
    /// UUID�� �̿��ؼ��� �õ��ߴµ� �ش� �κ��� ����� ������ȣ�� IMEI�� ����Ǵ� ���Ȼ� ������ �ֱ⿡
    /// Firebase�� �͸� �α������� ��ü�ϱ�� ��.
    /// </summary>
    private void ConnectToPhoton()
    {
        string userId = FirebaseManager.Instance.GetUserId();

        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("UserId�� �̿��� �� ����..���� ���� ����!.");
            return;
        }

        // PhotonNetwork���� ������ UserID�� �����ͼ� ������ ����.
        // �׽�Ʈ�ÿ��� userId�� �ҷ��ý� ParrelSync�� ������ �ȵǱ⿡ Random.Range�� ����.
        PhotonNetwork.AuthValues = new AuthenticationValues { UserId = /*userId*/ Random.Range(1000, 10000).ToString() };

        Debug.Log($"ConnectToPhoton {userId}");
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("���濡 ���� ��...");
    }

    /// <summary>
    /// Photon�� ������ڸ��� �κ�� �̵�.
    /// </summary>
    public override void OnConnectedToMaster()
    {
        if (!PhotonNetwork.InLobby)
        {
            Debug.Log("0. Photon Master Server�� ����!");
            PhotonNetwork.JoinLobby();
        }
        else
        {
            Debug.Log("Master Server �κ� ���� ��!");
        }
    }

    /// <summary>
    /// �κ����� �޼���.
    /// </summary>
    public override void OnJoinedLobby()
    {
        Debug.Log("1. PUN �κ� ����!");
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        if (SceneManager.GetActiveScene().name != "03_FusionLobby" && PhotonNetwork.InLobby)
        {
            Debug.Log("�ε� ������ �̵�...");
            SceneManager.LoadScene("LoadingScene");
        }
    }

    /// <summary>
    /// �� ���� �� �� ��� ���� ��ư ����
    /// </summary>
    public void CreateDynamicButtons()
    {
        Debug.Log("��ư ����");
        Canvas canvas = FindObjectOfType<Canvas>();

        if (!canvas)
        {
            Debug.LogError("Canvas�� ���� �������� �ʽ��ϴ�!");
            return;
        }

        // ���� ��ư ����
        if (instantiatedRoomMakeButton)
        {
            Destroy(instantiatedRoomMakeButton);
        }

        // �� ���� ��ư ����
        if (RoomMakeButtonPrefab)
        {
            instantiatedRoomMakeButton = Instantiate(RoomMakeButtonPrefab, canvas.transform);
            Button makeButton = instantiatedRoomMakeButton.GetComponent<Button>();
            if (makeButton != null)
            {
                TMP_Text buttonText = instantiatedRoomMakeButton.GetComponentInChildren<TMP_Text>();
                if (buttonText != null)
                {
                    buttonText.text = "Create Room";
                }

                makeButton.onClick.AddListener(() =>
                {
                    Debug.Log("Room Make ��ư Ŭ����!");
                    CreateAndMoveToPunRoom();
                });
            }
        }
    }

    /// <summary>
    /// Fusion ��Ʈ��ũ ���� �� Pun �� ���� �� �̵�
    /// </summary>
    public void CreateAndMoveToPunRoom()
    {
        // Pun �� ����
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 2,
            IsVisible = true,
            IsOpen = true
        };

        PhotonNetwork.JoinOrCreateRoom($"PunRoom_{RoomNum}", roomOptions, TypedLobby.Default);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log($"�� ���� ����!");
    }

    /// <summary>
    /// �� ���� ����
    /// </summary>
    public override void OnJoinedRoom()
    {
        // Pun �̵�
        Debug.Log($"�� ���� ����: {PhotonNetwork.CurrentRoom.Name}");
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }
        PhotonNetwork.LoadLevel("04_PunWaitingRoom"); // ���� ������ �̵�
    }

    /// <summary>
    /// �� ��� ������Ʈ
    /// </summary>
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo room in roomList)
        {
            Debug.Log($"�� �̸�: {room.Name}, �÷��̾�: {room.PlayerCount}/{room.MaxPlayers}");
            Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        }
    }

    /// <summary>
    /// �� ���� ���� ���� �޼���.
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError("�� ���� ����: " + message);
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Pun ���� �������ϴ�. ���Ӽ������� ������ ������ ��ü!");
    }
}
