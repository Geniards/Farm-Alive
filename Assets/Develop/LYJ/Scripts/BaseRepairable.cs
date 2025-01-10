using Photon.Pun;
using UnityEngine;

public abstract class BaseRepairable : MonoBehaviour, IRepairable
{
    protected Repair _repair;
    protected bool _isBroken;
    private bool _isSymptomSolved = false; // ���� ���� �ذ� ����

    protected virtual void Start()
    {
        _repair = GetComponent<Repair>();
        if (_repair == null)
        {
            Debug.LogError($"{gameObject.name}: Repair ������Ʈ�� ã�� �� �����ϴ�.");
            return;
        }

        _repair.OnSymptomRaised.AddListener(Symptom);
        _repair.OnBrokenRaised.AddListener(HandleBroken);
        _repair.OnBrokenSolved.AddListener(SolveBroken);
        _repair.OnSymptomSolved.AddListener(SolveSymptom);
    }

    public virtual void Symptom()
    {
        _isBroken = false;
        _isSymptomSolved = false; // ���� ���� �ذ� ���� �ʱ�ȭ
        MessageDisplayManager.Instance.ShowMessage($"{gameObject.name}: ���� ���� �߻�!");
    }

    public virtual bool Broken() // ��ȯ�� �߰�
    {
        if (_isSymptomSolved)
        {
            Debug.Log($"{gameObject.name}: ���� ������ �ذ�Ǿ����Ƿ� ������ �߻����� �ʽ��ϴ�.");
            return false; // ���� �߻����� ����
        }

        _isBroken = true;
        MessageDisplayManager.Instance.ShowMessage($"{gameObject.name}: ���� �߻�!");
        Debug.LogError($"{gameObject.name}: ���� �߻�!");
        return true; // ������ �߻���
    }

    public void HandleBroken()
    {
        Broken();
    }

    public virtual void SolveSymptom()
    {
        // ���� ���¿����� ���� ���� �ذ� �Ұ�
        if (_isBroken)
        {
            MessageDisplayManager.Instance.ShowMessage($"{gameObject.name}: �̹� ���峭 ���¿����� ���� ������ �ذ��� �� �����ϴ�.");
            return;
        }

        _repair.IsSymptom = false;
        _isSymptomSolved = true;
        _repair.ResetRepairState();
        MessageDisplayManager.Instance.ShowMessage($"{gameObject.name}: ���� ������ �ذ�Ǿ����ϴ�!");
        Debug.LogError($"{gameObject.name}: ���� ������ �ذ�Ǿ����ϴ�!");
    }

    public virtual void SolveBroken()
    {
        _isBroken = false;
        _repair.ResetRepairState();
        MessageDisplayManager.Instance.ShowMessage($"{gameObject.name}: �����Ǿ����ϴ�!");
        Debug.Log("�����Ϸ�");
    }

    public virtual bool IsBroken()
    {
        return _isBroken && !_repair.IsRepaired;
    }
}
