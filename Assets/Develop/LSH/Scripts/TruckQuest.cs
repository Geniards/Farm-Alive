using Fusion;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class TruckQuest : MonoBehaviour
{
    [SerializeField] int truckId;
    [SerializeField] TruckController truckSpawner;
    [SerializeField] public Text questText;

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

                PhotonView boxView = other.GetComponent<PhotonView>();

                List<int> truckIds = new List<int>();
                List<int> itemIndexes = new List<int>();
                List<int> requiredCounts = new List<int>();

                for (int i = 0; i < boxTrigger.requiredItems.Count; i++)
                {
                    QuestManager.RequiredItem item = boxTrigger.requiredItems[i];
                    for (int j = 0; j < QuestManager.Instance.questsList[truckId].requiredItems.Count; j++)
                    {
                        if (item.itemPrefab.name == QuestManager.Instance.questsList[truckId].requiredItems[j].itemPrefab.name ||
                            item.itemPrefab.name == QuestManager.Instance.questsList[truckId].requiredItems[i].itemPrefab.name + "(Clone)")
                        {
                            Debug.Log("�̸��� ����");
                            Debug.Log($"{QuestManager.Instance.questsList[truckId].requiredItems[j].requiredcount} <= {item.requiredcount}");
                            if (QuestManager.Instance.questsList[truckId].requiredItems[j].requiredcount <= 0)
                                break;
                            Debug.Log("���� ���");

                            truckIds.Add(truckId);
                            itemIndexes.Add(j);
                            requiredCounts.Add(item.requiredcount);
                            break;
                        }
                    }
                }

                int[] truckIdArray = truckIds.ToArray();
                int[] itemIndexArray = itemIndexes.ToArray();
                int[] requiredCountArray = requiredCounts.ToArray();
                QuestManager.Instance.CountUpdate(truckIdArray, itemIndexArray, requiredCountArray, boxView.ViewID);
            }
        }
    }

    public void SetID(int truckId, TruckController truckController)
    {
        this.truckId = truckId;
        this.truckSpawner = truckController;
        Debug.Log($"������Ʈ ID: {truckId}");
    }

    public void ChangeID(int truckId)
    {
        this.truckId = truckId;
        Debug.Log($"������Ʈ ID: {truckId}");
    }

    private void OnDestroy()
    {
        if (truckSpawner != null)
        {
            QuestManager.Instance.truckList.RemoveAt(truckId);
            truckSpawner.ClearSlot(truckId);            
            Debug.Log("�����Ϸ�");
        }
    }
}