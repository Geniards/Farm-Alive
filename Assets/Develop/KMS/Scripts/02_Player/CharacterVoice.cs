using UnityEngine;
using Photon.Pun;
using Photon.Voice.Unity;
using Photon.Voice.PUN;

public class CharacterVoice : MonoBehaviour
{
    private PhotonView _photonView;

    private void Start()
    {
        _photonView = GetComponent<PhotonView>();

        // Speaker ã�� �� ���
        var speaker = GetComponentInChildren<Speaker>();
        if (speaker != null)
        {
            var audioSource = speaker.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                SoundManager.Instance.RegisterPlayerVoice(_photonView, audioSource);
            }
            else
            {
                Debug.LogWarning("Speaker�� AudioSource�� �����ϴ�!");
            }
        }
        else
        {
            Debug.LogWarning("Speaker�� �� ĳ���Ϳ� �������� �ʽ��ϴ�!");
        }
    }

    private void OnDestroy()
    {
        if (_photonView != null)
        {
            SoundManager.Instance.UnregisterPlayerVoice(_photonView);
        }
    }
}
