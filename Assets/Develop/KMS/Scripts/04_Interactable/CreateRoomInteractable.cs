using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CreateRoomInteractable : XRGrabInteractable
{
    public GameObject modeObject;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    protected override void Awake()
    {
        base.Awake();

        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        SoundManager.Instance.PlaySFX("SFX_Lobby_CropSelected");
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
#if UNITY_EDITOR
        Debug.Log($"{args.interactableObject.transform.name}�� ���� �Ǿ����ϴ�.");
#endif
        StartCoroutine(SelectObjectDestroy(args.interactableObject.transform.gameObject));
    }

    IEnumerator SelectObjectDestroy(GameObject targetObject)
    {
        // 3�� ���
        yield return new WaitForSeconds(3f);

        if (targetObject)
        {
            // ��ü�� �ʱ� ��ġ�� ȸ������ ��� �̵�
            transform.position = initialPosition;
            transform.rotation = initialRotation;
        }

        modeObject.SetActive(true);
    }
}
