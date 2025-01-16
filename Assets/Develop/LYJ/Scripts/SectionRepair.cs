using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class SectionRepair : BaseRepairable
{
    [SerializeField] private XRLever _sectionLever;
    [SerializeField] private SectionMover _sectionMover;
    [SerializeField] private ParticleSystem _symptomParticle;
    [SerializeField] private ParticleSystem _brokenParticle;

    protected override ParticleSystem SymptomParticle => _symptomParticle;
    protected override ParticleSystem BrokenParticle => _brokenParticle;

    protected override string SymptomSoundKey => "SFX_Machine_Error";
    protected override string BrokenSoundKey => "SFX_Equipment_Problem";

    protected override void Start()
    {
        base.Start();

        if (_symptomParticle == null)
            _symptomParticle = transform.Find("SymptomParticle")?.GetComponentInChildren<ParticleSystem>(true);

        if (_brokenParticle == null)
            _brokenParticle = transform.Find("BrokenParticle")?.GetComponentInChildren<ParticleSystem>(true);

        if (_symptomParticle == null)
            Debug.LogWarning($"{gameObject.name}: 'SymptomParticle' ��ƼŬ�� ã�� �� �����ϴ�.");

        if (_brokenParticle == null)
            Debug.LogWarning($"{gameObject.name}: 'BrokenParticle' ��ƼŬ�� ã�� �� �����ϴ�.");

        _sectionLever = GetComponentInChildren<XRLever>();
        if (_sectionLever == null)
        {
            Debug.LogError("SectionLever�� �������� �ʽ��ϴ�.");
            return;
        }

        _sectionMover = GetComponentInChildren<SectionMover>();
        if ( _sectionMover == null)
        {
            Debug.Log("SectionMover�� �������� �ʽ��ϴ�.");
            return;
        }

        _sectionLever.onLeverActivate.AddListener(OnLeverActivated);
    }

    // ������ ������ ���� ������ �ذ�
    private void OnLeverActivated()
    {
        if (_repair == null || !_repair.IsSymptom) // ���� ������ ������ ó������ ����
            return;

        SolveSymptom();
    }

    public override bool Broken()
    {
        // �̹� ������ �߻��� ���¶�� �ƹ� �۾��� ���� ����
        if (_sectionMover.isBroken)
            return false;

        bool isBroken = base.Broken();

        if (isBroken && _sectionMover != null)
        {
            _sectionMover.DisableMovement(); // ���� ������ ��Ȱ��ȭ
        }

        return isBroken;
    }

    public override void SolveBroken()
    {
        base.SolveBroken();

        if (_sectionMover != null)
        {
            _sectionMover.EnableMovement(); // ���� ���� �� ������ ������ ���
        }
    }
}
