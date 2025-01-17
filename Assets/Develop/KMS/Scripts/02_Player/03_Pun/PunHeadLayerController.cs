using Photon.Pun;
using UnityEngine;

public class PunHeadLayerController : MonoBehaviourPun
{
    public Camera vrCamera;         // VR ī�޶� �巡�� �� ������� ����
    public GameObject headObject;   // �Ӹ� ������Ʈ�� ����
    public GameObject[] controllerObjects;   // ��Ʈ�ѷ� ������Ʈ �迭 (���� �÷��̾��� ��Ʈ�ѷ�)

    void Start()
    {
        if (photonView.IsMine) // �� ������Ʈ�� ���� �÷��̾����� Ȯ��
        {
            if (vrCamera && headObject)
            {
                // �Ӹ� ������Ʈ�� ���̾ ����
                headObject.layer = LayerMask.NameToLayer("HeadLayer");

                // ī�޶󿡼� �ش� ���̾ ����
                vrCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("HeadLayer"));
            }

            // Controller ���̾� ���� �� ī�޶󿡼� ����
            foreach (GameObject controller in controllerObjects)
            {
                if (controller)
                {
                    controller.layer = LayerMask.NameToLayer("Controller");
                    vrCamera.cullingMask |= (1 << LayerMask.NameToLayer("Controller"));
                }
            }
        }
        else
        {
            // ���� �÷��̾� ó��
            if (headObject)
            {
                // ���� �÷��̾��� �Ӹ� ���̾ Default�� �����Ͽ� ���̰� ��
                headObject.layer = LayerMask.NameToLayer("Default");
            }

            // ���� �÷��̾��� ��Ʈ�ѷ� ��Ȱ��ȭ
            foreach (GameObject controller in controllerObjects)
            {
                if (controller)
                {
                    controller.SetActive(false);
                }
            }
        }
    }
}
