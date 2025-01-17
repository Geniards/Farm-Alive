using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ReturnToLobbyInteractable : XRGrabInteractable
{
    [Header("ī��Ʈ �ٿ�")]
    public float countdownTime = 3f;

    [Header("�ȳ��� �ؽ�Ʈ ������Ʈ")]
    public TMP_Text text;


    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private Transform player;

    protected override void Awake()
    {
        base.Awake();

        initialPosition = transform.position;
        initialRotation = transform.rotation;

        text.text = "�κ�� ���ư� �� ���� Select �ϼ���.";
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        SoundManager.Instance.PlaySFX("SFX_Lobby_CropSelected");
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
#if UNITY_EDITOR
        Debug.Log($"{args.interactableObject.transform.name}�� ���� �Ǿ����ϴ�.");
#endif
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
            Debug.LogWarning("pps�� �����ϴ�.");
    }
}
