using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class AffordanceSystemManager : MonoBehaviour
{
    [Header("���� ����")]
    private Renderer _targetRenderer;
    [Tooltip("������Ʈ�� ������ ���� �� ��Ÿ�� ����")]
    [SerializeField] private Color _hoverColor;
    private Color _defaultColor;

    [Header("���� ����")]
    [SerializeField] private AudioSource _audioSource;
    [Tooltip("������Ʈ�� ������ ���� �� �鸮�� ����")]
    [SerializeField] private AudioClip _hoverSound;
    [Tooltip("������Ʈ�� ����� �� �鸮�� ����")]
    [SerializeField] private AudioClip _selectSound;

    [Header("������ ����")]
    private Transform _targetTransform;
    private Vector3 _defaultScale; // ������Ʈ�� �⺻ ������
    private Vector3 _hoverScale; // ������Ʈ�� ������ ���� �� ����Ǵ� ������

    [Tooltip("������Ʈ�� ������ ���� �� ����Ǵ� ��")]
    [SerializeField] private float _hoverScaleValue = 1; // ����Ǵ� ������ ��

    private XRBaseInteractable _interactable; // ��ȣ�ۿ� ���

    private void Awake()
    {
        _targetRenderer = GetComponentInChildren<Renderer>();
        _interactable = GetComponent<XRBaseInteractable>();
        _audioSource = GetComponent<AudioSource>();

        if (_targetRenderer == null)
        {
            Debug.Log("Renderer�� �����ϴ�.");
        }
        else
        {
            _defaultColor = _targetRenderer.material.color;
        }

        if (_interactable == null)
        {
            Debug.Log("XRBaseInteractable�� �����ϴ�.");
            return;
        }

        if ( _audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }

        _targetTransform = transform;
        _defaultScale = transform.localScale;
        _hoverScale = _defaultScale * _hoverScaleValue;

        _interactable.hoverEntered.AddListener(OnHoverEnter);
        _interactable.hoverExited.AddListener(OnHoverExit);
        _interactable.selectEntered.AddListener(OnSelectEnter);
        _interactable.selectExited.AddListener(OnSelectExit);
    }

    private void OnDestroy()
    {
        if (_interactable != null)
        {
            _interactable.hoverEntered.RemoveListener(OnHoverEnter);
            _interactable.hoverExited.RemoveListener(OnHoverExit);
            _interactable.selectEntered.RemoveListener(OnSelectEnter);
            _interactable.selectExited.RemoveListener(OnSelectExit);
        }
    }

    private void OnHoverEnter(HoverEnterEventArgs args)
    {
        Debug.Log("HoverEnter");
        ChangeColor(_hoverColor);
        ChangeScale(_hoverScale);
        PlaySound(_hoverSound);
    }

    private void OnHoverExit(HoverExitEventArgs args)
    {
        Debug.Log("HoverExit");
        InitialStateReset();
    }

    private void OnSelectEnter(SelectEnterEventArgs args)
    {
        Debug.Log("SelectEnter");
        InitialStateReset();
        PlaySound(_selectSound);
    }

    private void OnSelectExit(SelectExitEventArgs args)
    {
        Debug.Log("SelectExit");
        InitialStateReset();
    }

    private void ChangeColor(Color color)
    {
        if (_targetRenderer != null)
        {
            _targetRenderer.material.color = color;
        }
    }

    private void ChangeScale(Vector3 scale)
    {
        if (_targetTransform != null)
        {
            _targetTransform.localScale = scale;
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (_audioSource != null && clip != null)
        {
            _audioSource.PlayOneShot(clip);
        }
    }

    private void InitialStateReset()
    {
        ChangeColor(_defaultColor);
        ChangeScale(_defaultScale);
    }
}
