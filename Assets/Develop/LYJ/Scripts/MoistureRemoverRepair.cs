using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoistureRemoverRepair : BaseRepairable
{
    [SerializeField] private XRKnobDial _moistureRemoverDial;
    [SerializeField] private ParticleSystem _symptomParticle;
    [SerializeField] private ParticleSystem _brokenParticle;

    protected override ParticleSystem SymptomParticle => _symptomParticle;
    protected override ParticleSystem BrokenParticle => _brokenParticle;

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

        _moistureRemoverDial = GetComponentInChildren<XRKnobDial>();
        if (_moistureRemoverDial == null)
        {
            Debug.LogError("MoistureRemoverDial�� �������� �ʽ��ϴ�.");
            return;
        }

        _moistureRemoverDial.onValueChange.AddListener(OnKnobValueChanged);
    }

    private void OnKnobValueChanged(float value)
    {
        if (_repair == null || !_repair.IsSymptom) // ���� ������ ������ ó������ ����
            return;

        // Knob ���� 1.0f�� �������� �� ���� ���� �ذ�
        if (Mathf.Approximately(value, 1.0f))
        {
            SolveSymptom();
        }
    }
}
