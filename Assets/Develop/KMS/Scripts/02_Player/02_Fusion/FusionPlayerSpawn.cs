using UnityEngine;
using Fusion;

public class FusionPlayerSpawn : SimulationBehaviour, IPlayerJoined, IPlayerLeft
{
    [Tooltip("�κ� ĳ����")]
    [SerializeField] private GameObject _playerPrefab;
    [Tooltip("�κ� ĳ���� ���� ��ġ")]
    [SerializeField] private Transform _playerSpawn;

    private NetworkObject _spawnedPlayer;

    public void PlayerJoined(PlayerRef player)
    {
        if (player != Runner.LocalPlayer) return;

        Debug.Log("Fusion Player ����!");
        _spawnedPlayer = Runner.Spawn(_playerPrefab, _playerSpawn.position, _playerSpawn.rotation, player);

        var playerInfo = _spawnedPlayer.GetComponent<PlayerInfo>();
        if (playerInfo != null && Runner.IsServer)
        {
            playerInfo.InitializePlayerInfo();
            playerInfo.UpdateUI();
        }

    }

    public void PlayerLeft(PlayerRef player)
    {
        if(player != Runner.LocalPlayer) return;
        Debug.Log("Fusion Player ����!");

        if (_spawnedPlayer != null)
        {
            Runner.Despawn(_spawnedPlayer);
        }
    }
}
