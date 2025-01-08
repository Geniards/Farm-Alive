using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBarrelRepair : MonoBehaviour
{
    [SerializeField] private Repair _repair;
    [SerializeField] private DialInteractable _waterDial;
    [SerializeField] private bool _isBroken;

    private void Start()
    {
        _repair = GetComponent<Repair>();

        if (_repair == null )
        {
            Debug.LogError("Repair ������Ʈ�� ã�� �� �����ϴ�.");
            return;
        }

        _repair.OnSymptomRaised.AddListener(Symptom);
        _repair.OnBrokenRaised.AddListener(Broken);

        _waterDial = GetComponentInChildren<DialInteractable>();

        if (_waterDial == null)
        {
            Debug.LogError("WaterDial�� �������� �ʽ��ϴ�.");
        }

        _waterDial._onAngleChanged.AddListener(OnDialAngleChanged);
    }

    // ���̾� ���� ���� �̺�Ʈ ó��
    private void OnDialAngleChanged(float angle)
    {
        if (_repair == null || !_repair.IsSymptom) // ���� ������ ������ ó������ ����
            return;

        if (Mathf.Approximately(angle, _waterDial.maxAngle)) // ���̾��� ������ ȸ������ ��
        {
            SolveSymptom();
        }
    }

    // ���� ���� �߻� ó��
    public void Symptom()
    {
        _isBroken = false;
        MessageDisplayManager.Instance.ShowMessage("�޼���ġ �������� �߻�!");
    }

    // ���� �߻� ó��
    public void Broken()
    {
        if (_isBroken == false)
        {
            Debug.Log("���� ������ �ذ�Ǿ����Ƿ� ������ �߻����� �ʽ��ϴ�.");
            return;
        }

        _isBroken = true;
        MessageDisplayManager.Instance.ShowMessage("�޼���ġ�� ���峵���ϴ�!");
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
