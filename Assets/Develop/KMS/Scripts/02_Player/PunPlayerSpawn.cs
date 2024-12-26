using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunPlayerSpawn : MonoBehaviour
{
    [Tooltip("��ȯ�� �÷��̾� ������")]
    public GameObject playerPrefab;

    [Tooltip("�÷��̾� ��ȯ ��ġ")]
    public Transform spawnPoint;

    private void OnEnable()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        // ��Ʈ��ũ �󿡼� �÷��̾� ����
        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);

        if (player)
        {
            Debug.Log($"�÷��̾� ��ȯ �Ϸ�: {FirebaseManager.Instance.GetUserId()}");
        }
        else
        {
            Debug.LogError("�÷��̾� ��ȯ ����!");
        }
    }
}
