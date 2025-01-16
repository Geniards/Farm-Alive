using Fusion;
using GameData;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;
using static QuestManager;

public class BoxTrigger : MonoBehaviourPun
{
    [SerializeField] public GameObject boxTape;
    [SerializeField] public List<RequiredItem> requiredItems;
    [SerializeField] public BoxCover boxCover;
    [SerializeField] public List<int> idList = new List<int>();
    [SerializeField] Collider openCollider;
    [SerializeField] Collider closedCollider;

    [SerializeField] public delegate void OnRequiredItemsChanged(List<RequiredItem> items);
    [SerializeField] public event OnRequiredItemsChanged RequiredItemsChanged;


    private void Start()
    {
        boxCover = GetComponent<BoxCover>();
        boxCover.OnIsOpenChanged += NotifyRequiredItemsChanged;
    }

    private void OnEnable()
    {
        Debug.Log("����Ʈ");
        requiredItems = new List<RequiredItem>();
        NotifyRequiredItemsChanged();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tape"))
        {
            Debug.Log("�������");
            if (boxCover.IsOpen)
                return;

            Taping taping = other.GetComponent<Taping>();
            if (taping != null && !boxCover.IsPackaged)
            {
                Debug.Log("���������ν����غ�");
                taping.StartTaping(this, this.boxCover);
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
    }

    public void CountUpdate(int viewId, bool isBool)
    {
        if (!isBool)
        {
            PhotonView itemView = PhotonView.Find(viewId);
            idList.Add(viewId);

            Rigidbody itemRigid = itemView.GetComponent<Rigidbody>();
            itemRigid.drag = 10;
            itemRigid.angularDrag = 1;
            Crop cropView = itemView.GetComponent<Crop>();
            if (requiredItems.Count > 0)
            {
                foreach (QuestManager.RequiredItem item in requiredItems)
                {
                    Debug.Log(requiredItems);
                    if (item.itemPrefab.name == itemView.gameObject.name)
                    {
                        item.requiredcount += cropView.Value;
                        Debug.Log("ī��Ʈ��");
                        NotifyRequiredItemsChanged();
                        return;
                    }
                }
                requiredItems.Add(new RequiredItem(itemView.gameObject, cropView.Value));
            }
            else
            {
                requiredItems.Add(new RequiredItem(itemView.gameObject, cropView.Value));
            }

            NotifyRequiredItemsChanged();
        }
        else
        {
            PhotonView itemView = PhotonView.Find(viewId);

            Crop cropView = itemView.GetComponent<Crop>();
            if (requiredItems.Count > 0)
            {
                for (int i = requiredItems.Count - 1; i >= 0; i--)
                {
                    if (requiredItems[i].itemPrefab.name == itemView.gameObject.name)
                    {
                        requiredItems[i].requiredcount -= cropView.Value;

                        Rigidbody itemRigid = itemView.GetComponent<Rigidbody>();
                        itemRigid.drag = 0;
                        itemRigid.angularDrag = 0.05f;

                        if (requiredItems[i].requiredcount <= 0)
                        {
                            if (idList.Contains(itemView.ViewID))
                            {
                                idList.Remove(itemView.ViewID);
                                Debug.Log($"����Ʈ���� {itemView} ����");
                            }
                            requiredItems.RemoveAt(i);
                        }

                        NotifyRequiredItemsChanged();
                        return;
                    }
                }
            }
        }
    }

    public void CompleteTaping()
    {
        photonView.RPC(nameof(SyncTaping), RpcTarget.All);
    }

    [PunRPC]
    private void SyncTaping()
    {
        boxCover.IsPackaged = true;
        boxCover.tape.SetActive(true);        

        Debug.Log($"������ �Ϸ�: {this.name}");
    }

    private void NotifyRequiredItemsChanged()
    {
        RequiredItemsChanged?.Invoke(requiredItems);
    }

    public void ClearBox()
    {
        if (PhotonNetwork.IsMasterClient)
            StartCoroutine(ClearBoxList());
    }

    private IEnumerator ClearBoxList()
    {
        bool isClear = true;
        while (isClear)
        {
            yield return null;

            for (int i = idList.Count - 1; i >= 0; i--)
            {
                PhotonView cropView = PhotonView.Find(idList[i]);
                PhotonNetwork.Destroy(cropView.gameObject);
            }

            isClear = false;
        }

        PhotonNetwork.Destroy(gameObject);
    }
}