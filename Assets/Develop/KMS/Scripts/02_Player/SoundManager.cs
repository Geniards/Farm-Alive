using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    // BGM types
    public enum E_BGM
    {
        LOGIN,
        LOBBY,
        ROOM,
        GAME,
        SIZE_MAX
    }


    [Header("Audio Clips")]
    public AudioClip[] clipBgm;

    [System.Serializable]
    public class SFXInfo
    {
        public string key;
        public AudioClip clip;
    }

    [Header("Audio Source")]
    public AudioSource audioBgm;

    [Header("ȿ���� ���")]
    public SFXInfo[] sfxArr;

    private Dictionary<string, AudioClip> sfxDict = new Dictionary<string, AudioClip>();
    private Dictionary<int, AudioSource> playerVoices = new Dictionary<int, AudioSource>();
    private int localPlayerActorNumber;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // ���� �÷��̾��� ActorNumber ����
        localPlayerActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

        foreach (var info in sfxArr)
        {
            if (!sfxDict.ContainsKey(info.key))
                sfxDict.Add(info.key, info.clip);
            else
                Debug.LogWarning($"�ߺ��� SFX Ű �� : {info.key}");
        }
#if UNITY_EDITOR
        Debug.LogWarning($"ȿ���� �ʱ�ȭ �Ϸ�!");
#endif
    }

    // BGM ��� �� ���� ����
    public void PlayBGM(E_BGM bgmIdx)
    {
        audioBgm.clip = clipBgm[(int)bgmIdx];
        audioBgm.Play();
    }

    public void StopBGM()
    {
        if (audioBgm.isPlaying)
        {
            audioBgm.Stop();
        }
    }

    public void SetBGMVolume(float volume)
    {
        if (audioBgm.isPlaying)
        {
            audioBgm.volume = volume;
        }
    }

    // SFX ��� �� ���� ����
    public void PlaySFX(string key)
    {
        if (sfxDict.ContainsKey(key))
            AudioSource.PlayClipAtPoint(sfxDict[key], Vector3.zero);
        else
            Debug.LogWarning($"ȿ���� Ű��{key}�� ã�� �� �����ϴ�.");
    }

    public void SetSFXVolume(float volume)
    {
        foreach (var audioSource in playerVoices.Values)
        {
            audioSource.volume = volume;
        }
    }

    // �÷��̾� ���� ���
    public void RegisterPlayerVoice(PhotonView photonView, AudioSource voiceSource)
    {
        if (photonView != null)
        {
            int actorNumber = photonView.Owner.ActorNumber;

            if (!playerVoices.ContainsKey(actorNumber))
            {
                playerVoices[actorNumber] = voiceSource;
                Debug.Log($"SoundManager: Actor {actorNumber} ���� ���");
            }
        }
    }

    // �÷��̾� ���� ����
    public void UnregisterPlayerVoice(PhotonView photonView)
    {
        if (photonView != null)
        {
            int actorNumber = photonView.Owner.ActorNumber;

            if (playerVoices.ContainsKey(actorNumber))
            {
                playerVoices.Remove(actorNumber);
                Debug.Log($"SoundManager: Actor {actorNumber} ���� ����");
            }
        }
    }

    // Ư�� ĳ������ ���� ����
    public void SetPlayerVolume(int actorNumber, float volume)
    {
        if (playerVoices.ContainsKey(actorNumber))
        {
            playerVoices[actorNumber].volume = volume;
            Debug.Log($"Actor {actorNumber} ���� ����: {volume}");
        }
    }

    // ��� ĳ������ ������ ��ȯ (UI ������Ʈ�� ���)
    public Dictionary<int, AudioSource> GetAllPlayerVoices()
    {
        return new Dictionary<int, AudioSource>(playerVoices);
    }
}
