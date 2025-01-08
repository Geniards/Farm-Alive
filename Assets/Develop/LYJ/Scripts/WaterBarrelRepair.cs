using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class WaterBarrelRepair : BaseRepairable
{
    [SerializeField] private XRKnob _waterDial;

    protected override void Start()
    {
        base.Start();

        _waterDial = GetComponentInChildren<XRKnob>();
        if (_waterDial == null)
        {
            Debug.LogError("WaterDial�� �������� �ʽ��ϴ�.");
            return;
        }

        _waterDial.onValueChange.AddListener(OnKnobValueChanged);
    }

    private void OnKnobValueChanged(float value)
    {
        if (_repair == null || !_repair.IsSymptom) // ���� ������ ������ ó������ ����
            return;

        // Knob ���� 1.0f�� �������� �� ���� ���� �ذ�
        if (Mathf.Approximately(value, 1.0f))
        {
            SolveSymptom();

            ResetKnobValue();
        }
    }

    // Knob ���� 0���� �ʱ�ȭ
    private void ResetKnobValue()
    {
        if (_waterDial != null)
        {
            _waterDial.value = 0.0f;
        }
    }
}
