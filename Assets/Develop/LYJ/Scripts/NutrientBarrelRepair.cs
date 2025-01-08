using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class NutrientBarrelRepair : BaseRepairable
{
    [SerializeField] private XRKnobDial _nutrientDial;

    protected override void Start()
    {
        base.Start();

        _nutrientDial = GetComponentInChildren<XRKnobDial>();
        if (_nutrientDial == null)
        {
            Debug.LogError("NutrientDial�� �������� �ʽ��ϴ�.");
            return;
        }

        _nutrientDial.onValueChange.AddListener(OnKnobValueChanged);
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
