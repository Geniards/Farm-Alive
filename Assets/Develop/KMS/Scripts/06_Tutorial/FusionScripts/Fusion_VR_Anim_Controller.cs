using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Fusion_VR_Anim_Controller : NetworkBehaviour
{
    [Header("Controller Settings")]
    public XRNode controllerNode = XRNode.LeftHand;     // �޼� ��Ʈ�ѷ�
    public float moveSpeed = 1.0f;                      // �̵� �ӵ�

    [Header("References")]
    public Transform cameraTransform;                  // ī�޶� Transform
    public Animator animator;                          // Animator ������Ʈ

    private Vector2 _inputAxis;                         // ���̽�ƽ �Է°�

    public override void FixedUpdateNetwork()
    {
        if (Object.HasInputAuthority)
        {
            // ��Ʈ�ѷ� ���̽�ƽ �Է� ��������
            InputDevice controller = InputDevices.GetDeviceAtXRNode(controllerNode);
            if (controller.TryGetFeatureValue(CommonUsages.primary2DAxis, out _inputAxis))
            {
                if (_inputAxis != Vector2.zero)
                {
                    MoveCharacter();
                    animator.SetFloat("Speed", _inputAxis.magnitude); // Speed �Ķ���� ����
                }
                else
                {
                    animator.SetFloat("Speed", 0f); // ���� ����
                }

                // �ٸ� Ŭ���̾�Ʈ�� �ִϸ��̼� ����ȭ
                RPC_SyncAnimation(_inputAxis.magnitude);
            }
        }
    }

    private void MoveCharacter()
    {
        if (Object.HasInputAuthority)
        {
            // ī�޶� ������ �������� �̵� ���� ���
            Vector3 forward = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;
            Vector3 right = new Vector3(cameraTransform.right.x, 0, cameraTransform.right.z).normalized;

            Vector3 moveDirection = (forward * _inputAxis.y + right * _inputAxis.x).normalized;

            // ĳ���� �̵�
            transform.position += moveDirection * moveSpeed * Runner.DeltaTime;
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    private void RPC_SyncAnimation(float speed)
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", speed);
        }
    }
}
