using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CupInteractable : MonoBehaviourPun, IPunObservable
{
    [Tooltip("��ü�� �帣�� ��ƼŬ")]
    [SerializeField] public ParticleSystem particleSystemLiquid;
    
    [Tooltip("��ü�� �� ���� �ʱⰪ (1.0�̸� �� ��)")]
    [SerializeField] float _fillAmount = 1.0f;

    [Tooltip("����� ��� �귯���� ����")]
    [SerializeField] public float pourRate = 0.1f;

    // [Tooltip("��ü�� �帣�� ����")]
    // [SerializeField] AudioClip _pouringSound;

    // private AudioSource _audioSource;

    private bool _isPouring = false;

    private void OnEnable()
    {
        if (particleSystemLiquid != null)
        {
            Debug.LogWarning("�� �帣�� ��ƼŬ ����");
            particleSystemLiquid.Stop();
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
            Debug.LogWarning("�����������̳����»���");
            // �״� ���� �ƴѵ� ���� �ױ� ���� -> ���� ��ȯ
            if (!_isPouring)
            {
                _isPouring = true;
            }

            // ��ü�� ����
            _fillAmount -= pourRate * Time.deltaTime;
            if (_fillAmount < 0f) _fillAmount = 0f;

            // ��ƼŬ/���� (���ÿ��� ���)
            if (particleSystemLiquid != null && particleSystemLiquid.isStopped)
                particleSystemLiquid.Play();

            SoundManager.Instance.PlaySFXLoop("SFX_NutrientContainerPoured");
        }
        else
        {
            // �� �̻� ���� �ʴ� ��Ȳ
            if (_isPouring)
            {
                Debug.LogWarning("����������");
                _isPouring = false;
            }

            // ��ƼŬ/���� ����
            if (particleSystemLiquid != null) particleSystemLiquid.Stop();
            SoundManager.Instance.StopSFXLoop("SFX_NutrientContainerPoured");
        }
    }

    private void UpdateEffects()
    {
        if (_isPouring && _fillAmount > 0)
        {
            if (particleSystemLiquid != null && particleSystemLiquid.isStopped)
                particleSystemLiquid.Play();

            SoundManager.Instance.PlaySFXLoop("SFX_NutrientContainerPoured");
        }
        else
        {
            // �װ� ���� �ʰų� ��ü�� ������ ����
            if (particleSystemLiquid != null) particleSystemLiquid.Stop();
            SoundManager.Instance.StopSFXLoop("SFX_NutrientContainerPoured");
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
