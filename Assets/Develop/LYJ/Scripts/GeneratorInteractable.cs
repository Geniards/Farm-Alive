using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;

public class GeneratorInteractable : XRBaseInteractable
{
    private PhotonView photonView;

    [Header("Generator Settings")]
    [Tooltip("�õ��� �ɸ������ �ʿ��� �õ��� ���� �õ� Ƚ��")]
    [SerializeField] private int _startAttemptsRequired = 3;

    [Tooltip("������ �߻��ϱ������ �ð�")]
    [SerializeField] private float _breakdownWarningDuration = 5f;

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
    private HeadLightInteractable _headLight;

    private Vector3 _initialCordPosition;   // �õ����� �ʱ� ��ġ
    private int _currentAttempts = 0;        // ���� �õ� �õ� Ƚ��
    private float _currentKnobValue = 0f;    // ���� ���� �� (���ư� ��ġ�� ���� ��)
    private bool _isBeingPulled = false;     // �õ����� ������� �ִ����� ���� ����
    private bool _hasTriggered = false;      // �õ����� Ʈ���ŵ� ���������� ���� ����
    private bool _isGeneratorRunning = true; // �����Ⱑ �۵� �������� ���� ����
    private bool _isLeverDown = false;       // ������ ������ ���������� ���� ����
    private bool _warningActive = false;     // ���� ���� Ȱ��ȭ ����
    private bool _isKnobAtMax = false;       // ���� �ִ� ��ġ�� �ִ����� ���� ����
    private bool _leverResetRequired = false; // ���� ������ �߻��� ���� ������ �÷ȴٰ� ������ ��

    private Coroutine warningCoroutine = null;  // ���� ���� �ڷ�ƾ

    protected override void Awake()
    {
        base.Awake();
        photonView = GetComponent<PhotonView>();

        if (photonView == null)
        {
            Debug.Log("photonView�� �����ϴ�.");
        }
    }

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        startPos = transform.position;

        _repair = GetComponentInParent<Repair>();
        _repair.enabled = false;

        _headLight = FindObjectOfType<HeadLightInteractable>();

        _knob = transform.root.GetComponentInChildren<XRKnobGenerator>();
        _lever = transform.root.GetComponentInChildren<XRLever>();

        _knob.onValueChange.AddListener(OnKnobValueChanged);
        _lever.onLeverActivate.AddListener(OnLeverActivate);
        _lever.onLeverDeactivate.AddListener(OnLeverDeactivate);

        if (_cordObject != null)
        {
            _initialCordPosition = _cordObject.position;
        }

