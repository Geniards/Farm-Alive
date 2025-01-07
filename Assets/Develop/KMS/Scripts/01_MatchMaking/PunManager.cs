using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class PunManager : MonoBehaviourPunCallbacks
{
    [Tooltip("�׽�Ʈ�� ���� �� �ѹ� ����.")]
    public int RoomNum = 0;
    public int maxPlayer = 5;
    [Tooltip("�������� ID")]
    public int selectedStage = 511;
    private List<RoomInfo> cachedRoomList = new List<RoomInfo>();

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
        Debug.Log("Firebase �̺�Ʈ ���");
        FirebaseManager.Instance.OnFirebaseInitialized += ConnectToPhoton;
        //FirebaseManager.Instance.NotifyInitializationComplete();
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
        if (SceneManager.GetActiveScene().name != "03_Lobby" && PhotonNetwork.InLobby)
        {
            Debug.Log("�ε� ������ �̵�...");
            SceneLoader.LoadSceneWithLoading("03_FusionLobby");
        }
    }

    /// <summary>
    /// Fusion ��Ʈ��ũ ���� �� Pun �� ���� �� �̵�
    /// </summary>
    public void CreateAndMoveToPunRoom()
    {
        Debug.Log("����� ����!");

        //// 5�� ī��Ʈ�ٿ� �� �� ���� ����
        //StartCoroutine(PunRoomCountdown(5f));
    }



    /// <summary>
    /// Coroutine���� 5�� ���� ī��Ʈ�ٿ� �޽����� �����ϰ� �� ���� �� �̵�
    /// </summary>
    private IEnumerator PunRoomCountdown(float countdown)
    {
        float remainingTime = countdown;

        while (remainingTime > 0)
        {
            // �޽��� ����
            MessageDisplayManager.Instance.ShowMessage($"After {(int)remainingTime} seconds, you enter the room.", 1f, 3f);
            Debug.Log($"After {(int)remainingTime} seconds, you enter the room.");
            yield return new WaitForSeconds(1f);
            remainingTime--;
        }

        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = maxPlayer,
            IsVisible = true,
            IsOpen = true
        };

        Debug.Log("�� ���� �õ� ��...");
        PhotonNetwork.JoinOrCreateRoom($"PunRoom_{Random.RandomRange(100,1000)}", roomOptions, TypedLobby.Default);
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
        // �� ��� ������Ʈ
        cachedRoomList = roomList;
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

    /// <summary>
    /// �� ��� ��������.
    /// </summary>
    /// <returns></returns>
    public List<RoomInfo> GetRoomList()
    {
        return cachedRoomList;
    }

    /// <summary>
    /// ������ �� ����.
    /// </summary>
    /// <param name="roomName"></param>
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }
}
