using Fusion;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;
using static QuestManager;

public class BoxTrigger : MonoBehaviourPun//, IPunObservable
{
    [SerializeField] public GameObject boxTape;
    [SerializeField] public List<RequiredItem> requiredItems;
    [SerializeField] bool isBoxClose = false;
    [SerializeField] bool isBoxSealed = false;
    [SerializeField] bool isFirstItem = false;
    [SerializeField] BoxCover boxCover;

    private void Start()
    {
        boxCover = GetComponent<BoxCover>();
    }

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

            photonView.RPC(nameof(UpCount), RpcTarget.All, itemView.ViewID);
            Debug.Log("����");
        }

        else if (!isBoxSealed && other.CompareTag("Tape"))
        {
            Debug.Log("�������");

            Taping taping = other.GetComponent<Taping>();
            if (taping != null && !isBoxSealed)
            {
                Debug.Log("���������ν����غ�");
                taping.StartTaping(boxCover);
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

            PhotonView itemView = other.GetComponent<PhotonView>();

            photonView.RPC(nameof(DownCount), RpcTarget.All, itemView.ViewID);
        }
    }

    [PunRPC]
    private void UpCount(int viewId)
    {
        PhotonView itemView = PhotonView.Find(viewId);
        if (isFirstItem)
        {
            foreach (QuestManager.RequiredItem item in requiredItems)
            {
                Debug.Log(requiredItems);
                if (item.itemPrefab.name == itemView.gameObject.name)
                {
                    item.requiredcount++;
                    Debug.Log("ī��Ʈ��");
                }
                else
                {
                    requiredItems.Add(new RequiredItem(itemView.gameObject, 1));
                    Debug.Log("�߰�");
                }
            }
        }

        if (!isFirstItem)
        {
            requiredItems.Add(new RequiredItem(itemView.gameObject, 1));
            isFirstItem = true;
        }
    }

    [PunRPC]
    private void DownCount(int viewId)
    {
        PhotonView itemView = PhotonView.Find(viewId);
        if (requiredItems.Count > 0)
        {
            for (int i = requiredItems.Count - 1; i >= 0; i--)
            {
                if (requiredItems[i].itemPrefab.name == itemView.gameObject.name)
                {
                    requiredItems[i].requiredcount--;

                    if (requiredItems[i].requiredcount == 0)
                    {
                        requiredItems.RemoveAt(i);
                        isFirstItem = false;
                    }
                }
            }
        }
    }
}