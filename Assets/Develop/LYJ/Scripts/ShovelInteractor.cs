using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShovelInteractor : MonoBehaviour
{
    private bool _isShovelTouchingGround = false;

    private void OnTriggerEnter(Collider other)
    {
        // ���� DisappearGround�� ������
        if (other.CompareTag("DisappearGround"))
        {
            _isShovelTouchingGround = true; // ���� ���� ����
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // ���� DisappearGround���� ��������
        if (other.CompareTag("DisappearGround"))
        {
            if (_isShovelTouchingGround)
            {
                Destroy(other.gameObject); // DisappearGround ������Ʈ ����
                _isShovelTouchingGround = false; // ���� �ʱ�ȭ
            }
        }
    }
}
