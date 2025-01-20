using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LiquidContainer : MonoBehaviourPunCallbacks, IPunObservable
{
    public UnityEvent<float> OnGaugeUpdated;

    [Tooltip("�ʱ⿡ �� ���� ���� (0.0���� ������ �⺻)")]
    [SerializeField] float _fillAmount = 0f;

    public float FillAmount { get { return _fillAmount; } set { _fillAmount = value; OnGaugeUpdated?.Invoke(value); } }
    public float MaxAmount { get { return _maxAmount; } }

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

        FillAmount += amount;
        FillAmount = MathF.Min(FillAmount, _maxAmount);

        StartCoroutine(LiquidCheck());
    }
    public IEnumerator LiquidCheck()
    {
        Debug.Log($"���� �����̳� ���� ��ü��{FillAmount}");

        yield return new WaitForSeconds(5f);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // ���� Owner���, _fillAmount�� �ٸ� Ŭ���̾�Ʈ���� ����
            stream.SendNext(FillAmount);
        }
        else
        {
            // �� ������ �ƴ϶��, Owner�κ��� _fillAmount�� ����
            FillAmount = (float)stream.ReceiveNext();
        }
    }
}
