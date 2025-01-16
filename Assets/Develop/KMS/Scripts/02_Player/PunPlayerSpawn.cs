using Fusion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PunPlayerSpawn : MonoBehaviour
{
    [Tooltip("��ȯ�� �÷��̾� ������")]
    public GameObject playerPrefab;

    [Tooltip("�÷��̾� ��ȯ ��ġ")]
    public Transform spawnPoint;

    private GameObject _player;

    private void Start()
    {
        RemoveNetworkRunner();
        SpawnPlayer();
    }
#if UNITY_EDITOR
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Z))
    //    {
    //        ReturnToFusion();
    //    }
    //}
#endif

    private void RemoveNetworkRunner()
    {
        var runner = FindObjectOfType<NetworkRunner>();
        if (runner != null)
        {
            Debug.Log("�� �̵� �� ���� �ִ� Fusion���� ShutDown.");
            // Fusion ���� ShutDown�� ��Ŵ.
            // NetworkRunner�� ���� ��Ű�� �ʾҴµ�
            // �ѹ� Shut Down�� �� ����� ������ �Ұ����� �� ����.
            // (�ٽ� false�� true�� ����.)
            runner.Shutdown();
            Destroy(runner.gameObject);
        }
    }

    private void SpawnPlayer()
    {
        // ��Ʈ��ũ �󿡼� �÷��̾� ����
        _player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);

        if (_player)
        {
            Debug.Log($"�÷��̾� ��ȯ �Ϸ�: {FirebaseManager.Instance.GetUserId()}");
            if (SceneManager.GetActiveScene().name == "04_PunWaitingRoom")
            {
                PhotonNetwork.AutomaticallySyncScene = true;
                Debug.Log("04_���� �������� �ʵ���ȭ ����.");
            }
        }
        else
        {
            Debug.LogError("�÷��̾� ��ȯ ����!");
        }
    }

    public void ReturnToFusion()
    {
        ClearPunObject();

        if (PhotonNetwork.InRoom)
        {
            Debug.Log("Pun �� ������...");
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            Debug.LogWarning($"���� ���¿����� LeaveRoom�� ȣ���� �� �����ϴ�: {PhotonNetwork.NetworkClientState}");
        }


        Debug.Log("���� ��ü ��...");
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        Debug.Log($"PhotonNetwork.InRoom = {PhotonNetwork.InRoom}");
        Debug.Log($"PhotonNetwork.NetworkClientState = {PhotonNetwork.NetworkClientState}");
    }

    public void ClearPunObject()
    {
        var voiceConnection = FindObjectOfType<Photon.Voice.Unity.VoiceConnection>();
        if (voiceConnection != null)
        {
            Debug.Log("VoiceConnection ���� �ʱ�ȭ ��...");
            if (voiceConnection.Client.InRoom)
            {
                Debug.Log("VoiceConnection �濡�� ������...");
                voiceConnection.Client.OpLeaveRoom(false);
            }
            voiceConnection.Client.Disconnect(); // ������ ���� ����
        }

        if (_player != null)
        {
            var photonView = _player.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                Debug.Log("Pun �÷��̾� ����...");
                PhotonNetwork.Destroy(_player); // ��Ʈ��ũ �󿡼� ĳ���� ����
            }
            else
            {
                Debug.LogWarning("�� ��ü�� �ڽ��� ���� �ƴϹǷ� ������ �� �����ϴ�.");
            }
        }
    }
}
