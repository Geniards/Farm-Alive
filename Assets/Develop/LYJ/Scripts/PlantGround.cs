using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantGround : MonoBehaviour
{
    [SerializeField] private int _digCount; // �ʿ� ���� Ƚ��
    private int _currentDigCount = 0; // ���� ���� Ƚ��

    /// <summary>
    /// ���� ���� �޼ҵ�
    /// </summary>
    public void Dig()
    {
        _currentDigCount++;
        Debug.Log($"���� Ƚ��: {_currentDigCount} / {_digCount}");

        if (_currentDigCount >= _digCount)
        {
            GameObject disappearGround = GameObject.FindWithTag("DisappearGround");
            if (disappearGround != null)
            {
                Destroy(disappearGround);
                Debug.Log("DisappearGround�� �����Ǿ����ϴ�.");
            }
        }
    }
}
