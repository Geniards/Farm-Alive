using Fusion;
using UnityEngine;

public class HeadLayerController_Fusion : NetworkBehaviour
{
    public Camera vrCamera;         // VR ī�޶� �巡�� �� ������� ����
    public GameObject headObject;   // �Ӹ� ������Ʈ�� ����

    void Start()
    {
        if (Object.HasStateAuthority) // �� ������Ʈ�� ���� �÷��̾����� Ȯ��
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
