using UnityEngine;
using UnityEngine.XR;

public class VR_Anim_Controller_Local : MonoBehaviour
{
    public XRNode controllerNode = XRNode.LeftHand;     // �޼� ��Ʈ�ѷ�
    public float moveSpeed = 1.0f;                      // �̵� �ӵ�
    public Transform cameraTransform;                   // ī�޶� Transform
    public Animator animator;                           // Animator ������Ʈ
    private Vector2 inputAxis;                          // ���̽�ƽ �Է°�

    void Update()
    {
        InputDevice controller = InputDevices.GetDeviceAtXRNode(controllerNode);
        if (controller.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis))
        {
            if (inputAxis != Vector2.zero)
            {
                MoveCharacter();
                animator.SetFloat("Speed", inputAxis.magnitude); // Speed �Ķ���� ����
            }
            else
            {
                animator.SetFloat("Speed", 0f); // ���� ����
            }
        }

    }

    private void MoveCharacter()
    {
        // ī�޶� ������ �������� �̵� ���� ���
        Vector3 forward = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;
        Vector3 right = new Vector3(cameraTransform.right.x, 0, cameraTransform.right.z).normalized;

        Vector3 moveDirection = (forward * inputAxis.y + right * inputAxis.x).normalized;

        // ĳ���� �̵�
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }
}