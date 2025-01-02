using UnityEngine;
using UnityEngine.XR;

public class Local_Rigging : MonoBehaviour
{
    public Transform leftHandIK;            // �޼� IK 
    public Transform righttHandIK;          // ������ IK
    public Transform headIK;                // HMD IK

    public Transform leftHandController;    // �޼� ��Ʈ�ѷ�
    public Transform rightHandController;   // ������ ��Ʈ�ѷ�
    public Transform hmd;                   // HMD
    public Transform cameraOffset;

    public Vector3[] leftOffset;            // �޼� Offset
    public Vector3[] rightOffset;           // ������ Offset
    public Vector3[] headOffset;            // hmd Offset

    [Tooltip("���� ������")]
    public float smoothValue = 0.1f;        // �ε巴�� ������ ��
    [Tooltip("ĳ���� �⺻ ����")]
    public float modelHeight = 1.1176f;     // ĳ���� ���� ��
    [Tooltip("ĳ���� ���� �����ӵ�")]
    public float heightAdjustSpeed = 1f; // ���� ���� �ӵ�

    private float cameraHeightAdjustment = 0;

    /// <summary>
    /// ��Ʈ�ѷ��� ������ �� IK�� Transform�� ���߷���.
    /// </summary>
    private void LateUpdate()
    {
        // ������ ��Ʈ�ѷ��� ���̽�ƽ Y�� �Է����� modelHeight ����
        AdjustModelHeight();

        // ���� �÷��̾��� ���� ó��
        MappingHandTranform(leftHandIK, leftHandController, true);
        MappingHandTranform(righttHandIK, rightHandController, false);
        MappingBodyTransform(hmd);
        MappingHeadTransform(headIK, hmd);
    }

    /// <summary>
    /// ������ ��Ʈ�ѷ� ���̽�ƽ Y������ cameraHeightAdjustment ����
    /// </summary>
    private void AdjustModelHeight()
    {
        InputDevice rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        if (rightController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 joystickInput))
        {
            if (Mathf.Abs(joystickInput.y) > 0.01f)
            {
                cameraHeightAdjustment = joystickInput.y * heightAdjustSpeed * Time.deltaTime;
                cameraOffset.position = new Vector3(cameraOffset.position.x, cameraOffset.position.y + cameraHeightAdjustment, cameraOffset.position.z);
            }
        }

    }

    /// <summary>
    /// ��Ʈ�ѷ��� ��ũ�� ���߱� ���� Offset.
    /// </summary>
    /// <param name="ik"></param>
    /// <param name="controller"></param>
    /// <param name="isLeft"></param>
    private void MappingHandTranform(Transform ik, Transform controller, bool isLeft)
    {
        // ik�� Transform = Controller�� Transform
        var offset = isLeft ? leftOffset : rightOffset;

        // ��Ʈ�ѷ� ��ġ ��. [0]
        ik.position = controller.TransformPoint(offset[0]);
        // ��Ʈ�ѷ� ȸ�� ��. [1]
        ik.rotation = controller.rotation * Quaternion.Euler(offset[1]);
    }

    /// <summary>
    /// HMD�� ĳ������ ���� ���� ���ư����� ������ �޼���.
    /// </summary>
    /// <param name="hmd"></param>
    private void MappingBodyTransform(Transform hmd)
    {
        this.transform.position = new Vector3(hmd.position.x, hmd.position.y - modelHeight, hmd.position.z);
        float yaw = hmd.eulerAngles.y;
        var targetRotation = new Vector3(this.transform.eulerAngles.x, yaw, this.transform.eulerAngles.z);
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.Euler(targetRotation), smoothValue);
    }

    /// <summary>
    /// HMD�� IK Offset.
    /// </summary>
    /// <param name="ik"></param>
    /// <param name="hmd"></param>
    private void MappingHeadTransform(Transform ik, Transform hmd)
    {
        ik.position = hmd.TransformPoint(headOffset[0]);
        ik.rotation = hmd.rotation * Quaternion.Euler(headOffset[1]);
    }
}
