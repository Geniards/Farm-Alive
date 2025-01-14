using Photon.Pun;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;

public class GeneratorInteractable : XRBaseInteractable
{
    private PhotonView photonView;

    [Header("Generator Settings")]
    [Tooltip("�õ��� �ɸ������ �ʿ��� �õ��� ���� �õ� Ƚ��")]
    [SerializeField] private int _startAttemptsRequired;

    [Tooltip("�õ����� ���� ���� ��ġ")]
    [SerializeField] private Transform _cordStartPosition;
    [Tooltip("�õ����� �ִ� �� ��ġ")]
    [SerializeField] private Transform _cordEndPosition;
    [Tooltip("�õ��� ������Ʈ")]
    [SerializeField] private Transform _cordObject;

    private Rigidbody rigid;
    private Vector3 startPos;

    private XRKnobGenerator _knob;
    private XRLever _lever;

    private Repair _repair;

    private int _currentAttempts = 0;      // ���� �õ� �õ� Ƚ��
    private float _currentKnobValue = 0f;  // ���� ���� ��
    private bool _isBeingPulled = false;   // �õ����� ������� �ִ��� ����
    private bool _hasTriggered = false;    // �õ��� Ʈ���� ����
    private bool _isGeneratorRunning = true; // �����Ⱑ �۵� ������ ����
    private bool _isKnobAtMax = false;     // ���� �ִ� ��ġ���� ����
    private bool _isLeverDown = false;     // ������ ������ �������� ����
    private bool _isBroken = false;        // ���� ���� ����

    protected override void Awake()
    {
        base.Awake();
        photonView = GetComponent<PhotonView>();

        if (photonView == null)
        {
            Debug.LogError("PhotonView�� �����ϴ�.");
        }

        _startAttemptsRequired = CSVManager.Instance.Facilities[232].facility_maxBootCount;
    }

    private void Start()
    {
        Transform generatorParent = transform.parent;

        _cordStartPosition = generatorParent.Find("CordStartPosition");
        _cordEndPosition = generatorParent.Find("CordEndPosition");
        _cordObject = transform;

        rigid = GetComponent<Rigidbody>();
        startPos = transform.position;

        _knob = transform.root.GetComponentInChildren<XRKnobGenerator>();
        _lever = transform.root.GetComponentInChildren<XRLever>();

        _repair = GetComponentInParent<Repair>();
        if (_repair == null)
        {
            Debug.LogError("Repair ������Ʈ�� ã�� �� �����ϴ�.");
            return;
        }

        // Knob, Lever �̺�Ʈ ���
        _knob.onValueChange.AddListener(OnKnobValueChanged);
        _lever.onLeverActivate.AddListener(OnLeverActivate);
        _lever.onLeverDeactivate.AddListener(OnLeverDeactivate);

        // Repair �̺�Ʈ ����
        _repair.OnSymptomRaised.AddListener(Symptom);      // ���� ���� �߻�
        _repair.OnBrokenRaised.AddListener(Broken);        // ���� �߻�
        _repair.OnBrokenSolved.AddListener(SolveBroken);
    }

    private void OnKnobValueChanged(float value)
    {
        _currentKnobValue = value;

        if (_currentKnobValue >= 1f && !_isGeneratorRunning)
        {
            _isKnobAtMax = true;
            photonView.RPC(nameof(SyncKnobState), RpcTarget.AllBuffered, _currentKnobValue, true);
        }
        else if (_currentKnobValue < 1f && _isKnobAtMax)
        {
            _isKnobAtMax = false;
            photonView.RPC(nameof(SyncKnobState), RpcTarget.AllBuffered, _currentKnobValue, false);
        }
    }

    [PunRPC]
    private void SyncKnobState(float knobValue, bool isAtMax)
    {
        _currentKnobValue = knobValue;
        _isKnobAtMax = isAtMax;
    }

    private void OnLeverActivate()
    {
        if (_repair.IsSymptom)
        {
            _isLeverDown = true;
            SolveSymptom(); // ���� ���� �ذ�
        }
    }

    private void OnLeverDeactivate()
    {
        _isLeverDown = false;
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        rigid.isKinematic = false;
        _isBeingPulled = true;
        photonView.RPC(nameof(SyncPullState), RpcTarget.AllBuffered, true);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        _isBeingPulled = false;
        rigid.isKinematic = true; // ������ ����
        transform.position = startPos; // �õ��� ��ġ �ʱ�ȭ
        photonView.RPC(nameof(SyncPullState), RpcTarget.AllBuffered, false);
    }

