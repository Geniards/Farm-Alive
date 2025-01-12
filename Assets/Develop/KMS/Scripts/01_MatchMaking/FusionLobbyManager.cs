using Photon.Pun;
using UnityEngine;
using System.Threading.Tasks;
using Fusion;
using Unity.VisualScripting;
using UnityEngine.UI;

public class FusionLobbyManager : MonoBehaviour
{
    public NetworkRunner networkRunnerPrefab;
    public string lobbyName = "FusionLobby";

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
        networkRunnerPrefab.ProvideInput = true;

        var startResult = await networkRunnerPrefab.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = lobbyName
        });

        if (startResult.Ok)
        {
            Debug.Log("Fusion �κ� ���� ����");
        }
        else
        {
            Debug.LogError($"Fusion �κ� ���� ����: {startResult.ShutdownReason}");
        }
    }
}
