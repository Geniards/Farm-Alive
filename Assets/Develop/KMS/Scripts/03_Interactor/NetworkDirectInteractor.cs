using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkDirectInteractor : XRDirectInteractor
{
    [SerializeField] private PhotonView _photonView;

    /// <summary>
    /// ��ü�� ����� �� �����ϴ� �޼���.
    /// </summary>
    /// <param name="args"></param>
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        IXRSelectInteractable selectInteractable = args.interactableObject;
        
        if (args.interactorObject.transform.GetComponent<PhotonView>().IsMine == false)
            return;

        // 1. ���� �÷��̾ ���� ��ü�� �������� ��������.
        PhotonView interactablePV = selectInteractable.transform.GetComponent<PhotonView>();
        if (!interactablePV) return;
        interactablePV.TransferOwnership(PhotonNetwork.LocalPlayer);
        Debug.Log("������ ����");

        // 2. ���� ��� �˸���
        _photonView.RPC(nameof(SyncSelect), RpcTarget.Others, _photonView.ViewID, interactablePV.ViewID, true);
    }

    /// <summary>
    /// ��ü�� ������ �� �����ϴ� �޼���.
    /// </summary>
    /// <param name="args"></param>
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        if (!_photonView.IsMine)
            return;

        IXRSelectInteractable selectInteractable = args.interactableObject;

        // 1. ���� �÷��̾ ���� ��ü�� �������� ���忡�� �ٽ� �����ֱ�.
        PhotonView interactablePV = selectInteractable.transform.GetComponent<PhotonView>();
        if (!interactablePV) return;
        interactablePV.TransferOwnership(PhotonNetwork.MasterClient);

        // 2. ���� ��� �˸���
        _photonView.RPC(nameof(SyncSelect), RpcTarget.Others, _photonView.ViewID, interactablePV.ViewID, false);
    }

    [PunRPC]
    private void SyncSelect(int interactorID, int interactableID, bool isSelected)
    {
        PhotonView interactorPV =  PhotonView.Find(interactorID);
        PhotonView interactablePV =  PhotonView.Find(interactableID);
        Rigidbody interactableRb = interactablePV.GetComponent<Rigidbody>();
        if (interactablePV.GetComponent<TabletInteractable>() != null)
            return;

        IXRSelectInteractor interactor = interactorPV.GetComponent<IXRSelectInteractor>();
        IXRSelectInteractable interactable = interactablePV.GetComponent<IXRSelectInteractable>();

        if (isSelected)
        {
            interactionManager.SelectEnter(interactor, interactable);
        }
        else
        {
            if (!hasSelection)
                return;

            interactionManager.SelectExit(interactor, interactable);
        }
    }
}
