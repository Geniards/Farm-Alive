using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class EnterTheGameInteractable : XRGrabInteractable
{
    [Header("�̵��� �� �̸�")]
    public string targetSceneName = "AssetScene";

    private PhotonView _photonView;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    protected override void Awake()
    {
        base.Awake();

        initialPosition = transform.position;
        initialRotation = transform.rotation;

        _photonView = GetComponent<PhotonView>();
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        Debug.Log("���ӽ��� ���� ���� �Ǿ����ϴ�.");

        if(PhotonNetwork.IsMasterClient)
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
