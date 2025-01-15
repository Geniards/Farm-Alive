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
    [Tooltip("Room ������Ʈ (UI)")]
    public GameObject[] roomObjects;
    [Tooltip("�� ����Ʈ ���� ����")]
    public float updateInterval = 3f;

    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();
    private Coroutine updateCoroutine;

    private void OnEnable()
    {
        StartUpdatingRoomList();
    }

    private void OnDisable()
    {
        StopUpdatingRoomList();
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
        Debug.Log("�� ��� ���� ����!");

        var updatedRoomList = PunManager.Instance.GetRoomList();
        if (updatedRoomList == null)
        {
            Debug.LogWarning("�� ����� ����");
            ClearRoomList();
            return;
        }

        // ������ �� ó��
        var roomNamesToRemove = new List<string>();
        foreach (var cachedRoom in cachedRoomList)
        {
            if (!updatedRoomList.Exists(room => room.Name == cachedRoom.Key))
            {
                roomNamesToRemove.Add(cachedRoom.Key);
            }
        }

        foreach (var roomName in roomNamesToRemove)
        {
            cachedRoomList.Remove(roomName);
            Debug.Log($"�� ����: {roomName}");
        }

        // ���� �߰��ǰų� ������Ʈ�� �� ó��
        foreach (var roomInfo in updatedRoomList)
        {
            if (roomInfo.RemovedFromList)
            {
                if (cachedRoomList.ContainsKey(roomInfo.Name))
                {
                    cachedRoomList.Remove(roomInfo.Name);
                    Debug.Log($"�� ����: {roomInfo.Name}");
                }
            }
            else
            {
                cachedRoomList[roomInfo.Name] = roomInfo;
                Debug.Log($"�� �߰�/����: {roomInfo.Name}");
            }
        }

        // UI ������Ʈ
        DisplayRoomList();
    }

    private void DisplayRoomList()
    {
        ClearRoomList();

        int count = Mathf.Min(roomObjects.Length, cachedRoomList.Count);
        int index = 0;

        foreach (var room in cachedRoomList.Values)
        {
            if (index >= count) break;

            var roomObject = roomObjects[index];
            roomObject.SetActive(true);

            TMP_Text roomText = roomObject.GetComponentInChildren<TMP_Text>();
            if (roomText != null)
            {
                string gameMode = room.CustomProperties.TryGetValue("gameMode", out object gameModeValue) ? gameModeValue.ToString() : "Unknown";
                string stage = room.CustomProperties.TryGetValue("selectedStage", out object stageValue) ? stageValue.ToString() : "Unknown";

                E_GameMode e_GameMode = ((E_GameMode)(int.Parse(gameMode)));
                E_StageMode e_StageMode = ((E_StageMode)(int.Parse(stage)));

                roomText.text = $"{room.Name} ({room.PlayerCount}/{room.MaxPlayers})\nGame Mode : {e_GameMode}\nStage : {e_StageMode}";
            }

            Button roomButton = roomObject.GetComponentInChildren<Button>();
            if (roomButton != null)
            {
                string roomName = room.Name;
                roomButton.onClick.RemoveAllListeners();
                roomButton.onClick.AddListener(() => JoinRoom(roomName));
            }

            index++;
        }
    }

    private void ClearRoomList()
    {
        foreach (var roomObject in roomObjects)
        {
            roomObject.SetActive(false);
        }
    }

    private void JoinRoom(string roomName)
    {
        Debug.Log($"�� ���� �õ�: {roomName}");
        PunManager.Instance.JoinRoom(roomName);
    }
}
