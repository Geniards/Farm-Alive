using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Crop : MonoBehaviourPun
{
    public enum E_CropState
    {
        Growing, GrowStopped, GrowCompleted
    }

    [Header("�۹��� ���� ����")]
    [SerializeField] private E_CropState _cropState;

    [Header("��ġ")]
    [Tooltip("��Ȯ���� ���·� ����� ������ ���尡�� ���¿��� �ӹ����� �ϴ� �ð�")]
    [SerializeField] private float _growthTime;
    [Tooltip("���尡�� ���¿��� �ӹ��� �ð�")]
    [SerializeField] private float _elapsedTime;

    // TODO: ���尡�� ���� �Ǵ� ���� �߰�
    [SerializeField] private bool _canGrowth;

    private XRGrabInteractable _grabInteractable;

    private bool CanGrowth { 
        set
        { 
            _canGrowth = value;
            _cropState = _canGrowth ? E_CropState.Growing : E_CropState.GrowStopped;
        }
    }

    private void Awake()
    {
        _grabInteractable = GetComponent<XRGrabInteractable>();
    }

    private void OnEnable()
    {
        Init();
    }

    private void Update()
    {
        Grow();
    }

    private void Init()
    {
        _canGrowth = true;
        _cropState = E_CropState.Growing;
        _elapsedTime = 0.0f;
    }

    private void Grow()
    {
        if (!_canGrowth)
            return;

        _elapsedTime += Time.deltaTime;

        if (_elapsedTime >= _growthTime)
        {
            CompleteGrow();
        }
    }

    private void CompleteGrow()
    {
        _cropState = E_CropState.GrowCompleted;
        _canGrowth = false;
        _grabInteractable.enabled = true;

        // TODO: ����Ϸ� �ǵ�� ����
        transform.localScale *= 1.2f;
    }
}
