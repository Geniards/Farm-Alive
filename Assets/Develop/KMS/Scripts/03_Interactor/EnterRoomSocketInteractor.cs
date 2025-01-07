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
            Debug.Log($"{args.interactableObject.transform.name}�� ���Ͽ��� �������ϴ�.");
            PunManager.Instance.CreateAndMoveToPunRoom();

            //StartCoroutine(SelectObjectDestroy(args.interactableObject.transform.gameObject));
        }
    }

    IEnumerator SelectObjectDestroy(GameObject targetObject)
    {
        // 3�� ���
        yield return new WaitForSeconds(3f);

        // ������Ʈ�� null�� �ƴ��� Ȯ�� �� ����
        if (targetObject != null)
        {
            Debug.Log($"{targetObject.name} ���� �Ϸ�");
            Destroy(targetObject);
        }
    }
}
