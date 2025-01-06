using TMPro;
using UnityEngine;
using UnityEngine.XR;

public class GameExitManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject exitConfirmationPanel;
    public TMP_Text exitConfirmationText;

    private bool _isExitRequest = false;
    private float _buttonPressDuration = 0f;
    private const float _requiredHoldTime = 1.0f;

    private void Start()
    {
        if (exitConfirmationPanel)
            exitConfirmationPanel.SetActive(false);
        exitConfirmationText.text = "������ �����Ͻðڽ��ϱ�?\n(���� �����ϱ� - A / ���� ������ - B)";
    }

    private void Update()
    {
        HandleControllerInput();

#if UNITY_INCLUDE_TESTS
        HandleTestKeys();
#endif
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

#if UNITY_INCLUDE_TESTS
    private void HandleTestKeys()
    {
        // ESC Ű�� Y ��ư ����
        if (Input.GetKey(KeyCode.Escape))
        {
            HandleExitRequest(true);
        }
        else
        {
            HandleExitRequest(false);
        }

        // Y Ű�� A ��ư ���� (���� ����)
        if (Input.GetKeyDown(KeyCode.Y))
        {
            if (exitConfirmationPanel.activeSelf)
            {
                ConfirmExit();
            }
        }

        // N Ű�� B ��ư ���� (�˸�â �ݱ�)
        if (Input.GetKeyDown(KeyCode.N))
        {
            if (exitConfirmationPanel.activeSelf)
            {
                CancelExit();
            }
        }
    }
#endif

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

    private void ShowExitConfirmation()
    {
        if (exitConfirmationPanel)
            exitConfirmationPanel.SetActive(true);
    }

    private void HideExitConfirmation()
    {
        if (exitConfirmationPanel)
            exitConfirmationPanel.SetActive(false);
    }

    private void ConfirmExit()
    {
        Debug.Log("���� ����!");
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }

    private void CancelExit()
    {
        Debug.Log("���� ���� ���!");
        HideExitConfirmation();
    }
}
