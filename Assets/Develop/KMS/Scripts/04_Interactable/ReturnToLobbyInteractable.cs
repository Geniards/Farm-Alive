using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ReturnToLobbyInteractable : XRGrabInteractable
{
    public float countdownTime = 3f;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private Transform player;

    protected override void Awake()
    {
        base.Awake();

        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
#if UNITY_EDITOR
        Debug.Log($"{args.interactableObject.transform.name}�� ���� �Ǿ����ϴ�.");
#endif
        var player = args.interactorObject.transform;
        var playerPhotonView = player.GetComponentInParent<PhotonView>();

        if (playerPhotonView != null)
        {
            Debug.Log($"�̺�Ʈ�� ���۽�Ų �÷��̾�: {playerPhotonView.Owner.NickName} (ID: {playerPhotonView.Owner.ActorNumber})");
        }
        else
        {
            Debug.LogWarning("PhotonView�� ã�� �� �����ϴ�. ���� �÷��̾ �ƴ� �� �ֽ��ϴ�.");
        }

        StartCoroutine(ReturnToLobby(args.interactableObject.transform.gameObject, countdownTime));
    }

    private IEnumerator ReturnToLobby(GameObject targetObject, float countdown)
    {
        float remainingTime = countdown;

        if (targetObject)
        {
            transform.position = initialPosition;
            transform.rotation = initialRotation;
        }

        while (remainingTime > 0)
        {
            MessageDisplayManager.Instance.ShowMessage($"{(int)remainingTime} �� �� , �κ�� �̵� �մϴ�..", 1f, 3f);
            yield return new WaitForSeconds(1f);
            remainingTime--;
        }

        PunPlayerSpawn pps = FindObjectOfType<PunPlayerSpawn>();
        if (pps)
            pps.ReturnToFusion();
        else
            Debug.LogWarning("pps�� ����!");
    }
}
