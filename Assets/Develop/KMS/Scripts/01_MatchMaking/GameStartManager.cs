using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class GameStartManager : MonoBehaviour
{
    public GameObject introPanel;
    public TMP_Text introText;

    private int _currentStep = 0;
    private bool _isButtonPressed = false;

    [SerializeField] private string[] gameInstructions = new string[]
    {
        "Welcome VR World! Press B key",
        "This is Intro Text.",
        "10",
        "9",
        "8",
        "7",
        "6",
        "5",
        "4",
        "3",
        "2",
        "1",
        "Next stage in Press B Button..."
    };

    private void Start()
    {
        introPanel.SetActive(true);
        introText.text = gameInstructions[_currentStep];
    }

    private void Update()
    {
        InputDevice rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        // ��ư�� ���� �������� Ȯ��
        if (rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool isPressed))
        {
            if (isPressed && !_isButtonPressed) // ������ ���۵Ǿ��� ���� ó��
            {
                _isButtonPressed = true; // ��ư ���� ���� ���
                ShowNextInstruction();
            }
            else if (!isPressed) // ��ư�� �������� �� ���� �ʱ�ȭ
            {
                _isButtonPressed = false;
            }
        }
    }

    private void ShowNextInstruction()
    {
        _currentStep++;
        if (_currentStep < gameInstructions.Length)
        {
            introText.text = gameInstructions[_currentStep];
        }
        else
        {
            Debug.Log("Ʃ�丮�� ������ �̵�.");
            SceneManager.LoadScene("02_TutorialScene");
        }
    }
}
