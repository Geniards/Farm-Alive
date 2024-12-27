using Photon.Pun;
using UnityEngine;
using System.Threading.Tasks;
using Fusion;
using Unity.VisualScripting;
using UnityEngine.UI;

public class FusionLobbyManager : MonoBehaviour
{
    public NetworkRunner networkRunnerPrefab;
    private NetworkRunner _networkRunner;

    private async void Start()
    {
        if(!networkRunnerPrefab)
        {
            Debug.Log("networkRunnerPrefab�� �������� �ʽ��ϴ�.");
            Debug.Log("networkRunnerPrefab�� ã�� �ִ����Դϴ�.");
            networkRunnerPrefab = FindObjectOfType<NetworkRunner>();
        }

        // Fusion �κ� ����
        await StartFusionLobby();
    }

    private async Task StartFusionLobby()
    {
        //_networkRunner = Instantiate(networkRunnerPrefab);
        networkRunnerPrefab.ProvideInput = true;

        var startResult = await networkRunnerPrefab.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = "FusionLobby"
        });

        if (startResult.Ok)
        {
            Debug.Log("Fusion �κ� ���� ����");
            // Pun �κ� ����.
            // Pun���� �濡�� �������� ���� ���ŵɶ� Fusion�� Pun�� ������ �� �ֵ��� �Ѵ�.
            PhotonNetwork.JoinLobby();
        }
        else
        {
            Debug.LogError($"Fusion �κ� ���� ����: {startResult.ShutdownReason}");
        }
    }
}
