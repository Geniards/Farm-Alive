using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRepairableObject : MonoBehaviour, IRepairable
{
    private int _repairCount = 0;
    public void Repair()
    {
        _repairCount++;
        Debug.Log($"���� Ƚ��: {_repairCount}");

        // TODO : ���� Ƚ���� ���� ������ ���� �ۼ�
    }
}
