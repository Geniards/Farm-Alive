using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ShovelInteractor : XRGrabInteractable
{
    private PlantGround _currentGround;
    private int _groundTriggerCount = 0;
    private PhotonView _photonView;

    protected override void Awake()
    {
        base.Awake();
        _photonView = GetComponent<PhotonView>();
    }

    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {
        base.OnSelectEntering(args);

        if (_photonView != null && _photonView.IsMine == false)
        {
            _photonView.RequestOwnership();
            Debug.Log($"������ ���� => {PhotonNetwork.LocalPlayer.ActorNumber}");
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        if (_photonView != null && _photonView.IsMine)
        {
            _photonView.TransferOwnership(PhotonNetwork.MasterClient);
            Debug.Log("������ ���� => ������ Ŭ���̾�Ʈ�� ��ȯ��");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlantGround ground = other.GetComponentInParent<PlantGround>();
        if (ground != null)
        {
            if (_currentGround == null)
            {
                _currentGround = ground;
                Debug.Log($"���� ����");
            }
            else
            {
                Debug.Log("���� ����: PlantGround ��ũ��Ʈ�� ����");
            }

            if (ground == _currentGround)
            {
                _groundTriggerCount++;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlantGround ground = other.GetComponentInParent<PlantGround>();
        if (ground != null && ground == _currentGround)
        {
            _groundTriggerCount--;

            if (_groundTriggerCount <= 0)
            {
                _currentGround.Dig();
                _currentGround = null;
                _groundTriggerCount = 0;
                Debug.Log("���� ����");
            }
        }
    }
}
