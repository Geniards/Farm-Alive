using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SectionInfo : UIBinder
{
    private string _wheatherString = "Text_Wheater";
    private string _blightString = "Text_Blight";
    private string _temperatureString = "Text_Temperature";

    private TextMeshProUGUI _wheatherText;
    private TextMeshProUGUI _blightText;
    private TextMeshProUGUI _temperatureText;

    private void Start()
    {
        _wheatherText = GetUI<TextMeshProUGUI>(_wheatherString);
        _blightText = GetUI<TextMeshProUGUI>(_blightString);
        _temperatureText = GetUI<TextMeshProUGUI>(_temperatureString);

        EventManager.Instance.OnEventStarted.AddListener(OnWeatherRaised);
        EventManager.Instance.OnEventStarted.AddListener(OnBlightRaised);
        EventManager.Instance.OnEventStarted.AddListener(OnTemperatureRaised);

        EventManager.Instance.OnEventEnded.AddListener(OnWheatherStopped);
        EventManager.Instance.OnEventEnded.AddListener(OnBlightStopped);
        EventManager.Instance.OnEventEnded.AddListener(OnTemperatureStopped);
    }

    private void OnWeatherRaised(GameData.EVENT eventData)
    {
        switch (eventData.event_name)
        {
            case "��ǳ":
            case "����":
            case "����":
                UpdateWeatherText();
                break;
            default:
                break;
        }
    }

    private void OnWheatherStopped(GameData.EVENT eventData)
    {
        switch (eventData.event_name)
        {
            case "��ǳ":
            case "����":
            case "����":
                UpdateWeatherText();
                break;
            default:
                break;
        }
    }

    private void UpdateWeatherText()
    {
        _wheatherText.text = "����: ";
        foreach (int activeID in EventManager.Instance._activeEventsID)
        {
            _wheatherText.text += $"{CSVManager.Instance.Events[activeID].event_name} ";
        }

    }

    private void OnBlightRaised(GameData.EVENT eventData)
    {
        if (eventData.event_name != "������")
            return;

        _blightText.text = $"����: �߻�";
    }

    private void OnBlightStopped(GameData.EVENT eventData)
    {
        if (eventData.event_name != "������")
            return;

        _blightText.text = $"����: ����";
    }

    private void OnTemperatureRaised(GameData.EVENT eventData)
    {
        switch (eventData.event_name)
        {
            case "�µ� ���":
                _temperatureText.text = $"���: ����";
                break;
            case "�µ� �ϰ�":
                _temperatureText.text = $"���: ����";
                break;
            default:
                break;
        }
    }

    private void OnTemperatureStopped(GameData.EVENT eventData)
    {
        switch (eventData.event_name)
        {
            case "�µ� ���":
            case "�µ� �ϰ�":
                _temperatureText.text = $"���: ����";
                break;
            default:
                break;
        }
    }
}
