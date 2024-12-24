using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class FusionPlayer : NetworkBehaviour
{
    [Header("Camera and VR Components")]
    [SerializeField] private Camera cam;
    [SerializeField] private AudioListener audioListener;
    [SerializeField] private TrackedPoseDriver trackedPoseDriver;

    [SerializeField] private ActionBasedControllerManager leftControllerManager;
    [SerializeField] private ActionBasedControllerManager rightControllerManager;
    [SerializeField] private ActionBasedController leftController;
    [SerializeField] private ActionBasedController rightController;

    private void Start()
    {
        // ���� �÷��̾ �Է� �� ��ġ Ȱ��ȭ
        if (Object.HasInputAuthority)
        {
            EnableLocalPlayerComponents();
        }
        else
        {
            DisableNonLocalPlayerComponents();
        }
    }

    private void EnableLocalPlayerComponents()
    {
        Debug.Log("���� �÷��̾� ������Ʈ Ȱ��ȭ");

        cam.enabled = true;
        audioListener.enabled = true;
        trackedPoseDriver.enabled = true;
        leftController.enabled = true;
        rightController.enabled = true;

        leftControllerManager.enabled = true;
        rightControllerManager.enabled = true;
    }

    private void DisableNonLocalPlayerComponents()
    {
        Debug.Log("���� �÷��̾ �ƴ�. ������Ʈ ��Ȱ��ȭ");

        // 1. ī�޶� ��Ȱ��ȭ ��Ų��.
        cam.enabled = false;
        // 2. ����� �����ʸ� ��Ȱ��ȭ ��Ų��.
        audioListener.enabled = false;
        // 3. TRacked Pose Driver�� ��Ȱ��ȭ �Ͽ�, �Է¿� ���� ī�޶� �������� �ʵ��� �Ѵ�.
        trackedPoseDriver.enabled = false;
        // 4. ��Ʈ�ѷ��� �Է��� ��Ȱ��ȭ ��Ų��.
        leftController.enabled = false;
        rightController.enabled = false;

        leftControllerManager.enabled = false;
        rightControllerManager.enabled = false;
    }
}
