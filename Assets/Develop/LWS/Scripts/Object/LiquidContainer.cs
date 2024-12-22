using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LiquidContainer : MonoBehaviourPunCallbacks, IPunObservable
{
    [Tooltip("�ʱ⿡ �� ���� ���� (0.0���� ������ �⺻)")]
    [SerializeField] float _fillAmount = 0.0f;
    float _maxAmount = 1.0f;

    /// <summary>
    /// �ܺο��� ���� ä���� ȣ���� �Լ�.
    /// amount�� 1�� ���� á�ٰ� �������� ���� �����Դϴ�.
    /// </summary>
    /// <param name="amount"></param>
    public void ReceiveLiquid(float amount)
    {
        if (!photonView.IsMine)
            return;

        _fillAmount += amount;
        _fillAmount = MathF.Min(_fillAmount, _maxAmount);

        StartCoroutine(LiquidCheck());
    }
    public IEnumerator LiquidCheck()
    {
        Debug.Log($"���� �����̳� ���� ��ü��{_fillAmount}");

        yield return new WaitForSeconds(5f);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // ���� Owner���, _fillAmount�� �ٸ� Ŭ���̾�Ʈ���� ����
            stream.SendNext(_fillAmount);
        }
        else
        {
            // �� ������ �ƴ϶��, Owner�κ��� _fillAmount�� ����
            _fillAmount = (float)stream.ReceiveNext();
        }
    }
}
