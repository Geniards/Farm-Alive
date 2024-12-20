using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantDigCount : MonoBehaviour
{
    [SerializeField] private int _plantDigCount; // �Ĺ��� �ɾ��� ���� �ʿ��� ���� Ƚ��

    /// <summary>
    /// �� �Ĺ��� ���� �ɾ��� �� �ִ��� groundDigCount �� Ȯ��
    /// </summary>
    public bool CanPlant(int groundDigCount)
    {
        // ���� ���� Ƚ���� �Ĺ��� �ʿ� ���� Ƚ���� ��
        return groundDigCount == _plantDigCount;
    }
}
