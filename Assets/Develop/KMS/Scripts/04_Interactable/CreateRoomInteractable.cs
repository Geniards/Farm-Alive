using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CreateRoomInteractable : XRGrabInteractable
{
    public GameObject modeObject;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private bool _isSelected;
    private Coroutine resetCoroutine;

    protected override void Awake()
    {
        base.Awake();

        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    private void Update()
    {
        // Select ���� �ƴϰ� �ʱ� ��ġ���� ��� ���
        if (!_isSelected && Vector3.Distance(transform.position, initialPosition) > 0.01f)
        {
            // �̹� �ʱ�ȭ �ڷ�ƾ�� ���� ���̸� ����
            if (resetCoroutine == null)
            {
                resetCoroutine = StartCoroutine(ResetToInitialPosition());
            }
        }
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        _isSelected = true;
        SoundManager.Instance.PlaySFX("SFX_Lobby_CropSelected");

        if (resetCoroutine != null)
        {
            StopCoroutine(resetCoroutine);
            resetCoroutine = null;
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        _isSelected = false;
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

    private IEnumerator ResetToInitialPosition()
    {
        // 3�� ���
        yield return new WaitForSeconds(3f);

        if (!isSelected)
        {
            transform.position = initialPosition;
            transform.rotation = initialRotation;
        }

        resetCoroutine = null;
    }
}
