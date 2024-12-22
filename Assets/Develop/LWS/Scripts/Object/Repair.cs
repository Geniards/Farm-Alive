using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repair : MonoBehaviourPunCallbacks
{
    [Tooltip("�� �� ������ ����������")]
    [SerializeField] int _maxRepairCount;
    private int _curRepairCount = 0;

    [PunRPC]
    public void RPC_PlusRepairCount()
    { 
        if (!PhotonNetwork.IsMasterClient) return;

        _curRepairCount++;
        if (_curRepairCount >= _maxRepairCount)
        {
            // ���� �Ϸ�
            Debug.Log($"[{photonView.ViewID}] ���� �Ϸ�!");
            // TODO: ���� �Ϸ� ���� (ex: ������Ʈ �ı�, �ִϸ��̼�, ���� ��ȯ ��)
        }
    }
}
