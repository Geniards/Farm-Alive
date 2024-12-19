using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlantGround : MonoBehaviour
{
    [SerializeField] private int _digCount; // �ʿ� ���� Ƚ��
    private int _currentDigCount = 0; // ���� ���� Ƚ��
    private bool _isInteractable = true; // ��ȣ�ۿ� ���� ����
    private XRSocketInteractor _socketInteractor;

    private void Awake()
    {
        _socketInteractor = GetComponentInChildren<XRSocketInteractor>();

        if (_socketInteractor != null)
        {
            _socketInteractor.enabled = false;
        }
        else
        {
            Debug.Log("XRSocketInteractor�� ã�� �� �����ϴ�.");
        }
    }

    /// <summary>
    /// ���� ���� �޼ҵ�
    /// </summary>
    public void Dig()
    {
        if (!_isInteractable) return;

        _currentDigCount++;
        Debug.Log($"���� Ƚ��: {_currentDigCount} / {_digCount}");

        if (_currentDigCount >= _digCount)
        {
            GameObject disappearGround = GameObject.FindWithTag("DisappearingGround");
            if (disappearGround != null)
            {
                Destroy(disappearGround);
                Debug.Log("DisappearingGround�� �����Ǿ����ϴ�.");
            }

            if (_socketInteractor != null)
            {
                _socketInteractor.enabled = true;
            }


            _isInteractable = false;
        }
    }
}
