using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class StageSelectInteractable : XRGrabInteractable
{
    public GameObject parentObject;

    [Header("UI ����")]
    public GameObject uiPrefab;
    public Vector3 uiOffset;
    public Quaternion rotation;
    public float scale;
    
    private GameObject instantiatedUI;

    [Header("Global Keyboard ����")]
    public GameObject globalInputFieldPrefab;
    private GameObject instantiatedInputField;

    [Header("Stage Number")]
    public E_StageMode stageMode;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    protected override void Awake()
    {
        base.Awake();

        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
#if UNITY_EDITOR
        Debug.Log($"{args.interactableObject.transform.name}�� Hover�� ���õǾ����ϴ�.");
#endif
        if (instantiatedUI)
        {
            instantiatedUI.SetActive(true);
        }
        else if (uiPrefab)
        {
            instantiatedUI = Instantiate(uiPrefab, transform.position + transform.right + uiOffset, rotation);
            instantiatedUI.transform.SetParent(transform);

            TextMeshPro tmp = instantiatedUI.GetComponentInChildren<TextMeshPro>();
            if (tmp)
            {
                tmp.transform.localScale = Vector3.one * scale;
                // TODO : ���̾�̽����� HighstStage���� �����ͼ� �ش� ���� �������� �̸��� ���ؼ�
                // �ؽ�Ʈ ����ϱ�
                tmp.text = $"{gameObject.name} ���� ����";
            }
        }
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
#if UNITY_EDITOR
        Debug.Log($"{args.interactableObject.transform.name}�� Hover�� ���� �Ǿ����ϴ�.");
#endif
        TurnOnUi(false);
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
#if UNITY_EDITOR
        Debug.Log($"{args.interactableObject.transform.name}�� Select�� �Ǿ����ϴ�.");
#endif
        // TODO : ���̾�̽����� HighstStage���� �����ͼ� �ش� ���� �������� �̸��� ���ؼ�
        // ���ýÿ� �ش� ���������� ���� �Ҽ� �ִ��� �ľ��ϱ�.

        TurnOnUi(false);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
#if UNITY_EDITOR
        Debug.Log($"{args.interactableObject.transform.name}�� Select�� ���� �Ǿ����ϴ�.");
#endif
        StartCoroutine(SelectObjectDestroy(args.interactableObject.transform.gameObject));
    }

    IEnumerator SelectObjectDestroy(GameObject targetObject)
    {
        // 3�� ���
        yield return new WaitForSeconds(3f);

        transform.position = initialPosition;
        transform.rotation = initialRotation;

        // ���õ� StageNumber ��ȯ.
        Debug.Log($"�������� ��ȯ : {stageMode}");
        PunManager.Instance.SetStageNumber(stageMode);

        ActivateGlobalKeyboard();

        parentObject.SetActive(false);
    }

    private void TurnOnUi(bool isOn)
    {
        if (instantiatedUI)
        {
            instantiatedUI.SetActive(isOn);
        }
    }

    private void ActivateGlobalKeyboard()
    {
        if (globalInputFieldPrefab)
        {
            if (!instantiatedInputField)
            {
                Transform mainCameraTransform = Camera.main.transform;

                Vector3 directionToCamera = (mainCameraTransform.position - transform.position).normalized;
                Vector3 spawnPosition = transform.position + directionToCamera * 2.0f + Vector3.up * 1.0f; // ���� 2m, ���� 1m

                instantiatedInputField = Instantiate(globalInputFieldPrefab, spawnPosition, Quaternion.LookRotation(-directionToCamera));
                instantiatedInputField.transform.localScale = Vector3.one * 0.02f;
            }
            else
            {
                instantiatedInputField.SetActive(true);
            }
        }
    }
}
