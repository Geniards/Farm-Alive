using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.XR;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    // BGM types
    [System.Serializable]
    public class BGMInfo
    {
        [Tooltip("BGM�� ������ Ű")]
        public string key;
        [Tooltip("BGM AudioClip")]
        public AudioClip clip;
    }

    [System.Serializable]
    public class SFXInfo
    {
        [Tooltip("SFX�� ������ Ű")]
        public string key;
        [Tooltip("SFX AudioClip")]
        public AudioClip clip;
    }

    [Header("BGM ����")]
    [Tooltip("BGM ���� �迭")]
    public BGMInfo[] bgmInfo;

    [Header("SFX ����")]
    [Tooltip("SFX ���� �迭")]
    public SFXInfo[] sfxInfo;

    [Header("BGM Audio Source")]
    public AudioSource bgm;
    public AudioSource sfx;

    private Dictionary<string, AudioClip> _bgmDict = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> _sfxDict = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// ���ϴ� BGM�� ����մϴ�.
    /// </summary>
    /// <param name="key">����� ���ϴ� BGM</param>
    public void PlayBGM(string key)
    {
        if (!_bgmDict.ContainsKey(key))
        {
            Debug.LogWarning($"BGM '{key}'�� �����ϴ�!");
            return;
        }

        bgm.clip = _bgmDict[key];
        bgm.Play();
    }

    /// <summary>
    /// ��� ���� BGM�� �����մϴ�.
    /// </summary>
    public void StopBGM()
    {
        bgm.Stop();
    }


    private void Start()
    {
        // bgm ��ųʸ� �ʱ�ȭ
        foreach (var sfx in bgmInfo)
        {
            _bgmDict.Add(sfx.key, sfx.clip);
        }

        Debug.Log("BGM ��ųʸ� �ʱ�ȭ �Ϸ�");

        // SFX ��ųʸ� �ʱ�ȭ
        foreach (var sfx in sfxInfo)
        {
            _sfxDict.Add(sfx.key, sfx.clip);
        }

        Debug.Log("SFX ��ųʸ� �ʱ�ȭ �Ϸ�");
    }

    /// <summary>
    /// SFX ���
    /// </summary>
    /// <param name="key">����� SFX�� Ű</param>
    /// <param name="volumeScale">���� ũ��</param>
    public void PlaySFX(string key, float volumeScale = 1f)
    {
        if (!_sfxDict.ContainsKey(key))
        {
            Debug.LogWarning($"SFX '{key}'�� �����ϴ�!");
            return;
        }

        AudioClip clip = _sfxDict[key];
        sfx.PlayOneShot(clip, volumeScale);
    }

    /// <summary>
    /// SFX ��� �� duration �ð� �ڿ� ����
    /// </summary>
    /// <param name="key">����� SFX�� Ű</param>
    /// <param name="duration">���� �ð�</param>
    /// <param name="volumeScale">���� ũ��</param>
    public void PlaySFX(string key, float duration, float volumeScale = 1f)
    {
        if (!_sfxDict.ContainsKey(key))
        {
            Debug.LogWarning($"SFX '{key}'�� �����ϴ�!");
            return;
        }

        AudioClip clip = _sfxDict[key];
        sfx.PlayOneShot(clip, volumeScale);

        if (duration > 0)
        {
            StartCoroutine(StopSFXAfter(duration));
        }
    }

    /// <summary>
    /// ��� SFX�� �����մϴ�.
    /// </summary>
    public void StopAllSFX()
    {
        sfx.Stop();
    }

    private IEnumerator StopSFXAfter(float duration)
    {
        yield return new WaitForSeconds(duration);
        sfx.Stop();
    }
}
