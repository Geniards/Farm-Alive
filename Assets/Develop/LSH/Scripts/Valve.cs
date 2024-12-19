using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;
public class Valve : MonoBehaviour
{
    [SerializeField] XRKnob knob;
    [SerializeField] GameObject waterEffect;
    [SerializeField] bool isHoseConnected = false;
    [SerializeField] Transform hosePoint;

    private void Start()
    {
        knob.onValueChange.AddListener(OnChangeValue);
    }

    private void OnChangeValue(float valveValue)
    {
        Debug.Log("������");
        if (valveValue > 0.5f)
        {
            StartWater();
        }
        else
        {
            StopWater();
        }
    }

    private void StartWater()
    {
        Debug.Log("��Ȱ��ȭ");
    }

    private void StopWater()
    {
        Debug.Log("����Ȱ��ȭ");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("�浹");

        if (hosePoint == null)
        {
            Debug.LogError("ȣ�����ΰ� ����");
            return;
        }


        if (other.gameObject.tag == "Player")
        {
            XRGrabInteractable grabInteractable = other.GetComponent<XRGrabInteractable>();
            if (grabInteractable != null)
            {
                grabInteractable.selectExited.AddListener(x => OnHoseReleased(grabInteractable));
            }

        }
    }

    private void OnHoseReleased(XRGrabInteractable grabInteractable)
    {
        if (isHoseConnected) return;
        
        Transform hoseTransform = grabInteractable.transform;

        Transform startPoint = hoseTransform.Find("StartPoint");
        if (startPoint == null)
        {
            Debug.LogError("��ŸƮ����Ʈ�� ����");
            return;
        }

        if (startPoint == null)
        {
            Debug.LogError("��ŸƮ����Ʈ�� ����");
            foreach (Transform child in hoseTransform)
            {
                Debug.Log($"ȣ���� ���� ������Ʈ: {child.name}");
            }
            return;
        }


        AlignHose(hoseTransform, startPoint);


        Rigidbody hoseRigidbody = grabInteractable.GetComponent<Rigidbody>();
        if (hoseRigidbody != null)
        {
            hoseRigidbody.isKinematic = true;
        }

        isHoseConnected = true;

        Debug.Log("ȣ�� ���� �Ϸ�!!");
    }

    private void AlignHose(Transform hoseTransform, Transform startPoint)
    {
        Vector3 positionOffset = hosePoint.position - startPoint.position;
        hoseTransform.position += positionOffset;

        Quaternion rotationOffset = Quaternion.FromToRotation(startPoint.forward, hosePoint.forward);
        hoseTransform.rotation = rotationOffset * hoseTransform.rotation;

        hoseTransform.SetParent(transform);
    }
}