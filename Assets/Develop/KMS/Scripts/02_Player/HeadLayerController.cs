using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadLayerController : MonoBehaviourPun
{
    public Camera vrCamera;         // VR ī�޶� �巡�� �� ������� ����
    public GameObject headObject;   // �Ӹ� ������Ʈ�� ����

    void Start()
    {
        if (photonView.IsMine) // �� ������Ʈ�� ���� �÷��̾����� Ȯ��
        {
            if (vrCamera != null && headObject != null)
            {
                // �Ӹ� ������Ʈ�� ���̾ ����
                headObject.layer = LayerMask.NameToLayer("HeadLayer");

                // ī�޶󿡼� �ش� ���̾ ����
                vrCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("HeadLayer"));
            }
        }
        else
        {
            // ���� �÷��̾��� ��� �Ӹ� ������Ʈ�� ���̵��� ����
            headObject.layer = LayerMask.NameToLayer("Default");
        }
    }
}
