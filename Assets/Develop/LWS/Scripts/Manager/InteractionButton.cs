using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using UnityEngine;

public class InteractionButton : MonoBehaviour
{
    /// <summary>
    /// ���� �ִ� ��ư�� ����ϱ� ���� �ʵ� �� �޼���
    /// </summary>
    [Header("�� �ֱ�")]
    private bool _isWatering = false;

    public void Watering()
    {
        if (_isWatering) return;

        SectionManager.Instance.IncreaseMoisture();
    }


    /// <summary>
    /// ��Ḧ �ִ� ��ư�� ����ϱ� ���� �ʵ� �� �޼���
    /// </summary>
    private bool _isNutrienting = false;

    public void Nutrienting()
    {
        if (_isNutrienting) return;

        SectionManager.Instance.IncreaseNutrient();
    }

    //TODO : �̺�Ʈ �߰� �� �������� ��ư �� ���� ���� ��ư ����
}
