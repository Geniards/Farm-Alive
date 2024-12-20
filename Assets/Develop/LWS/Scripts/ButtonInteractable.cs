using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class ButtonInteractable : MonoBehaviour
{
    [System.Serializable]
    public class ButtonPressedEvent : UnityEvent { }
    [System.Serializable]
    public class ButtonReleasedEvent : UnityEvent { }

    // ��ư�� ������ �� ���� (�⺻������ y �Ʒ� ����)
    public Vector3 _axis = new Vector3(0, -1, 0);
    
    // ��ư�� ������ �� �� �Ÿ� (���� _axis �������� ������)
    public float _maxDistance;

    // ��ư�� ���� ��ġ�� ���ư��� �ӵ�
    public float _returnSpeed = 10.0f;

    // ��ư ������ �� �ۿ��� AudioClip
    // public AudioClip ButtonPressAudioClip;
    // public AudioClip ButtonReleaseAudioClip;

    // ��ư�� ������ ������ ���� �������� �� �����ϴ� �̺�Ʈ
    public ButtonPressedEvent _onButtonPressed;
    public ButtonReleasedEvent _onButtonReleased;

    // ��ư�� ������ġ ����
    Vector3 _startPosition;
    Rigidbody _rigidbody;
    Collider _collider;

    bool _pressed = false;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponentInChildren<Collider>();
        _startPosition = transform.position;
    }

    void FixedUpdate()
    {
        // ���� �����������
        Vector3 worldAxis = transform.TransformDirection(_axis);
        // end Position�� ��ư ���� ������ �Ÿ���ŭ
        Vector3 end = transform.position + worldAxis * _maxDistance;

        // ���� ��ư�� �̵� ��ġ
        float currentDistance = (transform.position - _startPosition).magnitude;
        RaycastHit hit;

        float move = 0.0f;

        if (_rigidbody.SweepTest(-worldAxis, out hit, _returnSpeed * Time.deltaTime + 0.005f))
        {
            // �浹�� �ִ� ��� : move�� ����� �Ͽ� ��ư�� �� ������
            move = (_returnSpeed * Time.deltaTime) - hit.distance;
        }
        else
        {
            // �浹�� ������ move�� ������ �Ͽ� ��ư ����
            move -= _returnSpeed * Time.deltaTime;
        }

        // ���ο� �̵��Ÿ� ��� �� ����
        float newDistance = Mathf.Clamp(currentDistance + move, 0, _maxDistance);

        _rigidbody.position = _startPosition + worldAxis * newDistance;

        // ������ �� �̺�Ʈ ȣ��
        if (!_pressed && Mathf.Approximately(newDistance, _maxDistance))
        {
            _pressed = true;
            /*
            SFXPlayer.Instance.PlaySFX(ButtonPressAudioClip, transform.position, new SFXPlayer.PlayParameters()
            {
                Pitch = Random.Range(0.9f, 1.1f),
                SourceID = -1,
                Volume = 1.0f
            }, 0.0f);
            */
            _onButtonPressed.Invoke();
        }
        else if (_pressed && !Mathf.Approximately(newDistance, _maxDistance))
        {
            _pressed = false;
            /*
            SFXPlayer.Instance.PlaySFX(ButtonReleaseAudioClip, transform.position, new SFXPlayer.PlayParameters()
            {
                Pitch = Random.Range(0.9f, 1.1f),
                SourceID = -1,
                Volume = 1.0f
            }, 0.0f);
            */
            _onButtonReleased.Invoke();
        }
    }
}