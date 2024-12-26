using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BoxTrigger : MonoBehaviourPun
{
    public GameObject boxCover;
    private bool isBoxSealed = false;

    private void OnTriggerEnter(Collider other)
    {      
        
        if (other.CompareTag("Item"))
        {
            PhotonView itemView = other.GetComponent<PhotonView>();
            if (itemView == null || !itemView.IsMine)
                return;

            XRGrabInteractable grabInteractable = other.GetComponent<XRGrabInteractable>();
            if (grabInteractable != null && !grabInteractable.isSelected)
                return;

            if (QuestManager.Instance.currentQuest == null)
                return;

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

            QuestManager.Instance.UpdateCount();
            Destroy(other.gameObject);

            if (QuestManager.Instance.IsQuestComplete())
            {
                CloseCover();
            }
        }
            
        else if (isBoxSealed && other.CompareTag("Tape"))
        {
            Debug.Log("�������");

            Taping taping = other.GetComponent<Taping>();
            if (taping != null && !isBoxSealed)
            {
                taping.StartTaping(this);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Tape")) return;

        Taping taping = other.GetComponent<Taping>();
        if (taping != null)
        {
            taping.StopTaping();
        }
    }

    private void CloseCover()
    {
        if (boxCover != null)
        {
            boxCover.SetActive(true);
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
        return boxCover != null && boxCover.activeSelf;
    }
}