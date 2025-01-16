using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RoomSelectInteractable : XRGrabInteractable
{
    [Header("�θ� ������Ʈ")]
    public GameObject parentObject;
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;
    private TMP_Text _text;


    protected override void Awake()
    {
        base.Awake();

        _initialPosition = transform.position;
        _initialRotation = transform.rotation;

        _text = GetComponent<TMP_Text>();
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
            transform.position = _initialPosition;
            transform.rotation = _initialRotation;
        }

        Debug.Log(_text.text);

        PunManager.Instance.JoinRoom(_text.text);

        parentObject.SetActive(false);
    }
}
