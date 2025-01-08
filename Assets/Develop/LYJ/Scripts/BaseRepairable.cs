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
        _repair.OnBrokenRaised.AddListener(Broken);
    }

    public virtual void Symptom()
    {
        _isBroken = false;
        MessageDisplayManager.Instance.ShowMessage($"{gameObject.name}: ���� ���� �߻�!");
    }

    public virtual void Broken()
    {
        if (_isSymptomSolved)
        {
            Debug.Log($"{gameObject.name}: ���� ������ �ذ�Ǿ����Ƿ� ������ �߻����� �ʽ��ϴ�.");
            return;
        }

        _isBroken = true;
        MessageDisplayManager.Instance.ShowMessage($"{gameObject.name}: ���� �߻�!");
    }

    public virtual void SolveSymptom()
    {
        _isBroken = false;
        _repair.IsSymptom = false;
        _repair.ResetRepairState();
        _isSymptomSolved = true;
        MessageDisplayManager.Instance.ShowMessage($"{gameObject.name}: ���� ������ �ذ�Ǿ����ϴ�!");
    }

    public virtual void SolveBroken()
    {
        _isBroken = false;
        _repair.ResetRepairState();
        MessageDisplayManager.Instance.ShowMessage($"{gameObject.name}: �����Ǿ����ϴ�!");
    }

    public virtual bool IsBroken()
    {
        return _isBroken && !_repair.IsRepaired;
    }
}
