using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class DialInteractable : XRBaseInteractable, IPunObservable
{
    [Header("���̾� ����")]
    [Tooltip("���̾��� ȸ���� �� �ִ� �ִ� ���� (ex:0~90)")]
    [SerializeField] float _maxAngle = 90f;

    [Tooltip("�ܰ� �� (0�̸� ������ ȸ��, 2�̸� 3�ܰ�� �и� => 90��, 3�ܰ�� 0, 45, 90)")]
    [SerializeField] int _steps = 0;

    [Tooltip("������ �� ����� �ܰ迡 ������ ����")]
    [SerializeField] bool _snapOnRelease = true;

    [Header("�̺�Ʈ")]
    [SerializeField] UnityEvent<int> _onStepChanged;
    [SerializeField] UnityEvent<float> _onAngleChanged;

    private XRBaseInteractor _directInteractor;
    private float _currentAngle = 0f;
    private int _currentStep = 0;
    private float _stepSize = 0f;
    private Quaternion _startGrabRotation;

    private PhotonView _photonView;

    protected override void Awake()
    {
        base.Awake();

        _photonView = GetComponent<PhotonView>();

        if (_steps > 0)
            _stepSize = _maxAngle / _steps;
        // 90��, 2�ܰ� -> stepSize = 45
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        _directInteractor = args.interactorObject as XRBaseInteractor;

        if (_directInteractor != null)
        {
            // ���̾� ȸ���� ���ͷ��� ȸ���� ���ϱ� ���� ������ �׷������� ����
            _startGrabRotation = Quaternion.Inverse(transform.rotation) * _directInteractor.transform.rotation;
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        if (_snapOnRelease && _steps > 0)
        {
            float snappedAngle = Mathf.Round(_currentAngle / _stepSize) * _stepSize;
        }

        _directInteractor = null;
    }

    /// <summary>
    /// �� �����Ӹ��� ���̾� ���� ó��
    /// </summary>
    /// <param name="updatePhase"></param>
    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);

        if (_photonView != null && !_photonView.AmOwner)
            return;

        if (isSelected && _directInteractor != null
            && updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
        {
            // 1. ���� ��Ʈ�ѷ� ȸ���� ����� ���� ��� ȸ�� ����
            Quaternion relativeRotation = _directInteractor.transform.rotation
                                          * Quaternion.Inverse(_startGrabRotation);

            // 2. ������ ����
            float angle = Quaternion.Angle(Quaternion.identity, relativeRotation);

            // 3. �ִ� = maxAngle�� ����
            angle = Mathf.Clamp(angle, 0f, _maxAngle);

            if (!_snapOnRelease && _steps > 0)
            {
                // ��� �ִ� ���ȿ��� �ǽð����� ���̱�
                float snappedAngle = Mathf.Round(angle / _stepSize) * _stepSize;
                UpdateAngle(snappedAngle);
            }
            else
            {
                // ����ȸ�� Ȥ�� ���� ������ �ε巴��
                UpdateAngle(angle);
            }

            // 4. ���� ���̾� ȸ��
            // ��: Y�� �������� _currentAngle ȸ��
            transform.localRotation = Quaternion.Euler(0f, _currentAngle, 0f);
        }
    }


    private void UpdateAngle(float newAngle)
    {
        if (!Mathf.Approximately(_currentAngle, newAngle))
        {
            _currentAngle = newAngle;
            _onAngleChanged?.Invoke(_currentAngle);
        }

        if (_steps > 0)
        {
            int newStep = Mathf.RoundToInt(_currentAngle / _stepSize);
            if (newStep != _currentStep)
            {
                _currentStep = newStep;
                _onStepChanged?.Invoke(_currentStep);
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_currentAngle);
            stream.SendNext(_currentStep);
        }
        else // �ٸ� Ŭ���̾�Ʈ�� ����
        {
            float receivedAngle = (float)stream.ReceiveNext();
            int receivedStep = (int)stream.ReceiveNext();

            // ������ ����/�������� ������Ʈ
            if (!Mathf.Approximately(_currentAngle, receivedAngle))
            {
                _currentAngle = receivedAngle;
                _onAngleChanged?.Invoke(_currentAngle);
            }
            if (_currentStep != receivedStep)
            {
                _currentStep = receivedStep;
                _onStepChanged?.Invoke(_currentStep);
            }

            // ���� ���̾� ��ġ�� ����
            transform.localRotation = Quaternion.Euler(0f, _currentAngle, 0f);
        }
    }
}