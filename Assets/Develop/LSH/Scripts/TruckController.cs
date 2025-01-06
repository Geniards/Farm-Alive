using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckController : MonoBehaviour
{
    [SerializeField] GameObject truckPrefab;

    [SerializeField] Transform[] transformCounts;
    [SerializeField] public GameObject[] truckObjects = new GameObject[4];

    public void ClearSlot(int truckId)
    {
        truckObjects[truckId] = null;

        for (int i = truckId; i < truckObjects.Length - 1; i++)
        {
            if (truckObjects[i + 1] == null)
                return;
            truckObjects[i] = truckObjects[i + 1]; // ���� ������ ���� �������� �̵�
            truckObjects[i + 1] = null; // ������ ������ null�� ����
            truckObjects[i].GetComponent<TruckQuest>().ChangeID(i);
        }
    }

    public void CreateTruck()
    {
        for (int i = 0; i < truckObjects.Length; i++)
        {
            if (truckObjects[i] == null)
            {
                GameObject truck = PhotonNetwork.Instantiate(truckPrefab.name, transformCounts[i].position, Quaternion.identity);
                TruckQuest truckQuest = truck.GetComponent<TruckQuest>();
                QuestManager.Instance.truckList.Add(truckQuest);

                truckQuest.SetID(i, this);

                truckObjects[i] = truck;
                break;
            }
        }
    }
}