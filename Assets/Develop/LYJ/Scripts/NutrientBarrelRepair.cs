using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NutrientBarrelRepair : MonoBehaviour
{
    [SerializeField] private Repair _repair;
    [SerializeField] private DialInteractable _nutrientDial;
    [SerializeField] private bool _isBroken;

    private void Start()
    {
        _repair = GetComponent<Repair>();

        if (_repair == null)
        {
            Debug.LogError("Repair ������Ʈ�� ã�� �� �����ϴ�.");
            return;
        }

        _repair.OnSymptomRaised.AddListener(Symptom);
        _repair.OnBrokenRaised.AddListener(Broken);

        _nutrientDial = GetComponentInChildren<DialInteractable>();

        if (_nutrientDial == null)
        {
            Debug.LogError("NutrientDial�� �������� �ʽ��ϴ�.");
        }

        _nutrientDial._onAngleChanged.AddListener(OnDialAngleChanged);
    }

    // ���̾� ���� ���� �̺�Ʈ ó��
    private void OnDialAngleChanged(float angle)
    {
        if (_repair == null || !_repair.IsSymptom) // ���� ������ ������ ó������ ����
            return;

        if (Mathf.Approximately(angle, _nutrientDial.maxAngle)) // ���̾��� ������ ȸ������ ��
        {
            SolveSymptom();
        }
    }

    // ���� ���� �߻� ó��
    public void Symptom()
    {
        _isBroken = false;
        MessageDisplayManager.Instance.ShowMessage("��������ġ �������� �߻�!");
    }

    // ���� �߻� ó��
    public void Broken()
    {
        _isBroken = true;
        MessageDisplayManager.Instance.ShowMessage("��������ġ�� ���峵���ϴ�!");
    }

    // ���� ���� Ȯ�� (���� ����� ���� ���¸� ����) -> ���� ���� ���� ��ȯ
    public bool IsBroken()
    {
        return _isBroken && !_repair.IsRepaired;
    }

    // ���� ���� �ذ� ó��
    public void SolveSymptom()
    {
        _isBroken = false;
        _repair.IsSymptom = false;
        _repair.ResetRepairState();
        MessageDisplayManager.Instance.ShowMessage("���� ������ �ذ�Ǿ����ϴ�!");
    }

    // ���� ���� ó��
    public void SolveBroken()
    {
        _isBroken = false;
        _repair.ResetRepairState();
        MessageDisplayManager.Instance.ShowMessage("�����Ǿ����ϴ�!");
    }
}
