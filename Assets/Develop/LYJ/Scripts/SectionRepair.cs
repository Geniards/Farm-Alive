using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class SectionRepair : BaseRepairable
{
    [SerializeField] private XRLever _sectionLever;
    [SerializeField] private SectionMover _sectionMover;

    protected override void Start()
    {
        base.Start();

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
