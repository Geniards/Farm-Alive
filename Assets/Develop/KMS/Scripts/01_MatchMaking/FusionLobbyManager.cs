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
        // Fusion �κ� ����
        await StartFusionLobby();
    }

    private async Task StartFusionLobby()
    {
        _networkRunner = Instantiate(networkRunnerPrefab);
        _networkRunner.ProvideInput = true;

        var startResult = await _networkRunner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = "FusionLobby"
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
