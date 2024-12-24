using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem.XR;

public class FusionPlayerSpawn : SimulationBehaviour, IPlayerJoined, IPlayerLeft
{
    [Tooltip("�κ� ĳ����")]
    [SerializeField] private GameObject _playerPrefab;
    [Tooltip("�κ� ĳ���� ���� ��ġ")]
    [SerializeField] private Transform _playerSpawn;

    public void PlayerJoined(PlayerRef player)
    {
        if (player != Runner.LocalPlayer) return;

        Debug.Log("Fusion Player ����!");
        Runner.Spawn(_playerPrefab, _playerSpawn.position, _playerSpawn.rotation, player);
    }

    public void PlayerLeft(PlayerRef player)
    {
        if(player != Runner.LocalPlayer) return;
        Debug.Log("Fusion Player ����!");
    }
}
