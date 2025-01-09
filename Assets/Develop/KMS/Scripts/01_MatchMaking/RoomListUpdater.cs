using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
        // �� ����Ʈ �ʱ�ȭ
        ClearRoomList();

        // Photon���� �ֽ� �� ����Ʈ ��������
        cachedRoomList = PunManager.Instance.GetRoomList();

        // �ִ� roomObjects.Length ������ŭ �� ����Ʈ ǥ��
        int count = Mathf.Min(roomObjects.Length, cachedRoomList.Count);


        for (int i = 0; i < count; i++)
        {
            roomObjects[i].SetActive(true);

            // �� ���� ����
            ObjectUI roomText = roomObjects[i].GetComponent<ObjectUI>();
            if (roomText)
            {
                roomText.nameTextPrefab.text = $"{cachedRoomList[i].Name} ({cachedRoomList[i].PlayerCount}/{cachedRoomList[i].MaxPlayers})";
            }
            else
            {
                Debug.Log("����");
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
}
