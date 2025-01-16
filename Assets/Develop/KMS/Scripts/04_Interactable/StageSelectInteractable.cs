using System;
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
        SoundManager.Instance.PlaySFX("SFX_Lobby_CropHovered");

        string highStage = FirebaseManager.Instance.GetHighStage();

        // ���� ���������� HighStage �� (�񿬼� Enum ����)
        Array stageModes = Enum.GetValues(typeof(E_StageMode));
        int currentIndex = Array.IndexOf(stageModes, stageMode);
        int highStageIndex = Array.IndexOf(stageModes, Enum.Parse(typeof(E_StageMode), highStage));

        if (currentIndex <= highStageIndex + 1) // ���� ���� ����
        {
            base.OnHoverEntered(args);

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

                    // ĳ�̵� ������ ���
                    var stageData = FirebaseManager.Instance.GetCachedStageData((int)stageMode);

                    if (stageData != null)
                    {
                        int stars = stageData.stars;
                        float playTime = stageData.playTime;

                        tmp.text = $"��������: {stageMode}\n" +
                                   $"��Ÿ: {stars}\n" +
                                   $"�÷��� Ÿ��: {playTime}��";
                    }
                    else
                    {
                        tmp.text = $"��������: {stageMode}\n" +
                               "��Ÿ: ������ ����\n" +
                               "�÷��� Ÿ��: ������ ����";
                    }
                }
            }
        }
        else // ���� �Ұ���
        {
            Debug.LogWarning($"���� ��������({stageMode})�� ������ �� �����ϴ�. HighStage�� {highStage}�Դϴ�.");
            ShowUnavailableUI();
        }
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        TurnOnUi(false);
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        SoundManager.Instance.PlaySFX("SFX_Lobby_CropSelected");
        TurnOnUi(false);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        string highStage = FirebaseManager.Instance.GetHighStage();

        Array stageModes = Enum.GetValues(typeof(E_StageMode));
        int currentIndex = Array.IndexOf(stageModes, stageMode);
        int highStageIndex = Array.IndexOf(stageModes, Enum.Parse(typeof(E_StageMode), highStage));

        if (currentIndex <= highStageIndex + 1) // ���� ���� ����
        {
            base.OnSelectExited(args);
            StartCoroutine(SelectObjectDestroy(args.interactableObject.transform.gameObject));
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogWarning($"��������({stageMode})�� ���� �Ұ����մϴ�. OnSelectExited ������ �����մϴ�.");
#endif
            StartCoroutine(ResetPosition());
        }
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
                Vector3 spawnPosition = transform.position + directionToCamera * 2.0f + Vector3.up * 1.0f;

                instantiatedInputField = Instantiate(globalInputFieldPrefab, spawnPosition, Quaternion.LookRotation(-directionToCamera));
                instantiatedInputField.transform.localScale = Vector3.one * 0.02f;
            }
            else
            {
                instantiatedInputField.SetActive(true);
            }
        }
    }

    // ���� �Ұ��� �� �˸� UI ǥ��
    private void ShowUnavailableUI()
    {
        if (instantiatedUI)
        {
            TextMeshPro tmp = instantiatedUI.GetComponentInChildren<TextMeshPro>();
            if (tmp)
            {
                tmp.text = $"{gameObject.name} ���� �Ұ�";
            }
        }
    }

    private IEnumerator ResetPosition()
    {
        yield return new WaitForSeconds(0.5f);

        transform.position = initialPosition;
        transform.rotation = initialRotation;
    }
}
