using Photon.Voice.Unity.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger Enter �߻�");

        if (other.GetComponent<Crop>() != null)
        {
            Debug.Log($"{other.name}�� �ٱ��Ͽ� ���Խ��ϴ�.");
            other.transform.SetParent(transform);
            return;
        }

        else if (other.CompareTag("Tool"))
        {
            Debug.Log($"{other.name}�� �ٱ��Ͽ� ���Խ��ϴ�.");
            other.transform.SetParent(transform);
            return;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Crop>() != null)
        {
            Debug.Log($"{other.name}�� �ٱ��Ͽ��� �������ϴ�.");
            other.transform.SetParent(null);
            return;
        }

        else if (other.CompareTag("Tool"))
        {
            Debug.Log($"{other.name}�� �ٱ��Ͽ��� �������ϴ�.");
            other.transform.SetParent(null);
            return;
        }
    }
}
