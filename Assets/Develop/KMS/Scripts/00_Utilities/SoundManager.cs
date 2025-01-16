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
    public enum E_BGM
    {
        GAMESTART,
        LOBBY,
        ROOM,
        INGAME,
        LOADING,

        SIZEMAX
    }

    [System.Serializable]
    public class SFXInfo
    {
        [Tooltip("SFX�� ������ Ű")]
        public string key;
        [Tooltip("SFX AudioClip")]
        public AudioClip clip;
    }

    [Header("BGM Clips")]
    public AudioClip[] bgmClips;

    [Header("SFX ����")]
    [Tooltip("SFX ���� �迭")]
    public SFXInfo[] _sfxInfo;

    [Header("BGM Audio Source")]
    public AudioSource bgm;
    public AudioSource sfx;

    private Dictionary<string, AudioClip> _sfxDict = new Dictionary<string, AudioClip>();

    //[Header("Audio Setting")]
    //public AudioMixer test; // ����� �ͼ�
    //public GameObject sliderUI; // �����̴� UI �г�
    //public XRNode rightControllerNode = XRNode.RightHand; // ������ ��Ʈ�ѷ�

    //private float maxVolumeDb = 20f;
    //private float minVolumeDb = -80f;
    //private float buttonHoldTime = 1f; // ��ư�� ������ �ϴ� �ð�
    //private float buttonHoldCounter = 0f; // ��ư ���� �ð� ����
    //private bool isSliderUIActive = false; // �����̴� UI Ȱ��ȭ ����

    //private Transform _mainCamera; // ���� ī�޶��� Transform
    //public float distanceFromCamera = 3f; // ī�޶󿡼� �����̴� UI�� ������ �Ÿ�

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
    /// <param name="bgmIdx">����� ���ϴ� BGM</param>
    public void PlayBGM(E_BGM bgmIdx)
    {
        bgm.clip = bgmClips[(int)bgmIdx];
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
        //_mainCamera = Camera.main.transform;

        //if (!_mainCamera)
        //{
        //    Debug.LogError("Main Camera�� ã�� �� �����ϴ�!");
        //}

        //if (sliderUI)
        //{
        //    sliderUI.SetActive(false);
        //}

        // SFX ��ųʸ� �ʱ�ȭ
        foreach (var sfx in _sfxInfo)
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

//    public void SetBGMVolume(float volume)
//    {
//        float dBValue = Mathf.Lerp(minVolumeDb, maxVolumeDb, volume);
//        test.SetFloat("PlayerVoiceVolum", dBValue);
//    }

//    private void Update()
//    {
//        if (IsControllerButtonPressed(rightControllerNode, CommonUsages.primaryButton))
//        {
//            buttonHoldCounter += Time.deltaTime;

//            if (buttonHoldCounter >= buttonHoldTime)
//            {
//                ToggleSliderUI();
//                buttonHoldCounter = 0f;
//            }
//        }
//#if UNITY_EDITOR
//        else if (Input.GetKey(KeyCode.Slash))
//        {
//            buttonHoldCounter += Time.deltaTime;

//            if (buttonHoldCounter >= buttonHoldTime)
//            {
//                ToggleSliderUI();
//                buttonHoldCounter = 0f;
//            }
//        }
//#endif
//        else
//        {
//            buttonHoldCounter = 0f;
//        }

//        if (isSliderUIActive && sliderUI != null && _mainCamera != null)
//        {
//            UpdateSliderUIPosition();
//        }
//    }

//    private void ToggleSliderUI()
//    {
//        isSliderUIActive = !isSliderUIActive;

//        if (sliderUI != null)
//        {
//            sliderUI.SetActive(isSliderUIActive);

//            if (isSliderUIActive)
//            {
//                UpdateSliderUIPosition();
//            }
//        }
//    }

//    private void UpdateSliderUIPosition()
//    {
//        Vector3 newPosition = _mainCamera.position + _mainCamera.forward * distanceFromCamera;
//        sliderUI.transform.position = newPosition;
//        sliderUI.transform.rotation = Quaternion.LookRotation(sliderUI.transform.position - _mainCamera.position);
//    }

//    private bool IsControllerButtonPressed(XRNode controllerNode, InputFeatureUsage<bool> button)
//    {
//        // ��Ʈ�ѷ� ��忡�� ��ư �Է� ���� Ȯ��
//        InputDevice device = InputDevices.GetDeviceAtXRNode(controllerNode);
//        if (device.isValid && device.TryGetFeatureValue(button, out bool isPressed))
//        {
//            return isPressed;
//        }
//        return false;
//    }
}
