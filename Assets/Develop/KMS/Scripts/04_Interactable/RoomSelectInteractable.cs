using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RoomSelectInteractable : XRGrabInteractable
{
    [Header("�θ� ������Ʈ")]
    public GameObject parentObject;
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;

    protected override void Awake()
    {
        base.Awake();

        _initialPosition = transform.position;
        _initialRotation = transform.rotation;
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
            transform.position = _initialPosition;
            transform.rotation = _initialRotation;
        }

        PunManager.Instance.JoinRoom(transform.name);

        parentObject.SetActive(false);
    }
}
