using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NpcTextView : MonoBehaviour
{
    [SerializeField] public Text text;

    public void NpcText()
    {
            text.text = "�ź��� ������� �͵�\n�̰ź��ٴ� �����ھ�!!";
    }

    public void NpcText(bool check)
    {
        if (check)
        {
            text.text = "�Ϻ��ϱ�!!\n �������� �� ��Ź�ϰڳ�!";
        }
        else
        {
            text.text = "���� ���ϴ� �۹��� �ƴ��ݾ�!!\n���� ��� �ϴ°ž�!";
        }
    }

    public void NpcText(int count)
    {
        if (count > 0)
            text.text = "�� �ֹ����� ���� �۹��� �����ִµ�\n�̰� �����ΰ�?\n ���� �� �ްڳ�";

        if (count <= 0)
            text.text = "����\n���� �۹��鵵 ������ ��Ź�ϰڳ�!";
    }
}