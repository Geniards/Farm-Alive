using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSpawn : MonoBehaviour
{
    [Tooltip("��ȯ�� �÷��̾� ������")]
    public GameObject playerPrefab;

    [Tooltip("�÷��̾� ��ȯ ��ġ")]
    public Transform spawnPoint;

    private GameObject _player;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            ReturnToFusion();
        }
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

    public void ReturnToFusion()
    {
        ClearSingletonManagers();
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

    private void ClearSingletonManagers()
    {
        // SectionManager ����
        if (SectionManager.Instance != null)
        {
            Debug.Log("SectionManager ����");
            Destroy(SectionManager.Instance.gameObject);
        }

        // LightingManager ����
        if (LightingManager.Instance != null)
        {
            Debug.Log("LightingManager ����");
            Destroy(LightingManager.Instance.gameObject);
            LightingManager.Instance = null;
        }
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
