using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ReturnInteractable : XRGrabInteractable
{
    [Header("���� ������Ʈ ��Ȳ")]
    public GameObject prevObject;
    public GameObject parentObject;

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
        Debug.Log($"{args.interactableObject.transform.name}�� ���õǾ����ϴ�.");
#endif
        StartCoroutine(SelectObjectDestroy(args.interactableObject.transform.gameObject));
    }

    IEnumerator SelectObjectDestroy(GameObject targetObject)
    {
        // 3�� ���
        yield return new WaitForSeconds(3f);

        transform.position = initialPosition;
        transform.rotation = initialRotation;

        if (!prevObject)
        {
            parentObject.SetActive(false);
            yield return null;
        }

        prevObject.SetActive(true);
        parentObject.SetActive(false);
    }
}
