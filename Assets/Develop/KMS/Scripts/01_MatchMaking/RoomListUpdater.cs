using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomListUpdater : MonoBehaviour
{
    [Header("Room List Settings")]
    [Tooltip("Room ������Ʈ")]
    public GameObject[] roomObjects;
    [Tooltip("�� ����Ʈ ���� ����")]
    public float updateInterval = 3f;

    private List<RoomInfo> cachedRoomList = new List<RoomInfo>();
    private Coroutine updateCoroutine;

    private void OnEnable()
    {
        // �� ����Ʈ ���� ����
        StartUpdatingRoomList();
    }
    private void OnDisable()
    {
        // ���� ����
        StopUpdatingRoomList();

        // �� ����Ʈ �ʱ�ȭ
        ClearRoomList();
    }

    private void StartUpdatingRoomList()
    {
        if (updateCoroutine == null)
        {
            updateCoroutine = StartCoroutine(RoomListUpdateCoroutine());
        }
    }

    private void StopUpdatingRoomList()
    {
        if (updateCoroutine != null)
        {
            StopCoroutine(updateCoroutine);
            updateCoroutine = null;
        }
    }

    private IEnumerator RoomListUpdateCoroutine()
    {
        while (true)
        {
            UpdateRoomListUI();
            yield return new WaitForSeconds(updateInterval);
        }
    }

    private void UpdateRoomListUI()
    {
        ClearRoomList();
        Debug.Log("�� ��� ����!");

        cachedRoomList = PunManager.Instance.GetRoomList();

        if (cachedRoomList == null || cachedRoomList.Count == 0)
        {
            Debug.LogWarning("�� ����� ����");
            return;
        }

        int count = Mathf.Min(roomObjects.Length, cachedRoomList.Count);

        for (int i = 0; i < count; i++)
        {
            if (i >= cachedRoomList.Count)
            {
                Debug.LogWarning($"Index {i} out of range for cachedRoomList.");
                break;
            }

            roomObjects[i].SetActive(true);

            TMP_Text roomText = roomObjects[i].GetComponentInChildren<TMP_Text>();
            if (roomText)
            {
                // CustomProperties���� ���� ���� �������� ��������
                string gameMode = cachedRoomList[i].CustomProperties.TryGetValue("gameMode", out object gameModeValue) ? gameModeValue.ToString() : "Unknown";
                string stage = cachedRoomList[i].CustomProperties.TryGetValue("selectedStage", out object stageValue) ? stageValue.ToString() : "Unknown";

                E_GameMode e_GameMode = ((E_GameMode)(int.Parse(gameMode)));
                E_StageMode e_StageMode = ((E_StageMode)(int.Parse(stage)));

                roomText.text = $"{cachedRoomList[i].Name} ({cachedRoomList[i].PlayerCount}/{cachedRoomList[i].MaxPlayers})\nGame Mode : {e_GameMode.ToString()}\nStage : {e_StageMode.ToString()}";
            }

            Button roomButton = roomObjects[i].GetComponentInChildren<Button>();
            if (roomButton != null)
            {
                string roomName = cachedRoomList[i].Name;
                roomButton.onClick.RemoveAllListeners();
                roomButton.onClick.AddListener(() => JoinRoom(roomName));
            }
        }
    }

    private void ClearRoomList()
    {
        foreach(var roomObject in roomObjects)
        {
            roomObject.SetActive(false);
        }
    }

    private void JoinRoom(string roomName)
    {
        Debug.Log($"Trying to join room: {roomName}");
        PunManager.Instance.JoinRoom(roomName);
    }
}
