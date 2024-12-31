using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ListRoomSocketInteractor : XRSocketInteractor
{
    [Tooltip("����+ť�� ������")]
    public GameObject socketCubePrefab;

    [Tooltip("���� ��ġ")]
    public Transform startPosition;

    [Tooltip("�� ��� ����")]
    public Vector3 offset = new Vector3(1.5f, -10f, 0);

    [Tooltip("�� �ٿ� ǥ���� ����")]
    public int maxColumns = 5;

    // �� ��� ����Ʈ
    private List<GameObject> activeRoomObjects = new List<GameObject>();

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        if(args.interactableObject != null)
        {
            Debug.Log($"{args.interactableObject.transform.name}�� ���Ͽ��� �������ϴ�. �� ��� ȣ��!");
            UpdateRoomList();
        }
    }

    private void UpdateRoomList()
    {
        if (!socketCubePrefab || !startPosition)
        {
            Debug.LogError("SocketCubePrefab �Ǵ� StartPosition�� �������� �ʾҽ��ϴ�!");
            return;
        }

        // ���� �� ��� ����
        foreach (GameObject roomObject in activeRoomObjects)
        {
            Destroy(roomObject);
        }
        activeRoomObjects.Clear();

        // Photon �� ��� ��������
        List<RoomInfo> roomInfos = PunManager.Instance.GetRoomList();
        if (roomInfos.Count == 0)
        {
            Debug.Log("���� ǥ���� �� ����� �����ϴ�.");
            return;
        }

        // ���ο� �� ��� ����
        for (int i = 0; i < roomInfos.Count; i++)
        {
            int row = i / maxColumns;
            int column = i % maxColumns;

            Vector3 spawnPosition = startPosition.position +
                                    new Vector3(column * offset.x, row * offset.y, 0);

            GameObject socketCube = Instantiate(socketCubePrefab, spawnPosition, Quaternion.identity, transform);
            
            // ť���� RoomInfoHolder�� �� �̸� ����
            RoomInfoHolder roomInfoHolder = socketCube.GetComponentInChildren<RoomInfoHolder>();
            roomInfoHolder.RoomName = roomInfos[i].Name;

            // ť���� �ؽ�Ʈ ������Ʈ
            TMP_Text roomText = socketCube.GetComponentInChildren<TMP_Text>();
            if (roomText)
            {
                RoomInfo room = roomInfos[i];
                roomText.text = $"Room Name: {room.Name}\nPlayer: {room.PlayerCount}/{room.MaxPlayers}";
            }
            Debug.Log($"���� �� �� ���� �Ϸ�: {roomInfos[i].Name}, ��ġ: {spawnPosition}");

            activeRoomObjects.Add(socketCube);
        }
    }
}
