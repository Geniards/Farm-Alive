using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NpcTextView : MonoBehaviour
{
    [SerializeField] Transform myCamera;
    [SerializeField] public Text text;

    private void OnEnable()
    {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            PhotonView photonView = player.GetComponent<PhotonView>();

            if (photonView != null && photonView.IsMine)
            {
                myCamera = player.transform.Find("Main Camera");
                break;
            }
        }
    }
    private void Update()
    {
        if (myCamera != null)
        {
            this.transform.LookAt(myCamera);
        }
    }

    public void NpcText()
    {
        text.text = "���� ���ϴ� �۹��� �ƴ��ݾ�!!\n���� ��� �ϴ°ž�!";
    }

    public void NpcText(int count)
    {
        if (count > 0)
            text.text = "�� �ֹ����� ���� �۹��� �����ִµ�\n�̰� �����ΰ�?\n ���� �� �ްڳ�";

        if (count <= 0)
            text.text = "�Ϻ��ϱ�!!\n�������� �� ��Ź�ϰڳ�!";
    }
}