using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class isSelectedTest : MonoBehaviour
{
    public XRGrabInteractable grabInteractable;

    private void Update()
    {

        if (grabInteractable != null && grabInteractable.isSelected)
        {
            // ������ ���� ���� ���� ó��
            Debug.Log("��� �ִ� ������ �½��ϴ�.");
        }
        else
        {
            // ������ ������ ���� ������ ���� ó��
            Debug.Log("������ ��� ���� �ʽ��ϴ�.");
        }
    }
}
