using Fusion;
using UnityEngine;

public class HeadLayerController_Fusion : NetworkBehaviour
{
    public Camera vrCamera;
    public GameObject headObject;
    public GameObject[] controllerObjects;
    public GameObject[] rayInteractors;

    void Start()
    {
        if (Object.HasStateAuthority) // �� ������Ʈ�� ���� �÷��̾����� Ȯ��
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
                if (controller != null)
                {
                    controller.SetActive(false);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (!Object.HasStateAuthority)
        {
            foreach (GameObject rayInteractor in rayInteractors)
            {
                if (rayInteractor != null && rayInteractor.activeSelf)
                {
                    rayInteractor.SetActive(false);
                }
            }
        }
    }
}
