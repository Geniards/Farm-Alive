using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CupInteractable : MonoBehaviourPunCallbacks, IPunObservable
{
    [Tooltip("��ü�� �帣�� ��ƼŬ")]
    [SerializeField] ParticleSystem _particleSystemLiquid;
    
    [Tooltip("��ü�� �� ���� �ʱⰪ (1.0�̸� �� ��)")]
    [SerializeField] float _fillAmount = 1.0f;

    [Tooltip("����� ��� �귯���� ����")]
    [SerializeField] float _pourRate = 0.1f;

    [Tooltip("��ü�� �帣�� ����")]
    [SerializeField] AudioClip _pouringSound;

    private AudioSource _audioSource;

    private bool _isPouring = false;

    private void OnEnable()
    {
        if (_particleSystemLiquid != null)
        {
            _particleSystemLiquid.Stop();
        }
    }

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource != null)
        {
            _audioSource.clip = _pouringSound;
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            Pour();
        }
        else
        {
            UpdateEffects();
        }
    }

    private void Pour()
    {
        // ���� ����������: transform.up�� Vector3.down�� ������ ����� "�����Ͱ� �Ʒ������� ���Ѵ�"
        bool shouldPour = (Vector3.Dot(transform.up, Vector3.down) > 0 && _fillAmount > 0);

        if (shouldPour)
        {
            // �״� ���� �ƴѵ� ���� �ױ� ���� -> ���� ��ȯ
            if (!_isPouring)
            {
                _isPouring = true;
            }

            // ��ü�� ����
            _fillAmount -= _pourRate * Time.deltaTime;
            if (_fillAmount < 0f) _fillAmount = 0f;

            // ��ƼŬ/���� (���ÿ��� ���)
            if (_particleSystemLiquid != null && _particleSystemLiquid.isStopped)
                _particleSystemLiquid.Play();

            if (_audioSource != null && !_audioSource.isPlaying)
                _audioSource.Play();

            // ����ĳ��Ʈ �� LiquidContainer ó��
            RaycastHit hit;
            if (_particleSystemLiquid != null)
            {
                if (Physics.Raycast(_particleSystemLiquid.transform.position, Vector3.down, out hit, 50.0f))
                {
                    LiquidContainer receiver = hit.collider.GetComponent<LiquidContainer>();
                    if (receiver != null)
                    {
                        float amount = _pourRate * Time.deltaTime;
                        receiver.ReceiveLiquid(amount);
                    }
                }
            }
        }
        else
        {
            // �� �̻� ���� �ʴ� ��Ȳ
            if (_isPouring)
            {
                _isPouring = false;
            }

            // ��ƼŬ/���� ����
            if (_particleSystemLiquid != null) _particleSystemLiquid.Stop();
            if (_audioSource != null) _audioSource.Stop();
        }
    }

    private void UpdateEffects()
    {
        if (_isPouring && _fillAmount > 0)
        {
            if (_particleSystemLiquid != null && _particleSystemLiquid.isStopped)
                _particleSystemLiquid.Play();

            if (_audioSource != null && !_audioSource.isPlaying)
                _audioSource.Play();
        }
        else
        {
            // �װ� ���� �ʰų� ��ü�� ������ ����
            if (_particleSystemLiquid != null) _particleSystemLiquid.Stop();
            if (_audioSource != null) _audioSource.Stop();
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_fillAmount);
            stream.SendNext(_isPouring);
        }
        else
        {
            _fillAmount = (float)stream.ReceiveNext();
            _isPouring = (bool)stream.ReceiveNext();
        }
    }
}
