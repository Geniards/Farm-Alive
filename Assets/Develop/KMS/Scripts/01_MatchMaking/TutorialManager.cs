using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    [Tooltip("��ȯ�� ĳ���� ������")]
    public GameObject playerPrefab;
    [Tooltip("�÷��̾� ��ȯ ��ġ")]
    public Transform spawnPoint;

    private GameObject _spawnedPlayer;

    private void Start()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        if (!playerPrefab || !spawnPoint)
        {
            Debug.LogError("PlayerPrefab �Ǵ� SpawnPoint�� �������� �ʾҽ��ϴ�.");
            return;
        }

        _spawnedPlayer = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);

        if (_spawnedPlayer)
        {
            Debug.Log("�÷��̾ ���������� �����Ǿ����ϴ�!");
        }
        else
        {
            Debug.LogError("�÷��̾� ���� ����!");
        }
    }

    // ��ư Ŭ�� �� Fusion �κ�� �̵�
    public void OnTutorialComplete()
    {
        Debug.Log("Loading Scene �̵�...");
        SceneLoader.LoadSceneWithLoading("03_FusionLobby");
    }
}
