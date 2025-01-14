using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.XR;

public class SoundManager : MonoBehaviour
{
    public AudioMixer test; // ����� �ͼ�
    public GameObject sliderUI; // �����̴� UI �г�
    public XRNode rightControllerNode = XRNode.RightHand; // ������ ��Ʈ�ѷ�

    private float maxVolumeDb = 20f;
    private float minVolumeDb = -80f;
    private float buttonHoldTime = 1f; // ��ư�� ������ �ϴ� �ð�
    private float buttonHoldCounter = 0f; // ��ư ���� �ð� ����
    private bool isSliderUIActive = false; // �����̴� UI Ȱ��ȭ ����

    private Transform _mainCamera; // ���� ī�޶��� Transform
    public float distanceFromCamera = 3f; // ī�޶󿡼� �����̴� UI�� ������ �Ÿ�

    private void Start()
    {
        _mainCamera = Camera.main.transform;

        if (!_mainCamera)
        {
            Debug.LogError("Main Camera�� ã�� �� �����ϴ�!");
        }

        if (sliderUI)
        {
            sliderUI.SetActive(false);
        }
    }

    public void SetBGMVolume(float volume)
    {
        float dBValue = Mathf.Lerp(minVolumeDb, maxVolumeDb, volume);
        test.SetFloat("PlayerVoiceVolum", dBValue);
    }

    private void Update()
    {
        if (IsControllerButtonPressed(rightControllerNode, CommonUsages.primaryButton))
        {
            buttonHoldCounter += Time.deltaTime;

            if (buttonHoldCounter >= buttonHoldTime)
            {
                ToggleSliderUI();
                buttonHoldCounter = 0f;
            }
        }
#if UNITY_EDITOR
        else if (Input.GetKey(KeyCode.Slash))
        {
            buttonHoldCounter += Time.deltaTime;

            if (buttonHoldCounter >= buttonHoldTime)
            {
                ToggleSliderUI();
                buttonHoldCounter = 0f;
            }
        }
#endif
        else
        {
            buttonHoldCounter = 0f;
        }

        if (isSliderUIActive && sliderUI != null && _mainCamera != null)
        {
            UpdateSliderUIPosition();
        }
    }

    private void ToggleSliderUI()
    {
        isSliderUIActive = !isSliderUIActive;

        if (sliderUI != null)
        {
            sliderUI.SetActive(isSliderUIActive);

            if (isSliderUIActive)
            {
                UpdateSliderUIPosition();
            }
        }
    }

    private void UpdateSliderUIPosition()
    {
        Vector3 newPosition = _mainCamera.position + _mainCamera.forward * distanceFromCamera;
        sliderUI.transform.position = newPosition;
        sliderUI.transform.rotation = Quaternion.LookRotation(sliderUI.transform.position - _mainCamera.position);
    }

    private bool IsControllerButtonPressed(XRNode controllerNode, InputFeatureUsage<bool> button)
    {
        // ��Ʈ�ѷ� ��忡�� ��ư �Է� ���� Ȯ��
        InputDevice device = InputDevices.GetDeviceAtXRNode(controllerNode);
        if (device.isValid && device.TryGetFeatureValue(button, out bool isPressed))
        {
            return isPressed;
        }
        return false;
    }
}
