using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoistureRemoverRepair : BaseRepairable
{
    [SerializeField] private XRKnobDial _moistureRemoverDial;

    protected override void Start()
    {
        base.Start();

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
