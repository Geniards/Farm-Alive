using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fusion_Rigging : NetworkBehaviour
{
    public Transform leftHandIK;            // �޼� IK 
    public Transform rightHandIK;           // ������ IK
    public Transform headIK;                // HMD IK

    public Transform leftHandController;    // �޼� ��Ʈ�ѷ�
    public Transform rightHandController;   // ������ ��Ʈ�ѷ�
    public Transform hmd;                   // HMD

    public Vector3[] leftOffset;            // �޼� Offset
    public Vector3[] rightOffset;           // ������ Offset
    public Vector3[] headOffset;            // HMD Offset

    public float smoothValue = 0.1f;        // �ε巴�� ������ ��
    public float modelHeight = 1.1176f;     // ĳ���� ���� ��

    /// <summary>
    /// LateUpdate ��� Fusion�� FixedUpdateNetwork�� ����Ͽ� ����ȭ
    /// </summary>
    public override void FixedUpdateNetwork()
    {
        if (Object.HasInputAuthority)
        {
            // ���� �÷��̾��� ���� ó��
            MappingHandTransform(leftHandIK, leftHandController, true);
            MappingHandTransform(rightHandIK, rightHandController, false);
            MappingBodyTransform(headIK, hmd);
            MappingHeadTransform(headIK, hmd);

            // ����ȭ�� ��ġ �� ȸ���� RPC�� ����
            RPC_SyncIK(
                leftHandIK.position, leftHandIK.rotation,
                rightHandIK.position, rightHandIK.rotation,
                headIK.position, headIK.rotation);
        }
    }

    /// <summary>
    /// ��Ʈ�ѷ��� ��ũ�� ���߱� ���� Offset.
    /// </summary>
    /// <param name="ik"></param>
    /// <param name="controller"></param>
    /// <param name="isLeft"></param>
    private void MappingHandTransform(Transform ik, Transform controller, bool isLeft)
    {
        // IK�� Transform = Controller�� Transform
        var offset = isLeft ? leftOffset : rightOffset;

        // ��Ʈ�ѷ� ��ġ ��
        ik.position = controller.TransformPoint(offset[0]);
        // ��Ʈ�ѷ� ȸ�� ��
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

    /// <summary>
    /// Fusion RPC�� ����ȭ�� IK �����͸� ����.
    /// </summary>
    /// <param name="leftHandPos"></param>
    /// <param name="leftHandRot"></param>
    /// <param name="rightHandPos"></param>
    /// <param name="rightHandRot"></param>
    /// <param name="headPos"></param>
    /// <param name="headRot"></param>
    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_SyncIK(Vector3 leftHandPos, Quaternion leftHandRot,
        Vector3 rightHandPos, Quaternion rightHandRot, Vector3 headPos, Quaternion headRot)
    {
        if (!Object.HasInputAuthority)
        {
            leftHandIK.position = leftHandPos;
            leftHandIK.rotation = leftHandRot;

            rightHandIK.position = rightHandPos;
            rightHandIK.rotation = rightHandRot;

            headIK.position = headPos;
            headIK.rotation = headRot;
        }
    }
}
