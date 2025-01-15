using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class EnterTheGameInteractable : XRGrabInteractable
{
    [Header("�̵��� �� �̸�")]
    public string targetSceneName = "AssetScene";

    [Header("�ȳ��� �ؽ�Ʈ ������Ʈ")]
    public TMP_Text text;

    private PhotonView _photonView;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    protected override void Awake()
    {
        base.Awake();

        initialPosition = transform.position;
        initialRotation = transform.rotation;

        _photonView = GetComponent<PhotonView>();

        text.text = "���� �� ���� ���常 Select �ϼ���.";
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        Debug.Log("���ӽ��� ���� ���� �Ǿ����ϴ�.");

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("������ Ŭ���Ʈ ���ε尡 ���õǾ����ϴ�.");
            StartCoroutine(GameStartCountDown(5));
        }
        else
        {
            MessageDisplayManager.Instance.ShowMessage($"���常 ������ �����մϴ�.", 1f, 3f);
        }
    }

    private IEnumerator GameStartCountDown(float countdown)
    {
        float remainingTime = countdown;
        while (remainingTime > 0)
        {
            // �޽��� ����
            _photonView.RPC("DisplayMessageRPC", RpcTarget.All, $"{(int)remainingTime}");

            Debug.Log($"After {(int)remainingTime} seconds, you enter the room.");
            yield return new WaitForSeconds(1f);
            remainingTime--;
        }

        Debug.Log("���� �� �̵� ��...");
        SceneLoader.LoadNetworkSceneWithLoading(targetSceneName);
    }

    [PunRPC]
    public void DisplayMessageRPC(string message)
    {
        MessageDisplayManager.Instance.ShowMessage($"{message}�� ��, �������� ���� �մϴ�.", 1f, 3f);
    }


}
