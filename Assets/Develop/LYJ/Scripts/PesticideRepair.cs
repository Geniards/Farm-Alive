using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class PesticideRepair : BaseRepairable
{
    [SerializeField] private XRKnobDial _pesticideDial;

    protected override void Start()
    {
        base.Start();

        _pesticideDial = GetComponentInChildren<XRKnobDial>();
        if (_pesticideDial == null)
        {
            Debug.LogError("PesticideDial�� �������� �ʽ��ϴ�.");
            return;
        }

        _pesticideDial.onValueChange.AddListener(OnKnobValueChanged);
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
        if (_pesticideDial != null)
        {
            _pesticideDial.value = 0.0f;
        }
    }
}
