using UnityEngine;

public class HeadLayerController_Local : MonoBehaviour
{
    public Camera vrCamera;         // VR ī�޶� �巡�� �� ������� ����
    public GameObject headObject;   // �Ӹ� ������Ʈ�� ����

    void Start()
    {
        if (vrCamera != null && headObject != null)
        {
            // �Ӹ� ������Ʈ�� ���̾ ����
            headObject.layer = LayerMask.NameToLayer("HeadLayer");

            // ī�޶󿡼� �ش� ���̾ ����
            vrCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("HeadLayer"));
        }
    }
}
