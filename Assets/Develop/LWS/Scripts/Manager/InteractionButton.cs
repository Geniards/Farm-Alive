using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionButton : MonoBehaviour
{
    private void Start()
    {
        if (_waterBarrelRepair == null)
        {
            _waterBarrelRepair = FindObjectOfType<WaterBarrelRepair>();
            if (_waterBarrelRepair == null)
            {
                Debug.Log("WaterBarrelRepair ������Ʈ�� ã�� �� �����ϴ�!");
            }
        }

        if (_nutrientBarrelRepair == null)
        {
            _nutrientBarrelRepair = FindObjectOfType<NutrientBarrelRepair>();
            if (_nutrientBarrelRepair == null)
            {
                Debug.Log("NutrientBarrelRepair ������Ʈ�� ã�� �� �����ϴ�!");
            }
        }

        if (_sprayingPesticideRepair == null)
        {
            _sprayingPesticideRepair = FindObjectOfType<PesticideRepair>();
            if (_sprayingPesticideRepair == null)
            {
                Debug.Log("PesticideRepair ������Ʈ�� ã�� �� �����ϴ�!");
            }
        }

        if (_moistureRemoverRepair == null)
        {
            _moistureRemoverRepair = FindObjectOfType<MoistureRemoverRepair>();
            if (_moistureRemoverRepair == null)
            {
                Debug.Log("MoistureRemoverRepair ������Ʈ�� ã�� �� �����ϴ�");
            }
        }
    }

    /// <summary>
    /// ���� �ִ� ��ư�� ����ϱ� ���� �ʵ� �� �޼���
    /// </summary>
    [Header("�� �ֱ�")]
    private bool _isWatering = false;

    [SerializeField] private WaterBarrelRepair _waterBarrelRepair;

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
            MessageDisplayManager.Instance.ShowMessage("������ ���� �ؾ� ���� �� �� �ֽ��ϴ�!");
            return;
        }

        SectionManager.Instance.IncreaseMoisture(); // �� �ֱ� ����
    }


    /// <summary>
    /// ��Ḧ �ִ� ��ư�� ����ϱ� ���� �ʵ� �� �޼���
    /// </summary>
    private bool _isNutrienting = false;

    [SerializeField] private NutrientBarrelRepair _nutrientBarrelRepair;

    public void Nutrienting()
    {
        if (_isNutrienting) return;

        if (_nutrientBarrelRepair == null)
        {
            Debug.LogError("NutrientBarrelRepair�� ������� �ʾҽ��ϴ�!");
            return;
        }

        if (_nutrientBarrelRepair.IsBroken())
        {
            MessageDisplayManager.Instance.ShowMessage("������ ���� �ؾ� ��Ḧ �� �� �ֽ��ϴ�!");
            return;
        }

        SectionManager.Instance.IncreaseNutrient();
    }
    
    /// <summary>
    /// ������ �Ѹ��� ��ư�� ����ϱ� ���� �ʵ� �� �޼���
    /// </summary>
    private bool _isSprayingPesticide = false;

    [SerializeField] private PesticideRepair _sprayingPesticideRepair;
   
    public void SprayingPesticide()
    {
        if (_isSprayingPesticide) return;

        if (_sprayingPesticideRepair == null)
        {
            Debug.LogError("PesticideRepair�� ������� �ʾҽ��ϴ�.");
            return;
        }

        if (_sprayingPesticideRepair.IsBroken())
        {
            MessageDisplayManager.Instance.ShowMessage("������ ���� �ؾ� �������� �� �� �ֽ��ϴ�!");
            return;
        }

        SectionManager.Instance.SprayPesticide();
    }

    private bool _isSprayingMoistureRemover = false;

    [SerializeField] private MoistureRemoverRepair _moistureRemoverRepair;

    public void SprayingMoisture()
    {
        if (_isSprayingMoistureRemover) return;

        if (_moistureRemoverRepair == null)
        {
            Debug.LogError("PesticideRepair�� ������� �ʾҽ��ϴ�.");
            return;
        }

        if (_moistureRemoverRepair.IsBroken())
        {
            MessageDisplayManager.Instance.ShowMessage("������ ���� �ؾ� ������������ �� �� �ֽ��ϴ�!");
            return;
        }

        SectionManager.Instance.DecreaseMoisture();
    }


}
