using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class OptionUI : MonoBehaviour
{
    public GameObject canvas;
    public float distanceFromCamera = 3f;
    public XRNode controllerNode = XRNode.LeftHand;

    private bool isCanvasActive = false;
    private bool isButtonPressed = false;

    void Start()
    {
        if (canvas)
        {
            canvas.SetActive(false);
        }
    }

    void Update()
    {
        // ��Ʈ�ѷ��� Y ��ư �Է� ���� Ȯ��
        bool buttonPressed = IsControllerButtonPressed(controllerNode, CommonUsages.primaryButton);

        // ��ư�� ���ȴٰ� ������ ������ ���� ��� ����
        if (buttonPressed && !isButtonPressed)
        {
            ToggleCanvas();
        }

        // ��ư ���� ������Ʈ
        isButtonPressed = buttonPressed;

        // ĵ������ Ȱ��ȭ�Ǿ� ������ ��ġ ������Ʈ
        if (isCanvasActive && canvas)
        {
            UpdateCanvasPosition();
        }
    }

    /// <summary>
    /// ĵ������ Ȱ��ȭ/��Ȱ��ȭ ��ȯ
    /// </summary>
    private void ToggleCanvas()
    {
        isCanvasActive = !isCanvasActive;

        if (canvas)
        {
            canvas.SetActive(isCanvasActive);

            if (isCanvasActive)
            {
                UpdateCanvasPosition();
            }
        }
    }

    /// <summary>
    /// ĵ���� ��ġ �� ȸ���� ī�޶� �������� ������Ʈ
    /// </summary>
    private void UpdateCanvasPosition()
    {
        if (!canvas) return;

        canvas.transform.position = Camera.main.transform.position + Camera.main.transform.forward * distanceFromCamera;
        canvas.transform.rotation = Quaternion.LookRotation(canvas.transform.position - Camera.main.transform.position);
    }

    /// <summary>
    /// ��Ʈ�ѷ��� ��ư �Է� Ȯ��
    /// </summary>
    private bool IsControllerButtonPressed(XRNode node, InputFeatureUsage<bool> button)
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(node);
        if (device.isValid && device.TryGetFeatureValue(button, out bool isPressed))
        {
            return isPressed;
        }
        return false;
    }
}