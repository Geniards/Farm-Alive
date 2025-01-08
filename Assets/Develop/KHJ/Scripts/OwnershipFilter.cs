using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
    
public class OwnershipFilter : MonoBehaviour, IXRHoverFilter, IXRSelectFilter
{
    public bool canProcess { get { return isActiveAndEnabled; } }

    public bool Process(IXRHoverInteractor interactor, IXRHoverInteractable interactable)
    {
        return CheckOwnership(interactor, interactable);
    }

    public bool Process(IXRSelectInteractor interactor, IXRSelectInteractable interactable)
    {
        return CheckOwnership(interactor, interactable);
    }

    private bool CheckOwnership(IXRInteractor interactor, IXRInteractable interactable)
    {
        PhotonView interactorPV = FindPhotonView(interactor.transform);
        PhotonView interactablePV = FindPhotonView(interactable.transform);
        if (interactorPV == null)
        {
            Debug.LogWarning("Interactor���� PhotonView�� ã�� �� �����ϴ�.");
            return false;
        }
        if (interactablePV == null)
        {
            Debug.LogWarning("Interactable���� PhotonView�� ã�� �� �����ϴ�.");
            return false;
        }

        // ������ �˻�
        return (interactorPV.Owner == interactablePV.Owner);
    }

    private PhotonView FindPhotonView(Transform transform)
    {
        PhotonView photonView = null;
        Transform curTransform = transform;

        while (curTransform != null)
        {
            photonView = curTransform.GetComponent<PhotonView>();
            if (photonView != null)
                return photonView;

            curTransform = curTransform.parent;
        }
        return null;
    }
}
