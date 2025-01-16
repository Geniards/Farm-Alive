using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NpcTextView : MonoBehaviour
{
    [SerializeField] GameObject textPanel;
    [SerializeField] public Text text;
    private float outputTime = 5f;
    private float outputSpeed = 0.05f;
    [SerializeField] Animator anim;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] soundList;

    private Coroutine textCoroutine;

    public void NpcText()
    {
        //text.text = "�ź��� ������� �͵�\n�̰ź��ٴ� �����ھ�!!";
        ShowText("�ź��� ������� �͵�\n�̰ź��ٴ� �����ھ�!!");
        anim.SetBool("isField", true);
        SoundManager.Instance.PlaySFX("SFX_NPCFail");
    }

    public void NpcText(bool check)
    {
        if (check)
        {
            //text.text = "�Ϻ��ϱ�!!\n �������� �� ��Ź�ϰڳ�!";
            ShowText("�Ϻ��ϱ�!!\n �������� �� ��Ź�ϰڳ�!");
            anim.SetBool("isSuccess", true);
            SoundManager.Instance.PlaySFX("SFX_NPCSuccess");
        }
        else
        {
            //text.text = "���� ���ϴ� �۹��� �ƴ��ݾ�!!\n���� ��� �ϴ°ž�!";
            ShowText("���� ���ϴ� �۹��� �ƴ��ݾ�!!\n���� ��� �ϴ°ž�!");
            anim.SetBool("isField", true);
            SoundManager.Instance.PlaySFX("SFX_NPCWrongCrop");
        }
    }

    public void NpcText(int count)
    {
        if (count > 0)
        {
            //text.text = "�� �ֹ����� ���� �۹��� �����ִµ�\n�̰� �����ΰ�?\n ���� �� �ްڳ�";
            ShowText("�� �ֹ����� ���� �۹��� �����ִµ�\n�̰� �����ΰ�?\n ���� �� �ްڳ�");
            anim.SetBool("isSuccess", true);
            SoundManager.Instance.PlaySFX("SFX_NPCManyCrop");
        }

        if (count <= 0)
        {
            //text.text = "����\n���� �۹��鵵 ������ ��Ź�ϰڳ�!";
            ShowText("����\n���� �۹��鵵 ������ ��Ź�ϰڳ�!");
            SoundManager.Instance.PlaySFX("SFX_NPCCorrect");
        }
    }

    private void ShowText(string npcText)
    {
        if (textCoroutine != null)
        {
            StopCoroutine(textCoroutine);
        }

        textPanel.SetActive(true);
        textCoroutine = StartCoroutine(TypingText(npcText));
        StartCoroutine(ShowPanelTime(outputTime));
    }

    private IEnumerator TypingText(string npcText)
    {
        text.text = "";
        foreach (char textcul in npcText)
        {
            text.text += textcul;
            yield return new WaitForSeconds(outputSpeed);
        }
    }

    private IEnumerator ShowPanelTime(float outputTiem)
    {
        yield return new WaitForSeconds(outputTiem);

        anim.SetBool("isSuccess", false);
        anim.SetBool("isField", false);
        textPanel.SetActive(false);
    }
}