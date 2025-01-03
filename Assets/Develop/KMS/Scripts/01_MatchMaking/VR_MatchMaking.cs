using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class VR_MatchMaking : MonoBehaviourPunCallbacks
{
    public GameObject xrOriginPrefab;       // XR Origin ������
    public GameObject characterPrefab;      // ĳ���� ������
    public GameObject newcharacterPrefab;   // ĳ���� ������
    public Vector3 PlayerSpawn;

    [Tooltip("�׽�Ʈ�� ���� �� �ѹ� ����.")]
    public int RoomNum = 0;

    /// <summary>
    /// FirebaseManager�� �ʱ�ȭ �Ϸ�Ǹ� ConnectToPhoton() ȣ��.
    /// VR�� Ư���� Ű���带 ����ϱ⿡ ������� �ִٰ� ������ ��
    /// ������ �������ڸ��� �ٷ� �α����� ����.
    /// </summary>
    private void Start()
    {
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
        Debug.Log("0. Photon Master Server�� ����!");
        PhotonNetwork.JoinLobby();
    }

    /// <summary>
    /// �κ����� �޼���.
    /// </summary>
    public override void OnJoinedLobby()
    {
        Debug.Log("1. �κ� ����!");
        LoadLobbyScene();
    }

    /// <summary>
    /// �κ� ���忡 ������ Level��ȯ �޼���.
    /// </summary>
    private void LoadLobbyScene()
    {
        // TODO: ��� test ��Ȳ���� ���� ���� ���� �����ϵ��� �Ѵ�.
        //PhotonNetwork.LoadLevel("LobbyScene");
        Debug.Log("�κ� �� �ε� �� �� ���� �õ�.");

        // �ӽ÷� �׽�Ʈ ������ �����ϵ��� ��.
        // TODO: ���� ����� �κ� ���� �ʿ�!.
        RoomOptions options = new RoomOptions() { IsVisible = false };
        PhotonNetwork.JoinOrCreateRoom($"TestRoom {RoomNum}", options, TypedLobby.Default);
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

    /// <summary>
    /// �� ���� ���� �޼���.
    /// </summary>
    public override void OnJoinedRoom()
    {
        // TODO: �÷��̾� ������ ����ϴ� ��.
        Debug.Log("�濡 �����߽��ϴ�. XR Origin �� ĳ���� ���� ��...");
        GameObject Player = PhotonNetwork.Instantiate(newcharacterPrefab.name, PlayerSpawn, Quaternion.identity);

        if (Player)
        {
            Debug.Log("Player �����Ϸ�!");
        }
        else
        {
            Debug.LogWarning($"Player ��������. newcharacterPrefab�� �����ϴ��� Ȯ��!");
        }
    }
}
