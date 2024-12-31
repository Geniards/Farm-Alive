using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TruckQuest : MonoBehaviour
{
    [SerializeField] int truckId;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("�浹");
        if (other.CompareTag("Box"))
        {
            Debug.Log("�ڽ�");
            BoxTrigger boxTrigger = other.GetComponent<BoxTrigger>();
            if (boxTrigger == null)
                return;

            foreach (QuestManager.RequiredItem item in boxTrigger.requiredItems)
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
                        /*if(item.requiredcount == QuestManager.Instance.questsList[truckId].requiredItems[i].requiredcount)
                        {
                            Debug.Log("ī��Ʈ�� ����");
                            QuestManager.Instance.SuccessQuest(truckId, i);
                            break;
                        }*/
                    }
                }
            }
        }
    }
}
