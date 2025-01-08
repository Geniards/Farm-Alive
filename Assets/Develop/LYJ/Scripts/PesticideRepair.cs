using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class PesticideRepair : BaseRepairable
{
    [SerializeField] private XRKnob _pesticideDial;

    protected override void Start()
    {
        base.Start();

        _pesticideDial = GetComponentInChildren<XRKnob>();
        if (_pesticideDial == null)
        {
            Debug.LogError("PesticideDial�� �������� �ʽ��ϴ�.");
            return;
        }
    }
}
