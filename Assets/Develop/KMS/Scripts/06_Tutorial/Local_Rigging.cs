using UnityEngine;

public class Local_Rigging : MonoBehaviour
{
    public Transform leftHandIK;            // �޼� IK 
    public Transform righttHandIK;          // ������ IK
    public Transform headIK;                // HMD IK

    public Transform leftHandController;    // �޼� ��Ʈ�ѷ�
    public Transform rightHandController;   // ������ ��Ʈ�ѷ�
    public Transform hmd;                   // HMD

    public Vector3[] leftOffset;            // �޼� Offset
    public Vector3[] rightOffset;           // ������ Offset
    public Vector3[] headOffset;            // hmd Offset

    public float smoothValue = 0.1f;        // �ε巴�� ������ ��
    public float modelHeight = 1.1176f;     // ĳ���� ���� ��

    /// <summary>
    /// ��Ʈ�ѷ��� ������ �� IK�� Transform�� ���߷���.
    /// </summary>
    private void LateUpdate()
    {
        // ���� �÷��̾��� ���� ó��
        MappingHandTranform(leftHandIK, leftHandController, true);
        MappingHandTranform(righttHandIK, rightHandController, false);
        MappingBodyTransform(headIK, hmd);
        MappingHeadTransform(headIK, hmd);

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
    /// <param name="ik"></param>
    /// <param name="hmd"></param>
    private void MappingBodyTransform(Transform ik, Transform hmd)
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
