using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterTheGame : MonoBehaviourPun
{
    [Header("�̵��� �� �̸�")]
    public string targetSceneName = "AssetScene";

    public void OnSelectEnter()
    {
        Debug.Log("���ӽ��� ���� ���� �Ǿ����ϴ�.");

        if(PhotonNetwork.IsMasterClient)
        {
            Debug.Log("������ Ŭ���Ʈ ���ε尡 ���õǾ����ϴ�.");
            StartCoroutine(GameStartCountDown(5));
        }
    }

    private IEnumerator GameStartCountDown(float countdown)
    {
        float remainingTime = countdown;
        while (remainingTime > 0)
        {
            // �޽��� ����
            photonView.RPC("DisplayMessageRPC", RpcTarget.All, $"{(int)remainingTime}");

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
