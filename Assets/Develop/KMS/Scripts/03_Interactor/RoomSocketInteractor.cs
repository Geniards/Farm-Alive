using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RoomSocketInteractor : XRSocketInteractor
{
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        if (args.interactableObject != null)
        {
            RoomInfoHolder roomInfoHolder = args.interactableObject.transform.GetComponent<RoomInfoHolder>();
            if (roomInfoHolder)
            {
                string roomName = roomInfoHolder.RoomName;
                Debug.Log($"{args.interactableObject.transform.name}�� ���Ͽ��� �������ϴ�. �� �̸�: {roomName}");
                EnterSelectedRoom(roomName);
            }
            else
            {
                Debug.LogWarning("RoomInfoHolder�� �ش� ������Ʈ�� �����ϴ�!");
            }
        }
    }

    private void EnterSelectedRoom(string roomName)
    {
        if (!string.IsNullOrEmpty(roomName))
        {
            Debug.Log($"'{roomName}' �濡 ���� �õ�...");
            PunManager.Instance.JoinRoom(roomName);
        }
        else
        {
            Debug.LogWarning("������ �� �̸��� ��� �ֽ��ϴ�!");
            Debug.Log(roomName);
        }
    }
}
