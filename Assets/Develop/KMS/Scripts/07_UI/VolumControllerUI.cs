using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumControllerUI : MonoBehaviour
{
    public Slider bgmVolumeSlider;
    public Slider sfxVolumeSlider;
    public GameObject playerVolumePrefab;
    public Transform playerVolumeContainer;

    private Dictionary<int, Slider> playerVolumeSliders = new Dictionary<int, Slider>();

    private void Start()
    {
        // BGM �� SFX �����̴� ����
        if (bgmVolumeSlider != null)
            bgmVolumeSlider.onValueChanged.AddListener(OnBGMVolumeChanged);

        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        UpdatePlayerVolumeUI();
    }

    private void OnDestroy()
    {
        if (bgmVolumeSlider != null)
            bgmVolumeSlider.onValueChanged.RemoveListener(OnBGMVolumeChanged);

        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.RemoveListener(OnSFXVolumeChanged);
    }

    private void OnBGMVolumeChanged(float value)
    {
        SoundManager.Instance.SetBGMVolume(value);
    }

    private void OnSFXVolumeChanged(float value)
    {
        SoundManager.Instance.SetSFXVolume(value);
    }

    // ĳ���ͺ� ���� UI ������Ʈ
    public void UpdatePlayerVolumeUI()
    {
        var playerVoices = SoundManager.Instance.GetAllPlayerVoices();

        // ���� �����̴� ����
        foreach (var slider in playerVolumeSliders.Values)
        {
            Destroy(slider.gameObject);
        }
        playerVolumeSliders.Clear();

        // �� �����̴� ����
        foreach (var kvp in playerVoices)
        {
            int actorNumber = kvp.Key;
            AudioSource voiceSource = kvp.Value;

            GameObject sliderObj = Instantiate(playerVolumePrefab, playerVolumeContainer);
            Slider slider = sliderObj.GetComponent<Slider>();

            slider.value = voiceSource.volume;
            slider.onValueChanged.AddListener((value) =>
            {
                SoundManager.Instance.SetPlayerVolume(actorNumber, value);
            });

            // �г��� ǥ��
            var photonView = voiceSource.GetComponentInParent<PhotonView>();
            string playerName = photonView != null ? photonView.Owner.NickName : $"Player {actorNumber}";

            Text label = sliderObj.GetComponentInChildren<Text>();
            if (label != null)
            {
                label.text = playerName;
            }

            Debug.Log("���");

            playerVolumeSliders[actorNumber] = slider;
        }
    }
}
