using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSpawn : MonoBehaviour
{
    [Tooltip("��ȯ�� �÷��̾� ������")]
    public GameObject playerPrefab;

    [Tooltip("�÷��̾� ��ȯ ��ġ")]
    public Transform spawnPoint;

    private GameObject _player;

    public void OnEnable()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        // ��Ʈ��ũ �󿡼� �÷��̾� ����
        _player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);

        if (_player)
        {
            Debug.Log($"���� �� �÷��̾� ��ȯ �Ϸ�: {FirebaseManager.Instance.GetUserId()}");
        }
        else
        {
            Debug.LogError("���� �� �÷��̾� ��ȯ ����!");
        }
    }
}