    [PunRPC]
    private void SyncPullState(bool isPulled)
    {
        _isBeingPulled = isPulled;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == _cordEndPosition && !_hasTriggered)
        {
            if (_isGeneratorRunning)
            {
                MessageDisplayManager.Instance.ShowMessage("������� �̹� ���� ���Դϴ�!");
                //Debug.LogError("������� �̹� ���� ���Դϴ�!");
                return;
            }

            if (!_repair.IsRepaired)
            {
                MessageDisplayManager.Instance.ShowMessage("���� ��ġ�� ������ �Ϸ��ϼ���.");
                //Debug.LogError("���� ��ġ�� ������ �Ϸ��ϼ���.");
                return;
            }

            if (!_isKnobAtMax || _currentKnobValue < 1f)
            {
                MessageDisplayManager.Instance.ShowMessage("�ٸ� �÷��̾ ���� �ִ�ġ�� ������ �õ����� ��� �� �ֽ��ϴ�.");
                //Debug.LogError("�ٸ� �÷��̾ ���� �ִ�ġ�� ������ �õ����� ��� �� �ֽ��ϴ�.");
                return;
            }

            _hasTriggered = true;
            _currentAttempts++;

            //Debug.LogError($"������ �õ� Ƚ��: {_currentAttempts}/{_startAttemptsRequired}");
            MessageDisplayManager.Instance.ShowMessage($"������ �õ� Ƚ��: {_currentAttempts}/{_startAttemptsRequired}");

            if (_currentAttempts >= _startAttemptsRequired)
            {
                photonView.RPC(nameof(SyncSuccessGeneratorStart), RpcTarget.AllBuffered);
            }
        }
    }

    [PunRPC]
    private void SyncSuccessGeneratorStart()
    {
        MessageDisplayManager.Instance.ShowMessage("������ �õ� ����!");
        //Debug.LogError("������ �õ� ����!");
        _isGeneratorRunning = true;
        _currentAttempts = 0;

        LightingManager.Instance.EndBlackout();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform == _cordEndPosition)
        {
            _hasTriggered = false;
        }
    }

    // �������� ���¸� �ʱ�ȭ
    private void ResetGeneratorState()
    {
        _isKnobAtMax = false;
        _currentKnobValue = 0f;

        _currentAttempts = 0;
        _hasTriggered = false;

        _isGeneratorRunning = true;
        _isBeingPulled = false;
    }

    // ���� ���� �߻� ó��
    public void Symptom()
    {
        _isBroken = false;
        MessageDisplayManager.Instance.ShowMessage("������ �������� �߻�!");
        //Debug.LogError("������ �������� �߻�!");
    }

    // ���� �߻� ó��
    public void Broken()
    {
        _isGeneratorRunning = false;
        _isBroken = true;
        MessageDisplayManager.Instance.ShowMessage("�����Ⱑ ���峵���ϴ�!");
        //Debug.LogError("�����Ⱑ ���峵���ϴ�!");
        LightingManager.Instance.StartBlackout();
    }

    // ���� ���� �ذ� ó��
    public void SolveSymptom()
    {
        // �̹� ���峭 ���¿����� ���� ������ �ذ��� �� ����
        if (_isBroken)
        {
            MessageDisplayManager.Instance.ShowMessage("�̹� ���峭 ���¿����� ���� ������ �ذ��� �� �����ϴ�.");
            //Debug.LogError("�̹� ���峭 ���¿����� ���� ������ �ذ��� �� �����ϴ�.");
            return;
        }

        if(_repair.IsSymptom)
        {
            _repair.IsSymptom = false;
        }

        _isBroken = false; // ���� ���� �ʱ�ȭ
        _repair.ResetRepairState();
        MessageDisplayManager.Instance.ShowMessage("���� ������ �ذ�Ǿ����ϴ�!");
        //Debug.LogError("���� ������ �ذ�Ǿ����ϴ�!");
    }

    // ���� ���� ó��
    public void SolveBroken()
    {
        // ���� ������ �̹� �ذ�� ��� ���� �߻� ����
        if (!_repair.IsSymptom)
        {
            return;
        }

        _isGeneratorRunning = false;
        _isBroken = true;
        _repair.ResetRepairState();
        MessageDisplayManager.Instance.ShowMessage("�����Ⱑ ���峵���ϴ�!");
        //Debug.LogError("�����Ⱑ ���峵���ϴ�!");
        LightingManager.Instance.StartBlackout();
    }
}
