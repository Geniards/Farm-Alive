using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ModeSlectInteractable : XRGrabInteractable
{
    [Header("���� ������Ʈ ��Ȳ")]
    public GameObject stageObject;
    public GameObject parentObject;

    [Header("GameMode")]
    public E_GameMode gameMode;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    protected override void Awake()
    {
        base.Awake();

        initialPosition = transform.position;
        initialRotation = transform.rotation;
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

        // ��ü�� �ʱ� ��ġ�� ȸ������ ��� �̵�
        transform.position = initialPosition;
        transform.rotation = initialRotation;

        // ���õ� ���� ��� �Ѱ��ֱ�
        PunManager.Instance.SetGameMode(gameMode);

        stageObject.SetActive(true);
        parentObject.SetActive(false);
    }
}
