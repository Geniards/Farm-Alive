using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class TemperatureDial : BaseRepairable
{
    [SerializeField] private XRKnobDial _temperatureDial;

    protected override void Start()
    {
        base.Start();

        _temperatureDial = GetComponent<XRKnobDial>();
        if (_temperatureDial = null)
            return;

        _temperatureDial.onValueChange.AddListener(OnKnobValueChanged);
    }

    private void OnKnobValueChanged(float value)
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
