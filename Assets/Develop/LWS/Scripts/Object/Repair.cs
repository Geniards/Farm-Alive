using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Repair : MonoBehaviourPun
{
    public UnityEvent OnSymptomRaised;
    public UnityEvent OnSymptomSolved;
    public UnityEvent OnBrokenRaised;
    public UnityEvent OnBrokenSolved;

    [Header("���� ����")]
    [Tooltip("�߻� �õ� �ֱ�(s)")]
    [SerializeField] float _invokePeriod;
    [Tooltip("�߻� Ȯ��(%)")]
    [SerializeField] float _invokeRate;
    [Tooltip("���� �ð�(s): �ʰ� �� ���� �߻�")]
    [SerializeField] float _limitTime;

    [Header("���� ����")]
    [Tooltip("���� �� ������ ���� �ʿ��� ��ġ�� Ƚ��")]
    [SerializeField] int _maxRepairCount;

    private int _curRepairCount = 0;
    private bool _isSymptom;
    private bool _isRepaired;

    public bool IsSymptom { 
        get { return _isSymptom; } 
        set 
        {
            _isSymptom = value;
            (_isSymptom ? OnSymptomRaised : OnSymptomSolved)?.Invoke();
        } 
    }
    public bool IsRepaired
    {
        get { return _isRepaired; }
        set
        {
            _isRepaired = value;
            (_isRepaired ? OnBrokenSolved : OnBrokenRaised)?.Invoke();
        }
    }

    private void Awake()
    {
        _invokePeriodDelay = new WaitForSeconds(_invokePeriod);
        _limitTimeDelay = new WaitForSeconds(_limitTime);

        ResetRepairState();

        OnSymptomRaised.AddListener(InvokeSymptom);
        OnSymptomSolved.AddListener(ResetRepairState);
        OnBrokenSolved.AddListener(ResetRepairState);
    }

    private void Start()
    {
        InvokeSymptom();
    }

    private void OnDisable()
    {
        if (_invokeSymptomCoroutine != null)
            StopCoroutine(_invokeSymptomCoroutine);
    }

    private void InvokeSymptom()
    {
        if (_invokeSymptomCoroutine == null)
            _invokeSymptomCoroutine = StartCoroutine(InvokeSymptomRoutine());
    }

    Coroutine _invokeSymptomCoroutine;
    private WaitForSeconds _invokePeriodDelay;
    IEnumerator InvokeSymptomRoutine()
    {
        while (IsSymptom == false)
        {
            Debug.Log($"{gameObject.name} �������� �߻� Ȯ��...");
            IsSymptom = ProbabilityHelper.Draw(_invokeRate);
            if (IsSymptom)
                InvokeBroken();

            yield return _invokePeriodDelay;
        }

        yield return null;
    }

    private void InvokeBroken()
    {
        Debug.Log($"{gameObject.name} ���� {_limitTime}�� �� �߻�");
        if (_invokeBrokenCoroutine == null)
            _invokeBrokenCoroutine = StartCoroutine(InvokeBrokenRoutine());
    }

    Coroutine _invokeBrokenCoroutine;
    private WaitForSeconds _limitTimeDelay;
    IEnumerator InvokeBrokenRoutine()
    {
        yield return _limitTimeDelay;

        IsRepaired = false;
    }

    [PunRPC]
    public void RPC_PlusRepairCount()
    {
        if (IsRepaired)
            return;

        if (!PhotonNetwork.IsMasterClient)
            return;

        _curRepairCount++;
        MessageDisplayManager.Instance.ShowMessage($"������: {_curRepairCount}/{_maxRepairCount}");

        if (_curRepairCount >= _maxRepairCount)
        {
            // ���� �Ϸ�
            MessageDisplayManager.Instance.ShowMessage("�����Ϸ�!");
            IsRepaired = true;  //�����⿡�� ��ġ�� 1�� ������ �Ǿ��ٴ� �� �˾ƾ� 2�� ���� (�� + �õ���)�� �� �� ����
            // TODO: ���� �Ϸ� ���� (ex: ������Ʈ �ı�, �ִϸ��̼�, ���� ��ȯ ��)
        }
    }

    /// <summary>
    /// ���� ���¸� �ʱ�ȭ(�������� �ذ� �Ϸ� or ���� ���� �Ϸ�)
    /// </summary>
    public void ResetRepairState()
    {
        IsSymptom = true;
        IsRepaired = true;
        _curRepairCount = 0;
        _invokeSymptomCoroutine = null;
        _invokeBrokenCoroutine = null;

        InvokeSymptom();

        Debug.Log($"{gameObject.name} ���� ���� �ʱ�ȭ: _curRepairCount={_curRepairCount}, IsRepaired={IsRepaired}");
    }

    #region TestCode
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
            ResetRepairState();
    }

    public void Symptom()
    {
        Debug.Log($"{gameObject.name} �������� �߻�!");
        GetComponent<Renderer>().material.color = Color.gray;
    }

    public void Broken()
    {
        Debug.Log($"{gameObject.name} ���� �߻�!");
        GetComponent<Renderer>().material.color = Color.black;
    }
    #endregion
}
