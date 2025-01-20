using Photon.Pun;
using UnityEngine;

public abstract class BaseRepairable : MonoBehaviour, IRepairable
{
    protected Repair _repair;
    protected bool _isBroken;
    private bool _isSymptomSolved = false; // ���� ���� �ذ� ����

    protected virtual ParticleSystem SymptomParticle { get; }
    protected virtual ParticleSystem BrokenParticle { get; }

    protected virtual string SymptomSoundKey => null; // ���� ���� ���� Ű ��
    protected virtual string BrokenSoundKey => null; // ���� ���� Ű ��

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
        SymptomParticle?.Play(); // ���� ���� ��ƼŬ ���

        // ���� ���� ���� ���
        if (!string.IsNullOrEmpty(SymptomSoundKey))
        {
            //Debug.Log("���� ���� ���� ���");
            SoundManager.Instance.PlaySFXLoop(SymptomSoundKey, 0.5f);
        }
        MessageDisplayManager.Instance.ShowMessage($"���� ���� �߻�! �׺��� Ȯ�����ּ���!, 5f");
    }

    public virtual bool Broken() // ��ȯ�� �߰�
    {
        if (_isSymptomSolved)
        {
            //Debug.Log($"���� ������ �ذ�Ǿ����Ƿ� ������ �߻����� �ʽ��ϴ�.");
            return false; // ���� �߻����� ����
        }

        _isBroken = true;
        SymptomParticle?.Stop();
        BrokenParticle?.Play(); // ���� ��ƼŬ ���

        // ���� ���� ��� �� ���� ���� ���� ����
        if (!string.IsNullOrEmpty(BrokenSoundKey))
        {
            //Debug.Log("���� ���� ���� ���� �� ���� ���� ���");
            SoundManager.Instance.StopSFXLoop(SymptomSoundKey);
            SoundManager.Instance.PlaySFXLoop(BrokenSoundKey, 0.5f);
        }

        MessageDisplayManager.Instance.ShowMessage($"���� �߻�! ��ġ�� �������ּ���!, 5f");
        //Debug.LogError($"{gameObject.name}: ���� �߻�!");
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
            return;
        }

        SymptomParticle?.Stop();
        _repair.IsSymptom = false;
        _isSymptomSolved = true;
        _repair.ResetRepairState();

        // ���� ���� ���� ����
        if (!string.IsNullOrEmpty(SymptomSoundKey))
        {
            SoundManager.Instance.StopSFXLoop(SymptomSoundKey);
        }
        MessageDisplayManager.Instance.ShowMessage($"���� ������ �ذ�Ǿ����ϴ�!, 5f");
        //Debug.LogError($"{gameObject.name}: ���� ������ �ذ�Ǿ����ϴ�!");
    }

    public virtual void SolveBroken()
    {
        _isBroken = false;
        BrokenParticle?.Stop();
        _repair.ResetRepairState();

        // ���� ���� ����
        if (!string.IsNullOrEmpty(BrokenSoundKey))
        {
            SoundManager.Instance.StopSFXLoop(BrokenSoundKey);
        }
        //MessageDisplayManager.Instance.ShowMessage($"{gameObject.name}: �����Ǿ����ϴ�!");
        //Debug.LogError($"{gameObject.name}: �����Ǿ����ϴ�!");
    }

    public virtual bool IsBroken()
    {
        return _isBroken && !_repair.IsRepaired;
    }
}
