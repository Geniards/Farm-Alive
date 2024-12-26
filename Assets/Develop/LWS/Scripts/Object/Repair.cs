using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repair : MonoBehaviourPunCallbacks
{
    [Tooltip("�� �� ������ ����������")]
    [SerializeField] int _maxRepairCount;
    private int _curRepairCount = 0;

    /// <summary>
    /// ������ �Ϸ�Ǿ����� ���θ� ��Ÿ���� �Ӽ�
    /// </summary>
    public bool IsRepaired { get; private set; } = false;

    [PunRPC]
    public void RPC_PlusRepairCount()
    {
        if (!enabled)
        {
            return;
        }

        if (!PhotonNetwork.IsMasterClient) return;

        _curRepairCount++;
        Debug.Log($"������: {_curRepairCount}/{_maxRepairCount}");
        if (_curRepairCount >= _maxRepairCount)
        {
            // ���� �Ϸ�
            Debug.Log($"[{photonView.ViewID}] ���� �Ϸ�!");
            IsRepaired = true; //�����⿡�� ��ġ�� 1�� ������ �Ǿ��ٴ� �� �˾ƾ� 2�� ���� (�� + �õ���)�� �� �� ����
            enabled = false; //���� �Ϸ�� �� ��ũ��Ʈ ��Ȱ��ȭ �ϸ� ��� ������ _maxRepairCount ���Ŀ� �۵� X
            // TODO: ���� �Ϸ� ���� (ex: ������Ʈ �ı�, �ִϸ��̼�, ���� ��ȯ ��)
        }
    }
}
