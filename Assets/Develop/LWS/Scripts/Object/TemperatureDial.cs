using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class TemperatureDial : BaseRepairable
{
    [SerializeField] private XRKnobDial _temperatureDial;
    [SerializeField] private ParticleSystem _symptomParticle;
    [SerializeField] private ParticleSystem _brokenParticle;

    protected override ParticleSystem SymptomParticle => _symptomParticle;
    protected override ParticleSystem BrokenParticle => _brokenParticle;

    protected override string SymptomSoundKey => "SFX_Machine_Error";
    protected override string BrokenSoundKey => "SFX_Thermostat_Malfunction";

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

        _temperatureDial = GetComponent<XRKnobDial>();
        if (_temperatureDial == null)
        {
            Debug.LogError("TemperatureDial�� �������� �ʽ��ϴ�.");
            return;
        }

        _temperatureDial.onValueChange.AddListener(OnKnobValueChanged);
    }

    private void OnKnobValueChanged(float value)
    {
        // 1. ���� ���� Ȯ��
        if (_repair != null && IsBroken())
        {
            MessageDisplayManager.Instance.ShowMessage($"�µ� ���� ��ġ�� �����ؾ� ��� �� �� �ֽ��ϴ�.");
            return;
        }

        // 2. ���� ���� �ذ� ����
        if (_repair != null && _repair.IsSymptom) // ���� ������ ���� ���
        {
            if (Mathf.Approximately(value, 1.0f) || Mathf.Approximately(value, 0.0f))
            {
                SolveSymptom();
                return; // ���� ������ �ذ�Ǹ� ���� ���� �������� ����
            }
        }

        // 3. �̺�Ʈ ó�� ����
        TemperatureEvents(value);
    }

    private void TemperatureEvents(float value)
    {
        if (Mathf.Approximately(value, 1f)) // ���̾��� ���������� ������ �µ��� �ø� ��,
        {
            if (!EventManager.Instance._activeEventsID.Contains(421))
                // �µ� �ϰ� �̺�Ʈ�� �������� �ƴϸ�
                return;

            EventManager.Instance.EndEvent(421);
            ParticleManager.Instance.StopParticle("421");
        }

        if (Mathf.Approximately(value, 0f)) // ���̾��� �������� ������ �µ��� ���� ��,
        {
            if (!EventManager.Instance._activeEventsID.Contains(441))
                // �µ� ��� �̺�Ʈ�� �������� �ƴϸ�
                return;

            EventManager.Instance.EndEvent(441);
            ParticleManager.Instance.StopParticle("441");
        }
    }
}
