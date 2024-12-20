using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CupInteractable : MonoBehaviour
{
    public ParticleSystem _particleSystemLiquid;
    
    // ��ü�� �� ���� �ʱⰪ (1.0�̸� �� ��)
    private float _fillAmount = 1.0f;
    // �ܺο��� ���� �ٲ� ��� (ȣ���� ���� ä��ٰų�)
    public float FillAmount { get { return _fillAmount; } set { _fillAmount = value; } }

    // �ʴ� �귯���� ����
    private float _pourRate = 0.1f;

    // ���� �帣�� ����
    public AudioClip _pouringSound;
    private AudioSource _audioSource;

    private void OnEnable()
    {
        _particleSystemLiquid.Stop();
    }

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _pouringSound;
    }

    private void Update()
    {
        // ������ �� �ְ�, ��ü�� �������� ��
        if (Vector3.Dot(transform.up, Vector3.down) > 0 && _fillAmount > 0)
        {
            // ��ƼŬ, ������ �Ҹ� ���
            if (_particleSystemLiquid.isStopped && _particleSystemLiquid != null && _audioSource != null)
            {
                _particleSystemLiquid.Play();
                _audioSource.Play();
                // TODO:��������
                Debug.Log("��ü ������ �Ҹ� �� ��ƼŬ ���");
            }

            // �ܷ� ����
            _fillAmount -= _pourRate * Time.deltaTime;
            _fillAmount = Mathf.Max(_fillAmount, 0.0f);
            
            RaycastHit hit;
            // ��ƼŬ �ý��� �������� �Ʒ��� 50������ ���̸� �߻��ؼ�,
            if (Physics.Raycast(_particleSystemLiquid.transform.position, Vector3.down, out hit, 50.0f))
            {
                // hit�� LiquidReceiver (����, ����뿡 ����) ������Ʈ�� ������ ���� ���
                LiquidContainer receiver = hit.collider.GetComponent<LiquidContainer>();
                Debug.Log("������ �����̳ʿ� ���� ����");

                // ��, ��� Ÿ�Կ� ���� �Լ� ����
                // => Ÿ�� ���� ���� �״� ���� ��ŭ ��ü ä���
                if (receiver != null)
                {
                    float amount = _pourRate * Time.deltaTime;
                    receiver.ReceiveLiquid(amount);
                    Debug.Log("������ �����̳� ReceiveLiquid ȣ��");
                }
            }
        }
        else
        {
            _particleSystemLiquid.Stop();
            _audioSource.Stop();
            Debug.Log("��ü ������ �Ҹ� �� ��ƼŬ ����");
        }
    }
}
