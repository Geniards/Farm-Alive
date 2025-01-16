using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class ExitGameInteractable : XRGrabInteractable
{
    [Header("UI Elements")]
    public GameObject exitConfirmationPanel;
    public TMP_Text exitConfirmationText;

    [Header("Panel Setting")]
    public float panelDistance = 1.5f;
    public Vector3 panelOffset = Vector3.zero;

    public TMP_Text text;

    private bool _isExitRequest = false;
    private float _buttonPressDuration = 0f;
    private const float _requiredHoldTime = 1.0f;

    private Vector3 initialPosition;
    private Quaternion initialRotation;



    protected override void Awake()
    {
        base.Awake();

        initialPosition = transform.position;
        initialRotation = transform.rotation;

        if (exitConfirmationPanel)
            exitConfirmationPanel.SetActive(false);
        exitConfirmationText.text = "������ �����Ͻðڽ��ϱ�?\n(���� �����ϱ� - A / ���� ������ - B)";

        text.text = "���� ������";
    }

    private void Update()
    {
        HandleControllerInput();

        UpdatePanelPos();
    }
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
#if UNITY_EDITOR
        Debug.Log($"{args.interactableObject.transform.name}�� ���� �Ǿ����ϴ�.");
#endif
        StartCoroutine(ExitGameMode(args.interactableObject.transform.gameObject));
    }

    IEnumerator ExitGameMode(GameObject targetObject)
    {
        yield return null;

        if (targetObject)
        {
            // ��ü�� �ʱ� ��ġ�� ȸ������ ��� �̵�
            transform.position = initialPosition;
            transform.rotation = initialRotation;
        }

        ShowExitConfirmation();
    }
    private void HandleExitRequest(bool isPressed)
    {
        if (isPressed)
        {
            _buttonPressDuration += Time.deltaTime;

            if (_buttonPressDuration >= _requiredHoldTime && !_isExitRequest)
            {
                _isExitRequest = true;
                ShowExitConfirmation();
            }
        }
        else
        {
            // ��ư�� �������� �� �ʱ�ȭ
            _buttonPressDuration = 0f;
            _isExitRequest = false;
        }
    }

    private void HandleControllerInput()
    {
        InputDevice leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        InputDevice rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        // ��Ʈ�ѷ� Y ��ư (�˸�â ǥ��)
        if (leftController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool isYPressed))
        {
            HandleExitRequest(isYPressed);
        }

        // ��Ʈ�ѷ� A ��ư (���� ����)
        if (rightController.TryGetFeatureValue(CommonUsages.primaryButton, out bool isAPressed))
        {
            if (isAPressed && exitConfirmationPanel.activeSelf)
            {
                ConfirmExit();
            }
        }

        // ��Ʈ�ѷ� B ��ư (�˸�â �ݱ�)
        if (rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool isBPressed))
        {
            if (isBPressed && exitConfirmationPanel.activeSelf)
            {
                CancelExit();
            }
        }
    }

    private void ShowExitConfirmation()
    {
        if (exitConfirmationPanel)
        { 
            exitConfirmationPanel.SetActive(true);
            UpdatePanelPos();
        } 
    }

    private void HideExitConfirmation()
    {
        if (exitConfirmationPanel)
            exitConfirmationPanel.SetActive(false);
    }

    private void ConfirmExit()
    {
#if UNITY_EDITOR
        Debug.Log("���� ����!");
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    private void CancelExit()
    {
        Debug.Log("���� ���� ���!");
        HideExitConfirmation();
    }

    private void UpdatePanelPos()
    {
        if(exitConfirmationPanel.activeSelf)
        {
            Vector3 camForward = Camera.main.transform.forward;
            Vector3 camPos = Camera.main.transform.position;

            Vector3 panelPos = camPos + camForward * panelDistance + panelOffset;
            exitConfirmationPanel.transform.position = panelPos;

            exitConfirmationPanel.transform.rotation = Quaternion.LookRotation(panelPos - camPos);
        }
    }

}
