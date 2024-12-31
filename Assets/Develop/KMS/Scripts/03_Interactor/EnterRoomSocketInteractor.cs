using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class EnterRoomSocketInteractor : XRSocketInteractor
{
    private bool isSelected;

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        if(args.interactableObject != null && !isSelected)
        {
            isSelected = true;
            Debug.Log($"{args.interactableObject.transform.name}�� ���Ͽ��� �������ϴ�. ������ �����մϴ�!");
            PunManager.Instance.CreateAndMoveToPunRoom();
        }
    }
}
