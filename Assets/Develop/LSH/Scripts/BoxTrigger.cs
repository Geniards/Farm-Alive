using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BoxTrigger : MonoBehaviourPun
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("�浹");
        if (!other.CompareTag("Item"))
            return;
        Debug.Log("�±� ���");

        PhotonView itemView = other.GetComponent<PhotonView>();
        if (itemView == null || !itemView.IsMine) 
            return;
        Debug.Log("�� ���");

        if (QuestManager.Instance.currentQuest == null)
            return;
        Debug.Log("����Ʈ ���");

        GameObject[] requiredItems = QuestManager.Instance.currentQuest.requiredItems;
        bool isValidItem = false;

        foreach (GameObject item in requiredItems)
        {
            if (item.name == other.gameObject.name)
            {
                isValidItem = true;
                break;
            }
        }

        if (!isValidItem)
            return;
        Debug.Log("��ȿ������ ���");

        QuestManager.Instance.UpdateCount();
        Destroy(other.gameObject);        
    }
}