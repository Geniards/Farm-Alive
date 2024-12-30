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
    public bool IsRepaired { get; set; } = false;

    [PunRPC]
    public void RPC_PlusRepairCount()
    {
        if (!enabled)
        {
            return;
        }

        if (!PhotonNetwork.IsMasterClient) return;

        _curRepairCount++;
        MessageDisplayManager.Instance.ShowMessage($"������: {_curRepairCount}/{_maxRepairCount}");
        //Debug.Log($"������: {_curRepairCount}/{_maxRepairCount}");
        if (_curRepairCount >= _maxRepairCount)
        {
            // ���� �Ϸ�
            MessageDisplayManager.Instance.ShowMessage("�����Ϸ�!");
            //Debug.Log($"[{photonView.ViewID}] ���� �Ϸ�!");
            IsRepaired = true; //�����⿡�� ��ġ�� 1�� ������ �Ǿ��ٴ� �� �˾ƾ� 2�� ���� (�� + �õ���)�� �� �� ����
            enabled = false; //���� �Ϸ�� �� ��ũ��Ʈ ��Ȱ��ȭ �ϸ� ��� ������ _maxRepairCount ���Ŀ� �۵� X
            // TODO: ���� �Ϸ� ���� (ex: ������Ʈ �ı�, �ִϸ��̼�, ���� ��ȯ ��)
        }
    }

    /// <summary>
    /// ���� ���¸� �ʱ�ȭ (���� Ƚ�� �ʱ�ȭ)
    /// </summary>
    public void ResetRepairState()
    {
        Debug.Log($"���� ���� �ʱ�ȭ: _curRepairCount={_curRepairCount}, IsRepaired={IsRepaired}");
        _curRepairCount = 0;
        IsRepaired = false;  // ���� �Ϸ� ���� �ʱ�ȭ
        enabled = true;      // ��ũ��Ʈ �ٽ� Ȱ��ȭ
    }
}
