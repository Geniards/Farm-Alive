using UnityEngine;
using UnityEngine.XR;

public class VR_Anim_Controller_Local : MonoBehaviour
{
    [Header("Controller Settings")]
    public Transform cameraTransform;                       
    public Animator animator;                               
    public float moveSpeed = 1.0f;                          
    public float rotationSpeed = 100.0f;
    public float hmdRotationThreshold = 30f;
    public float hmdLerpSpeed = 0.07f;

    private Vector2 _leftInputAxis;
    private Vector2 _rightInputAxis;
    private XRNode _leftControllerNode = XRNode.LeftHand;
    private XRNode _rightControllerNode = XRNode.RightHand;

    private bool _isMoving;
    private bool _isRotating;

    private float _previousHmdYaw;

    private void Start()
    {
        // �ʱ� ���� HMD Yaw �� ����
        _previousHmdYaw = cameraTransform.eulerAngles.y;
    }

    void Update()
    {
        InputDevice controller = InputDevices.GetDeviceAtXRNode(_leftControllerNode);
        if (controller.TryGetFeatureValue(CommonUsages.primary2DAxis, out _leftInputAxis))
        {
            if (_leftInputAxis != Vector2.zero)
            {
                MoveCharacter();
                animator.SetFloat("Speed", _leftInputAxis.magnitude);
            }
            else
            {
                animator.SetFloat("Speed", 0f);
            }
        }

        // ������ ��Ʈ�ѷ� �Է� ó�� (ȸ��)
        InputDevice rightController = InputDevices.GetDeviceAtXRNode(_rightControllerNode);
        if (rightController.TryGetFeatureValue(CommonUsages.primary2DAxis, out _rightInputAxis))
        {
            if (Mathf.Abs(_rightInputAxis.x) > 0.1f)
            {
                if (!_isRotating)
                {
                    _isRotating = true;
                }

                RotateCharacter(_rightInputAxis.x);
            }
            else
            {
                if (_isRotating)
                {
                    _isRotating = false;
                }
            }
        }

        // ���̽�ƽ ȸ�� ���� �ƴ� ��쿡�� HMD ȸ�� ����
        if (!_isRotating)
        {
            RotateCharacterWithHMD();
        }
    }

    private void RotateCharacter(float rotationInput)
    {
        float rotationAngle = rotationInput * rotationSpeed * Time.deltaTime;
        transform.Rotate(0, rotationAngle, 0);
    }

    private void RotateCharacterWithHMD()
    {
        float currentHmdYaw = cameraTransform.eulerAngles.y;
        float yawDelta = Mathf.DeltaAngle(_previousHmdYaw, currentHmdYaw);

        if (Mathf.Abs(yawDelta) > hmdRotationThreshold)
        {
            Vector3 targetRotation = new Vector3(0, currentHmdYaw, 0);

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(targetRotation), hmdLerpSpeed);
            _previousHmdYaw = currentHmdYaw;
        }
    }

    private void MoveCharacter()
    {
        // ī�޶� ������ �������� �̵� ���� ���
        Vector3 forward = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;
        Vector3 right = new Vector3(cameraTransform.right.x, 0, cameraTransform.right.z).normalized;

        Vector3 moveDirection = (forward * _leftInputAxis.y + right * _leftInputAxis.x).normalized;

        // ĳ���� �̵�
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }
}