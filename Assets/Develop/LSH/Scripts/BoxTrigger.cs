using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;
using static QuestManager;

public class BoxTrigger : MonoBehaviourPun
{
    [SerializeField] public GameObject boxCover1;
    [SerializeField] public GameObject boxCover2;
    [SerializeField] public GameObject boxTape;
    [SerializeField] public List<RequiredItem> requiredItems;
    [SerializeField] bool isBoxClose = false;
    [SerializeField] bool isBoxSealed = false;

    private void OnEnable()
    {
        Debug.Log("����Ʈ");
        requiredItems = new List<RequiredItem>();
    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Crop"))
        {
            PhotonView itemView = other.GetComponent<PhotonView>();
            if (itemView == null || !itemView.IsMine)
                return;

            XRGrabInteractable grabInteractable = other.GetComponent<XRGrabInteractable>();
            if (grabInteractable != null && !grabInteractable.isSelected)
                return;

            if (QuestManager.Instance.currentQuest == null)
                return;
            #region ���������? �ڵ�
            //List<QuestManager.Quest> requiredItem = QuestManager.Instance.questsList;
            //bool isValidItem = false;

            /*int x = 0;
            int y = 0;
            foreach (QuestManager.Quest item in requiredItem)
            {
                for (int i = 0; i < item.requiredItems.Count; i++)
                {
                    if (item.requiredItems[i].itemPrefab.name == other.gameObject.name)
                    {
                        if (item.requiredItems[i].count < item.requiredItems[i].requiredcount)
                        {
                            isValidItem = true;
                            Debug.Log($"{x+1}��°�� ����Ʈ�� {y+1}��° ������");
                            break;
                        }
                        else
                        {
                            Debug.Log("���� �ʰ�");
                        }
                    }
                    y++;
                }
                x++;
            }*/
            #endregion
            foreach (QuestManager.RequiredItem item in requiredItems)
            {
                if (item.itemPrefab.name == other.gameObject.name)
                {
                    item.count++;
                }
                else
                {
                    requiredItems.Add(new RequiredItem(other.gameObject, 1));
                }
            }

            #region ���������? �ڵ�
            //if (!isValidItem)
            //    return;

            //QuestManager.Instance.UpdateCount(x, y);

            /*if (QuestManager.Instance.IsQuestComplete())
            {
                CloseCover();
            }*/
            #endregion
        }

        else if (!isBoxSealed && other.CompareTag("Tape"))
        {
            Debug.Log("�������");

            Taping taping = other.GetComponent<Taping>();
            if (taping != null && !isBoxSealed)
            {
                Debug.Log("���������ν����غ�");
                taping.StartTaping(this);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Tape"))
        {
            Taping taping = other.GetComponent<Taping>();
            if (taping != null)
            {
                taping.StopTaping();
            }
        }
        else if (other.CompareTag("Crop"))
        {
            XRGrabInteractable grabInteractable = other.GetComponent<XRGrabInteractable>();
            if (grabInteractable != null && !grabInteractable.isSelected)
                return;

            if (requiredItems.Count > 0)
            {
                for (int i = requiredItems.Count - 1; i >= 0; i--)
                {
                    if (requiredItems[i].itemPrefab.name == other.gameObject.name)
                    {
                        requiredItems[i].count--;

                        if (requiredItems[i].count == 0)
                        {
                            requiredItems.RemoveAt(i);
                        }
                    }
                }
            }
            //QuestManager.Instance.ExitItemCount();
        }
    }

    private void CloseCover()
    {
        if (boxCover1 != null && boxCover2 != null)
        {
            isBoxClose = true;
            Debug.Log("���� �Ѳ��� �������ϴ�!");
        }
    }

    public void SealBox()
    {
        isBoxSealed = true;
        Debug.Log($"{name} ���ڰ� ���������� ����Ǿ����ϴ�!");
    }

    public bool IsCoverClosed()
    {
        return boxCover1 != null && boxCover2 != null && isBoxClose;
    }
}