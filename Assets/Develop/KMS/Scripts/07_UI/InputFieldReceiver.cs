using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.SpatialKeyboard;

public class InputFieldReceiver : MonoBehaviour
{
    [SerializeField] private XRKeyboardDisplay _keyboardDisplay;

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
        // TODO : �� �̸� �Ѱ��ֱ�. 
    }
}
