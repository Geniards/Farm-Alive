using UnityEngine;

public class HeadLayerController_Local : MonoBehaviour
{
    public Camera vrCamera;         // VR ī�޶� �巡�� �� ������� ����
    public GameObject headObject;   // �Ӹ� ������Ʈ�� ����
    public GameObject[] controllerObjects; // ��Ʈ�ѷ� ������Ʈ �迭
    void Start()
    {
        if (vrCamera != null && headObject != null)
        {
            // �Ӹ� ������Ʈ�� ���̾ ����
            headObject.layer = LayerMask.NameToLayer("HeadLayer");

            // ī�޶󿡼� �ش� ���̾ ����
            vrCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("HeadLayer"));
        }

        // Controller ���̾� ���� �� ī�޶󿡼� ����
        foreach (GameObject controller in controllerObjects)
        {
            if (controller != null)
            {
                controller.layer = LayerMask.NameToLayer("Controller");
                vrCamera.cullingMask |= (1 << LayerMask.NameToLayer("Controller"));
            }
        }
    }
}
