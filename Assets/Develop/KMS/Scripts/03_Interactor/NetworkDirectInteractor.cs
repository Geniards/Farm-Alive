using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkDirectInteractor : XRDirectInteractor
{
    [SerializeField] private PhotonView photonView;

    /// <summary>
    /// ��ü�� ����� �� �����ϴ� �޼���.
    /// </summary>
    /// <param name="args"></param>
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        // TODO : ���� ����� ��Ʈ��ũ�� ���ؼ� ����.
        // 1. ���� �÷��̾ ���� ��ü�� �������� ��������.
        PhotonView interactablePV = args.interactableObject.transform.GetComponent<PhotonView>();
        interactablePV.RequestOwnership();
    }

    /// <summary>
    /// ��ü�� ������ �� �����ϴ� �޼���.
    /// </summary>
    /// <param name="args"></param>
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        // TODO : ���� ����� ��Ʈ��ũ�� ���ؼ� ����.
        // 1. ���� �÷��̾ ���� ��ü�� �������� ���忡�� �ٽ� �����ֱ�.
        PhotonView interactablePV = args.interactableObject.transform.GetComponent<PhotonView>();
        interactablePV.TransferOwnership(PhotonNetwork.MasterClient);
    }
}
