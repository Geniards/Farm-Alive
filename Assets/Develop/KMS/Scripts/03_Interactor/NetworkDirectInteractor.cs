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

        IXRSelectInteractable selectInteractable = args.interactableObject;

        // 1. ���� �÷��̾ ���� ��ü�� �������� ��������.
        PhotonView interactablePV = selectInteractable.transform.GetComponent<PhotonView>();
        interactablePV.RequestOwnership();

        // 2. ���� ��� �˸���
        photonView.RPC(nameof(SyncSelect), RpcTarget.Others, selectInteractable, true);
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
        photonView.RPC(nameof(SyncSelect), RpcTarget.Others, selectInteractable, true);
    }

    [PunRPC]
    private void SyncSelect(IXRSelectInteractable interactable, bool isSelected)
    {
        Rigidbody interactableRigid = interactable.transform.GetComponent<Rigidbody>();

        if (isSelected)
        {
            interactableRigid.useGravity = false;
            interactableRigid.isKinematic = true;
        }
        else
        {
            interactableRigid.useGravity = true;
            interactableRigid.isKinematic = false;
        }
    }
}
