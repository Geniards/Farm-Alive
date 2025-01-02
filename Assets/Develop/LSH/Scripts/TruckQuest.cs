using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TruckQuest : MonoBehaviour
{
    [SerializeField] int truckId;

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("�浹");
        if (PhotonNetwork.IsMasterClient)
        {
            if (other.CompareTag("Box"))
            {
                XRGrabInteractable interactable = other.GetComponent<XRGrabInteractable>();
                if (interactable.isSelected)
                    return;

                Debug.Log("�ڽ�");
                BoxTrigger boxTrigger = other.GetComponent<BoxTrigger>();
                if (boxTrigger == null)
                    return;

                /*foreach (QuestManager.RequiredItem item in boxTrigger.requiredItems)
                {
                    for (int i = 0; i < QuestManager.Instance.questsList[truckId].requiredItems.Count; i++)
                    {
                        if(item.itemPrefab.name == QuestManager.Instance.questsList[truckId].requiredItems[i].itemPrefab.name)
                        {
                            Debug.Log("�̸��� ����");
                            Debug.Log($"{QuestManager.Instance.questsList[truckId].requiredItems[i].requiredcount} <= {item.requiredcount}");
                            if (QuestManager.Instance.questsList[truckId].requiredItems[i].requiredcount <= 0)
                                break;
                            Debug.Log("���� ���");

                            QuestManager.Instance.CountUpdate(truckId, i, item.requiredcount);
                            break;
                            *//*if(item.requiredcount == QuestManager.Instance.questsList[truckId].requiredItems[i].requiredcount)
                            {
                                Debug.Log("ī��Ʈ�� ����");
                                QuestManager.Instance.SuccessQuest(truckId, i);
                                break;
                            }*//*
                        }
                    }
                }*/

                for (int i = 0; i < boxTrigger.requiredItems.Count; i++)
                {
                    QuestManager.RequiredItem item = boxTrigger.requiredItems[i];
                    for (int j = 0; j < QuestManager.Instance.questsList[truckId].requiredItems.Count; j++)
                    {
                        if (item.itemPrefab.name == QuestManager.Instance.questsList[truckId].requiredItems[j].itemPrefab.name)
                        {
                            Debug.Log("�̸��� ����");
                            Debug.Log($"{QuestManager.Instance.questsList[truckId].requiredItems[j].requiredcount} <= {item.requiredcount}");
                            if (QuestManager.Instance.questsList[truckId].requiredItems[j].requiredcount <= 0)
                                break;
                            Debug.Log("���� ���");

                            QuestManager.Instance.CountUpdate(truckId, i, item.requiredcount);
                            break;
                        }
                    }
                }

                PhotonNetwork.Destroy(other.gameObject);
            }
        }
    }
}
