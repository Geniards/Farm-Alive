using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ListRoomSocketInteractor : XRSocketInteractor
{
    [Tooltip("Room Layer �̸�")]
    public GameObject roomListPrefabs;
    public string roomLayerName = "Room";
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        if(args.interactableObject != null)
        {
            Debug.Log($"{args.interactableObject.transform.name}�� ���Ͽ��� �������ϴ�. �� ��� ȣ��!");
            ActivateRoomObject();
        }
    }

    private void ActivateRoomObject()
    {
        int roomLayer = LayerMask.NameToLayer(roomLayerName);
        if (roomLayer == -1)
        {
            Debug.LogWarning($"Layer '{roomLayerName}'�� �������� �ʽ��ϴ�.");
            return;
        }

        ActivateChildrenFind(roomListPrefabs.transform, roomLayer);

    }

    private void ActivateChildrenFind(Transform parent, int layer)
    {
        foreach (Transform child in parent)
        {
            if (child.gameObject.layer == layer)
            {
                child.gameObject.SetActive(true);
                Debug.Log($"Ȱ��ȭ�� ������Ʈ: {child.gameObject.name}");
            }

            // �ڽ� ��ü�� �� �ִٸ� ��������� Ž��
            ActivateChildrenFind(child, layer);
        }
    }
}
