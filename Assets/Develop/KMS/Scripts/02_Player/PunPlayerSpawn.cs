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
    }

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

    private void OnEnable()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        // ��Ʈ��ũ �󿡼� �÷��̾� ����
        _player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);

        if (_player)
        {
            Debug.Log($"�÷��̾� ��ȯ �Ϸ�: {FirebaseManager.Instance.GetUserId()}");
        }
        else
        {
            Debug.LogError("�÷��̾� ��ȯ ����!");
        }
    }

    public void ReturnToFusion()
    {
        if (_player != null)
        {
            Debug.Log("Pun �÷��̾� ����...");
            PhotonNetwork.Destroy(_player);
        }

        if (PhotonNetwork.InRoom)
        {
            Debug.Log("Pun �� ������...");
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            Debug.LogWarning($"���� ���¿����� LeaveRoom�� ȣ���� �� �����ϴ�: {PhotonNetwork.NetworkClientState}");
        }

        // VoiceConnection �ʱ�ȭ
        var voiceConnection = FindObjectOfType<Photon.Voice.Unity.VoiceConnection>();
        if (voiceConnection != null)
        {
            Debug.Log("VoiceConnection ���� �ʱ�ȭ ��...");
            voiceConnection.Client.Disconnect();
            Destroy(voiceConnection.gameObject); // VoiceConnection ��ü ����
        }

        // 3. �ε� �� ȣ��
        Debug.Log("�ε� ������ �̵�...");
        SceneManager.LoadScene("LoadingScene");
    }
}
