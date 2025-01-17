using Photon.Pun;
using Photon.Voice.PUN;
using UnityEngine;

public class VoiceIconController : MonoBehaviourPun
{
    public GameObject speakingIcon;  // ���� �� ǥ�õ� ������
    public GameObject listeningIcon; // ���� �� ǥ�õ� ������

    private PhotonVoiceView _photonVoiceView;

    void Start()
    {
        _photonVoiceView = GetComponent<PhotonVoiceView>();
        if (_photonVoiceView == null)
        {
            Debug.LogError("PhotonVoiceView�� �������� �ʾҽ��ϴ�.");
            return;
        }

        if (speakingIcon) speakingIcon.SetActive(false);
        if (listeningIcon) listeningIcon.SetActive(false);
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            if (_photonVoiceView.IsRecording)
            {
                // ���� �÷��̾ ���� �ϰ� ���� �� speakingIcon Ȱ��ȭ
                SetIconState(speakingIcon, true);
            }
            else
            {
                // ���� ������ �� speakingIcon ��Ȱ��ȭ
                SetIconState(speakingIcon, false);
            }
        }
        else
        {
            if (_photonVoiceView.IsSpeaking)
            {
                // �ٸ� �÷��̾��� ������ ��� ���� �� listeningIcon Ȱ��ȭ
                SetIconState(listeningIcon, true);
            }
            else
            {
                // ��� ���°� �ƴϸ� listeningIcon ��Ȱ��ȭ
                SetIconState(listeningIcon, false);
            }
        }
    }

    // ������ Ȱ��ȭ/��Ȱ��ȭ ó�� �޼���
    private void SetIconState(GameObject icon, bool state)
    {
        if (icon && icon.activeSelf != state)
        {
            icon.SetActive(state);
        }
    }
}