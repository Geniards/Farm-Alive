using System.Collections;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;

public class GeneratorInteractable : XRBaseInteractable
{
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

    private Coroutine warningCoroutine = null;  // ���� ���� �ڷ�ƾ

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
            Debug.Log("���� �ִ� �������� ���ư� �õ����� ��� �� �ֽ��ϴ�.");
        }

        // ���� �ִ� ������ ��� ���
        else if (_currentKnobValue < 1f && _isKnobAtMax)
        {
            _isKnobAtMax = false;
            Debug.Log("���� �ִ� �������� ��� �õ����� ��� �� �����ϴ�.");
        }
    }

    // ������ ������ �� ȣ��
    private void OnLeverActivate()
    {
        // ���� ������ Ȱ��ȭ�� ��쿡�� ���� �����Ⱑ ������
        // ���� ���� ���� ���� ������ �ǹ� ����
        if (_warningActive)
        {
            _isLeverDown = true;
            Debug.Log("������ ���������ϴ�.");

            if (warningCoroutine != null)
            {
                StopCoroutine(warningCoroutine);
                warningCoroutine = null;
                _warningActive = false;
                Debug.Log("������ �������� ���� ������.");
            }
        }
        else
        {
            Debug.Log("���� ������ �߻��� ���� �������� ���� ���� X");
        }
    }

    // ������ �÷��� �� ȣ��
    private void OnLeverDeactivate()
    {
        _isLeverDown = false;
        Debug.Log("������ �ö󰬽��ϴ�.");
    }

    // �õ����� ����� �� ȣ��
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        rigid.isKinematic = false;
        _isBeingPulled = true;
    }

    // �õ����� ������ �� ȣ��
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        _isBeingPulled = false;
        rigid.isKinematic = true; // ������ ����
        transform.position = startPos; // �õ��� ��ġ �ʱ�ȭ (�������)
    }

    // 
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
            // ������ �Ϸ�Ǿ����� Ȯ��
            // ��ġ�� ������ ���� ���� ������ �õ����� ���ų� ���� ������ �ǹ� ����
            if (!_repair.IsRepaired)
            {
                Debug.Log("���� ��ġ�� ������ �Ϸ��ϼ���.");
                return;
            }

            // ���� �ִ� ��ġ�� �ƴϸ� �õ����� ��� �� ����
            if (!_isKnobAtMax)
            {
                Debug.Log("�ٸ� �÷��̾ ���� �ִ�ġ�� ������ �õ����� ��� �� �ֽ��ϴ�.");
                return;
            }

            _hasTriggered = true;
            _currentAttempts++;

            Debug.Log($"������ �õ� �õ�: {_currentAttempts}/{_startAttemptsRequired}");

            // �õ� ���� ���� Ȯ��
            if (_currentAttempts >= _startAttemptsRequired && _currentKnobValue >= 1f)
            {
                Debug.Log("������ �õ� ����!");
                _isGeneratorRunning = true;
                _currentAttempts = 0;

                // ���� ����
                if (_headLight != null)
                {
                    _headLight.RecoverFromBlackout(); // ���� ����
                }
                else
                {
                    Debug.LogWarning("HeadLightInteractable�� �������� �ʾҽ��ϴ�!");
                }
            }
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
        if (warningCoroutine == null)
        {
            _warningActive = true; // ���� ���� Ȱ��ȭ
            warningCoroutine = StartCoroutine(BreakdownWarning());
        }
    }

    // ���� ���� ó��
    // ���⼭ ó���ϸ� ���峪�� ���� (ó������ ���ϸ� ���峲)
    private IEnumerator BreakdownWarning()
    {
        Debug.Log("���� ����! ������ ���� ������ �����ϼ���!!!");
        yield return new WaitForSeconds(_breakdownWarningDuration);

        if (!_isLeverDown)
        {
            Debug.Log("������ �߻��߽��ϴ�!");
            _repair.enabled = true;
            _isGeneratorRunning = false;

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
}
