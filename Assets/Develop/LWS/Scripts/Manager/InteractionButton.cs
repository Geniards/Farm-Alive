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

    [SerializeField] private WaterBarrelRepair _waterBarrelRepair;

    private void Start()
    {
        if (_waterBarrelRepair == null)
        {
            _waterBarrelRepair = FindObjectOfType<WaterBarrelRepair>();
            if (_waterBarrelRepair == null)
            {
                Debug.LogError("WaterBarrelRepair ������Ʈ�� ã�� �� �����ϴ�!");
            }
        }
    }

    public void Watering()
    {
        if (_isWatering) return;

        if (_waterBarrelRepair == null)
        {
            Debug.LogError("WaterBarrelRepair�� ������� �ʾҽ��ϴ�!");
            return;
        }

        // ���� ���� Ȯ��
        if (_waterBarrelRepair.IsBroken()) // ���� ������ ���
        {
            MessageDisplayManager.Instance.ShowMessage("������ ���� �����ؾ� ���� �� �� �ֽ��ϴ�!");
            return;
        }

        SectionManager.Instance.IncreaseMoisture(); // �� �ֱ� ����
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
