using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LiquidContainer : MonoBehaviour
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
        _fillAmount += amount;
        _fillAmount = MathF.Min(_fillAmount, _maxAmount);
        StartCoroutine(LiquidCheck());
    }
    public IEnumerator LiquidCheck()
    {
        Debug.Log($"���� �����̳� ���� ��ü��{_fillAmount}");

        yield return new WaitForSeconds(5f);
    }
}
