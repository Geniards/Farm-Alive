using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUpgradableObject : MonoBehaviour, IUpgradable
{
    private int _upgradeCount = 0;
    public void Upgrade()
    {
        _upgradeCount++;
        Debug.Log($"���׷��̵� Ƚ�� : {_upgradeCount}");

        // TODO : ���׷��̵� Ƚ���� ���� ������ ���� �ۼ�
    }
}
