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

        // 1. ���� �÷��̾ ���� ��ü�� �������� ��������.
        PhotonView interactablePV = selectInteractable.transform.GetComponent<PhotonView>();
        interactablePV.RequestOwnership();

        // 2. ���� ��� �˸���
        _photonView.RPC(nameof(SyncSelect), RpcTarget.Others, interactablePV.ViewID, true);
    }

    /// <summary>
    /// ��ü�� ������ �� �����ϴ� �޼���.
    /// </summary>
    /// <param name="args"></param>
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        IXRSelectInteractable selectInteractable = args.interactableObject;

        // 1. ���� �÷��̾ ���� ��ü�� �������� ���忡�� �ٽ� �����ֱ�.
        PhotonView interactablePV = selectInteractable.transform.GetComponent<PhotonView>();
        interactablePV.TransferOwnership(PhotonNetwork.MasterClient);

        // 2. ���� ��� �˸���
        _photonView.RPC(nameof(SyncSelect), RpcTarget.Others, interactablePV.ViewID, false);
    }

    [PunRPC]
    private void SyncSelect(int interactableID, bool isSelected)
    {
        PhotonView interactablePV =  PhotonView.Find(interactableID);
        Rigidbody interactableRb = interactablePV.GetComponent<Rigidbody>();
        if (interactablePV.GetComponent<TabletInteractable>() != null)
            return;

        IXRSelectInteractor interactor = _photonView.GetComponent<IXRSelectInteractor>();
        IXRSelectInteractable interactable = interactablePV.GetComponent<IXRSelectInteractable>();

        if (isSelected)
        {
            interactionManager.SelectEnter(interactor, interactable);

            interactableRb.useGravity = false;
            interactableRb.isKinematic = true;
        }
        else
        {
            interactionManager.SelectExit(interactor, interactable);

            interactableRb.useGravity = true;
            interactableRb.isKinematic = false;
        }
    }
}
