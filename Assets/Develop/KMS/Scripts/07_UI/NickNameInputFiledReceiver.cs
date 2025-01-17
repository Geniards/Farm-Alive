using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.SpatialKeyboard;

public class NickNameInputFiledReceiver : MonoBehaviour
{
    [Tooltip("XR Ű����")]
    [SerializeField] private XRKeyboardDisplay _keyboardDisplay;
    [Tooltip("InputFiled ����")]
    [SerializeField] private TMP_InputField _inputField;
    [Tooltip("Test ���")]
    [SerializeField] private TMP_Text _text; 
    private string _nickName;

    private void OnEnable()
    {
        if (_keyboardDisplay)
        {
            // Ű���� ���͸� ������ �����ϴ� �̺�Ʈ
            _keyboardDisplay.onTextSubmitted.AddListener(OnKeyboardInputSubmitted);
        }
    }

    private void OnDisable()
    {
        if (_keyboardDisplay)
        {
            _keyboardDisplay.onTextSubmitted.RemoveListener(OnKeyboardInputSubmitted);
        }
    }

    // Ű���� �Է°��� ó���ϴ� �޼���
    private void OnKeyboardInputSubmitted(string inputText)
    {
        Debug.Log($"�Է� ���� �ؽ�Ʈ : {inputText}");

        _nickName = inputText;
    }

    public void CreateNickName()
    {
        if (string.IsNullOrWhiteSpace(_nickName))
        {
            MessageDisplayManager.Instance.ShowMessage($"�г����� ����ֽ��ϴ�. �ٽ� �Է����ּ���!", 1f, 3f);
            _text.text = "�г����� �Է����ּ���.";
            _inputField.text = "";
            _inputField.ActivateInputField();
            return;
        }

        Debug.Log($"�г��� ���� �Ϸ�: {_nickName}");

        _inputField.gameObject.SetActive(false);
        _text.text = "�κ�� �����մϴ�!";
        FirebaseManager.Instance.SaveNickName(_nickName);

        SceneLoader.LoadSceneWithLoading("03_Lobby");
    }
}
