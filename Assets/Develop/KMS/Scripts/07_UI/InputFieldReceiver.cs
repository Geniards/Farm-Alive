using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.SpatialKeyboard;

public class InputFieldReceiver : MonoBehaviour
{
    [Tooltip("XR Ű����")]
    [SerializeField] private XRKeyboardDisplay _keyboardDisplay;
    [Tooltip("InputFiled ����")]
    [SerializeField] private TMP_InputField _inputField;
    [Tooltip("�� ���� ����")]
    [SerializeField] private TMP_Text _text;
    [Tooltip("�θ� ������Ʈ")]
    [SerializeField] private GameObject _gameObject;

    private string _roomName;

    private void OnEnable()
    {
        if (_keyboardDisplay)
        {
            // Ű���� ���͸� ������ �����ϴ� �̺�Ʈ
            _keyboardDisplay.onTextSubmitted.AddListener(OnKeyboardInputSubmitted);
        }

        if(_inputField)
        {
            // TODO : ���̾�̽����� �޾ƿ� ĳ������ �г��� + Room���� �⺻ �̸� �����ϱ�.
            _roomName = "Player" + "'s Room";
            _inputField.text = _roomName;
        }

        if(_text)
        {
            _text.transform.localScale = Vector3.one * 0.7f;
            _text.text = $"���� ��� : {PunManager.Instance.GetGameMode()}\n�������� : {PunManager.Instance.GetStageNumber()}";
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

        if (inputText == "")
        {
            inputText = _roomName;
            Debug.Log(_roomName);
            _inputField.text = _roomName;
        }
        _roomName = inputText;
    }

    public void CreateRoom()
    {
        _gameObject.SetActive(false);
        PunManager.Instance.SetRoomName(_roomName);
        PunManager.Instance.CreateAndMoveToPunRoom();
    }

    public void ReturnRoom()
    {
        _gameObject.SetActive(false);
    }
}
