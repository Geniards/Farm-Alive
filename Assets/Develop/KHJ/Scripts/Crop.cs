using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Crop : MonoBehaviourPun
{
    public enum E_CropState
    {
        Seeding, Growing, GrowStopped, GrowCompleted, SIZE
    }

    [Header("�۹� ����")]
    [Tooltip("���� �ܰ迡 ���� ��ȭ�ϴ� ����")]
    [SerializeField] private GameObject[] _GFXs;

    [Header("�۹��� ���� ����")]
    [SerializeField] private E_CropState _curState;

    [Header("��ġ")]
    [Tooltip("�翡 �ɾ����� ���� ���� ���۵ž��ϴ� Ƚ��")]
    [SerializeField] private int _digCount;
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
    private CropInteractable _cropInteractable;
    private int _maxGrowthStep;

    public E_CropState CurState { get { return _curState; } }
    public int DigCount {  get { return _digCount; } }

    private void Awake()
    {
        Transform GFX = transform.GetChild(0);
        _GFXs = new GameObject[GFX.childCount];
        for (int i = 0; i < GFX.childCount; i++)
        {
            _GFXs[i] = GFX.GetChild(i).gameObject;
        }
        _maxGrowthStep = _GFXs.Length - 1;

        _states[(int)E_CropState.Seeding] = new SeedingState(this);
        _states[(int)E_CropState.Growing] = new GrowingState(this);
        _states[(int)E_CropState.GrowStopped] = new GrowStoppedState(this);
        _states[(int)E_CropState.GrowCompleted] = new GrowCompletedState(this);

        _cropInteractable = GetComponent<CropInteractable>();
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
        _GFXs[0].SetActive(true);
        for (int i = 1; i < _GFXs.Length; i++)
            _GFXs[i].SetActive(false);

        _curState = E_CropState.Seeding;
        _states[(int)_curState].StateEnter();

        _elapsedTime = 0.0f;
        _curMoisture = 0;
        _curNutrient = 0;
    }

    [PunRPC]
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

    private class SeedingState : CropState
    {
        public SeedingState(Crop crop) : base(crop) { }

        public override void StateEnter() { }
        public override void StateExit() { }
        public override void StateUpdate() { }
    }

    private class GrowingState : CropState
    {
        private int curGrowthStep = 0;

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
                crop.photonView.RPC(nameof(crop.ChangeState), RpcTarget.All, E_CropState.GrowCompleted);
            }
        }

        private void Grow()
        {
            // ���� �ð� ����
            crop._elapsedTime += Time.deltaTime;

            // ����ġ�� ���� ���� ��ȭ
            if (crop._elapsedTime >= crop._growthTime * (curGrowthStep + 1) / crop._maxGrowthStep)
            {
                crop._GFXs[curGrowthStep].SetActive(false);
                crop._GFXs[++curGrowthStep].SetActive(true);
            }
        }
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
                crop.photonView.RPC(nameof(crop.ChangeState), RpcTarget.All, E_CropState.Growing);
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
            crop._GFXs[crop._maxGrowthStep - 1].SetActive(false);
            crop._GFXs[crop._maxGrowthStep].SetActive(true);
        }

        public override void StateExit() { }

        public override void StateUpdate() { }
    }
    #endregion

    #region TestCode
    public void CompleteGrowth()
    {
        ChangeState(E_CropState.GrowCompleted);
    }
    #endregion
}
