using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadVisiableController : MonoBehaviour
{
    public Camera vrCamera;
    public GameObject headObject;
    private bool _isCameraInside = false;

    void Start()
    {
        if (!vrCamera)
        {
            Debug.LogError("VR ī�޶� ������� �ʾҽ��ϴ�.");
        }

        if (!headObject)
        {
            Debug.LogError("�Ӹ� ������Ʈ�� ������� �ʾҽ��ϴ�.");
        }

        headObject.layer = LayerMask.NameToLayer("HeadLayer");
        vrCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("HeadLayer"));
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == vrCamera.gameObject)
        {
            _isCameraInside = true;
            vrCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("HeadLayer"));
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == vrCamera.gameObject)
        {
            _isCameraInside = false;
            vrCamera.cullingMask |= (1 << LayerMask.NameToLayer("HeadLayer"));
        }
    }
}