        if (_headLight == null)
        {
            Debug.LogWarning("HeadLightInteractable�� ã�� �� �����ϴ�.");
        }
    }

    // �� ���� ����� ������ ȣ��
    private void OnKnobValueChanged(float value)
    {
        _currentKnobValue = value;

        // ���� �ִ� ������ ������ ���
        if (_currentKnobValue >= 1f && !_isGeneratorRunning)
        {
            _isKnobAtMax = true;
            photonView.RPC(nameof(SyncKnobState), RpcTarget.AllBuffered, true);

            if (!_repair.IsRepaired)
            {
                Debug.Log("���峪�� �ʾұ� ������ �õ��� ��뿡 �ǹ̰� �����ϴ�.");
                return;
            }

            //MessageDisplayManager.Instance.ShowMessage("���� �ִ� �������� ���ư� �õ����� ��� �� �ֽ��ϴ�.");
            //Debug.Log("���� �ִ� �������� ���ư� �õ����� ��� �� �ֽ��ϴ�.");
        }

        // ���� �ִ� ������ ��� ���
        else if (_currentKnobValue < 1f && _isKnobAtMax)
        {
            _isKnobAtMax = false;
            photonView.RPC(nameof(SyncKnobState), RpcTarget.AllBuffered, false);
            //MessageDisplayManager.Instance.ShowMessage("���� �ִ� �������� ��� �õ����� ��� �� �����ϴ�.");
            //Debug.Log("���� �ִ� �������� ��� �õ����� ��� �� �����ϴ�.");
        }
    }

    [PunRPC]
    private void SyncKnobState(bool isAtMax)
    {
        _isKnobAtMax = isAtMax;
    }

    // ������ ������ �� ȣ��
    private void OnLeverActivate()
    {
        // ���� ������ Ȱ��ȭ�� ��쿡�� ���� �����Ⱑ ������
        // ���� ���� ���� ���� ������ �ǹ� ����
        if (_warningActive)
        {
            if (_leverResetRequired)
            {
                // ������ �� �� �ö󰬴� �������� ���� ���� ���� �ؼ� ����
                _isLeverDown = true;
                photonView.RPC(nameof(SyncLeverState), RpcTarget.AllBuffered, true);

                // ���� ���� �ؼ�
                if (warningCoroutine != null)
                {
                    StopCoroutine(warningCoroutine);
                    warningCoroutine = null;
                    _warningActive = false;
                    _leverResetRequired = false; // ���� �ʱ�ȭ
                    MessageDisplayManager.Instance.ShowMessage("���� ������ �ذ�Ǿ����ϴ�!");
                }
            }
            else
            {
                MessageDisplayManager.Instance.ShowMessage("������ �÷ȴٰ� ������ ���� ������ �ذ��� �� �ֽ��ϴ�.");
            }
        }
        else
        {
            _isLeverDown = true;
            photonView.RPC(nameof(SyncLeverState), RpcTarget.AllBuffered, true);
        }
    }

    // ������ �÷��� �� ȣ��
    private void OnLeverDeactivate()
    {
        _isLeverDown = false;
        photonView.RPC(nameof(SyncLeverState), RpcTarget.AllBuffered, false);

        if (_warningActive)
        {
            // ���� ������ �߻��� ���¿��� ������ �� �� �ø�
            _leverResetRequired = true;
        }
    }

    [PunRPC]
    private void SyncLeverState(bool isDown)
    {
        _isLeverDown = isDown;
    }


    // �õ����� ����� �� ȣ��
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        rigid.isKinematic = false;
        _isBeingPulled = true;
        photonView.RPC(nameof(SyncPullState), RpcTarget.AllBuffered, true);
    }

    // �õ����� ������ �� ȣ��
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        _isBeingPulled = false;
        rigid.isKinematic = true; // ������ ����
        transform.position = startPos; // �õ��� ��ġ �ʱ�ȭ (�������)
        photonView.RPC(nameof(SyncPullState), RpcTarget.AllBuffered, false);
    }

    [PunRPC]
    private void SyncPullState(bool isPulled)
    {
        _isBeingPulled = isPulled;
    }

    private void Update()
    {
        // ���� ���� �׽�Ʈ (T Ű �Է� ��)
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("���� ���� �׽�Ʈ ����");
            TriggerWarning();
        }
    }

    // �õ����� �� ��ġ�� �������� �� ȣ��
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == _cordEndPosition && !_hasTriggered)
        {
            if (_isGeneratorRunning)
            {
                MessageDisplayManager.Instance.ShowMessage("������� �̹� ���� ���Դϴ�!");
                return;
            }

            // ������ �Ϸ�Ǿ����� Ȯ��
            // ��ġ�� ������ ���� ���� ������ �õ����� ���ų� ���� ������ �ǹ� ����
            if (!_repair.IsRepaired)
            {
                MessageDisplayManager.Instance.ShowMessage("���� ��ġ�� ������ �Ϸ��ϼ���.");
                //Debug.Log("���� ��ġ�� ������ �Ϸ��ϼ���.");
                return;
            }

            // ���� �ִ� ��ġ�� �ƴϸ� �õ����� ��� �� ����
            if (!_isKnobAtMax)
            {
                MessageDisplayManager.Instance.ShowMessage("�ٸ� �÷��̾ ���� �ִ�ġ�� ������ �õ����� ��� �� �ֽ��ϴ�.");
                //Debug.Log("�ٸ� �÷��̾ ���� �ִ�ġ�� ������ �õ����� ��� �� �ֽ��ϴ�.");
                return;
            }

            _hasTriggered = true;
            _currentAttempts++;

            MessageDisplayManager.Instance.ShowMessage($"������ �õ� Ƚ��: {_currentAttempts}/{_startAttemptsRequired}");
            //Debug.Log($"������ �õ� �õ�: {_currentAttempts}/{_startAttemptsRequired}");

            // �õ� ���� ���� Ȯ��
            if (_currentAttempts >= _startAttemptsRequired && _currentKnobValue >= 1f)
            {
                photonView.RPC(nameof(SyncSuccessGeneratorStart), RpcTarget.AllBuffered);
            }
        }
    }

    [PunRPC]
    private void SyncSuccessGeneratorStart()
    {
        MessageDisplayManager.Instance.ShowMessage("������ �õ� ����!");
        //Debug.Log("������ �õ� ����!");
        _isGeneratorRunning = true;
        _currentAttempts = 0;

        if (_headLight != null)
        {
            _headLight.RecoverFromBlackout(); // ���� ����
        }
        else
        {
            Debug.LogWarning("HeadLightInteractable�� �������� �ʾҽ��ϴ�!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform == _cordEndPosition)
        {
            _hasTriggered = false;
        }
    }

    // ���� ���� ����
    public void TriggerWarning()
    {
        //Debug.Log("TriggerWarning RPC ȣ�� �õ�");
        photonView.RPC(nameof(SyncTriggerWarning), RpcTarget.AllBuffered);
        //Debug.Log("TriggerWarning RPC ȣ�� �Ϸ�");
    }

    [PunRPC]
    private void SyncTriggerWarning()
    {
        _warningActive = true;
        _leverResetRequired = false;
        _isLeverDown = false;
        MessageDisplayManager.Instance.ShowMessage("���� ����! ������ ���� ������ �����ϼ���!!!");
        //Debug.Log("���� ����! ������ ���� ������ �����ϼ���!!!");

        //Debug.Log("SyncTriggerWarning �����");

        if (warningCoroutine == null)
        {
            //Debug.Log("BreakdownWarning �ڷ�ƾ ����");
            warningCoroutine = StartCoroutine(BreakdownWarning());
        }
    }

    // ���� ���� ó��
    // ���⼭ ó���ϸ� ���峪�� ���� (ó������ ���ϸ� ���峲)
    private IEnumerator BreakdownWarning()
    {
        //Debug.Log("BreakdownWarning �����");
        yield return new WaitForSeconds(_breakdownWarningDuration);

        if (!_isLeverDown)
        {
            MessageDisplayManager.Instance.ShowMessage("������ �߻��߽��ϴ�!! ��ġ�� 1�� ���� ���ּ���!!");
            //Debug.Log("������ �߻��߽��ϴ�!");
            photonView.RPC(nameof(SyncEnableRepair), RpcTarget.AllBuffered, true);
            _isGeneratorRunning = false;

            //Debug.Log("TriggerBlackout ȣ���");
            if (_headLight != null)
            {
                _headLight.TriggerBlackout(); // ���� �߻�
            }
            else
            {
                Debug.LogWarning("HeadLightInteractable�� �������� �ʾҽ��ϴ�!");
            }
        }

        _warningActive = false; // ���� ������ �� �̻� ������� ����
        warningCoroutine = null;
    }

    [PunRPC]
    private void SyncEnableRepair(bool isRepaired)
    {
        _repair.enabled = isRepaired;
        _repair.IsRepaired = isRepaired;

        if (isRepaired)
        {
            ResetGeneratorState();
        }
    }

    // �������� ���¸� �ʱ�ȭ
    private void ResetGeneratorState()
    {
        _isKnobAtMax = false;
        _currentKnobValue = 0f;

        _currentAttempts = 0;
        _hasTriggered = false;

        _isGeneratorRunning = false;
        _isBeingPulled = false;

        _warningActive = false;
    }

}
