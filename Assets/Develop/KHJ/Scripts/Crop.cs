using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Crop : MonoBehaviourPun
{
    public enum E_CropState
    {
        Growing, GrowStopped, GrowCompleted, SIZE
    }

    [Header("�۹��� ���� ����")]
    [SerializeField] private E_CropState _curState;

    [Header("��ġ")]
    [Tooltip("��Ȯ���� ���·� ����� ������ ���尡�� ���¿��� �ӹ����� �ϴ� �ð�")]
    [SerializeField] private float _growthTime;
    [Tooltip("���尡�� ���¿��� �ӹ��� �ð�")]
    [SerializeField] private float _elapsedTime;
    [Tooltip("���尡�� ���°� �Ǳ� ���� �ʿ��� ����")]
    [SerializeField] private int _maxMoisture;
    [Tooltip("�۹��� ���� ����")]
    [SerializeField] private int _curMoisture;
    [Tooltip("���尡�� ���°� �Ǳ� ���� �ʿ��� �����")]
    [SerializeField] private int _maxNutrient;
    [Tooltip("�۹��� ���� �����")]
    [SerializeField] private int _curNutrient;

    private BaseState[] _states = new BaseState[(int)E_CropState.SIZE];
    private XRGrabInteractable _grabInteractable;

    private void Awake()
    {
        _states[(int)E_CropState.Growing] = new GrowingState(this);
        _states[(int)E_CropState.GrowStopped] = new GrowStoppedState(this);
        _states[(int)E_CropState.GrowCompleted] = new GrowCompletedState(this);

        _grabInteractable = GetComponent<XRGrabInteractable>();
    }

    private void OnEnable()
    {
        Init();
    }

    private void Update()
    {
        _states[(int)_curState].StateUpdate();
    }

    private void OnDestroy()
    {
        _states[(int)_curState].StateExit();
    }

    private void Init()
    {
        _curState = E_CropState.GrowStopped;
        _states[(int)_curState].StateEnter();

        _elapsedTime = 0.0f;
        _curMoisture = 0;
        _curNutrient = 0;
    }

    public void ChangeState(E_CropState state)
    {
        _states[(int)_curState].StateExit();
        _curState = state;
        _states[(int)_curState].StateEnter();
    }

    public void IncreaseMoisture() => _curMoisture++;
    public void IncreaseNutrient() => _curNutrient++;

    #region �۹� ���� �ൿ �� ����
    private class CropState : BaseState
    {
        public Crop crop;
        public CropState(Crop crop) => this.crop = crop;

        public override void StateEnter()
        {
            throw new System.NotImplementedException();
        }

        public override void StateExit()
        {
            throw new System.NotImplementedException();
        }

        public override void StateUpdate()
        {
            throw new System.NotImplementedException();
        }
    }

    private class GrowingState : CropState
    {
        public GrowingState(Crop crop) : base(crop) { }

        public override void StateEnter() { }

        public override void StateExit() { }

        public override void StateUpdate()
        {
            // �ൿ
            Grow();

            // ���� ����
            if (crop._elapsedTime >= crop._growthTime)
            {
                crop.ChangeState(E_CropState.GrowCompleted);
            }
        }

        private void Grow() => crop._elapsedTime += Time.deltaTime;
    }

    private class GrowStoppedState : CropState
    {
        public GrowStoppedState(Crop crop) : base(crop) { }

        public override void StateEnter() { }

        public override void StateExit() { }

        public override void StateUpdate()
        {
            // ���� ����
            if (CheckGrowthCondition())
            {
                crop.ChangeState(E_CropState.Growing);
            }
        }

        private bool CheckGrowthCondition()
        {
            return CheckMoisture() && CheckNutrient();
        }

        private bool CheckMoisture() => crop._curMoisture >= crop._maxMoisture;
        private bool CheckNutrient() => crop._curNutrient >= crop._maxNutrient;
    }

    private class GrowCompletedState : CropState
    {
        public GrowCompletedState(Crop crop) : base(crop) { }

        public override void StateEnter()
        {
            crop._grabInteractable.enabled = true;

            // TODO: ����Ϸ� �ǵ�� ����
            crop.transform.localScale *= 1.2f;

        }

        public override void StateExit() { }

        public override void StateUpdate() { }
    }
    #endregion
}
