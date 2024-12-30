using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class EnterRoomSocketInteractor : XRSocketInteractor
{
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        if(args.interactableObject != null)
        {
            Debug.Log($"{args.interactableObject.transform.name}�� ���Ͽ��� �������ϴ�. ������ �����մϴ�!");
            PunManager.Instance.CreateAndMoveToPunRoom();
        }
    }
}
