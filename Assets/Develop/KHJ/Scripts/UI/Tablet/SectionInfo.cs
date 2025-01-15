using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SectionInfo : UIBinder
{
    private string _weatherString = "Text_Weather";
    private string _blightString = "Text_Blight";
    private string _temperatureString = "Text_Temperature";

    private TextMeshProUGUI _weatherText;
    private TextMeshProUGUI _blightText;
    private TextMeshProUGUI _temperatureText;

    private void Start()
    {
        _weatherText = GetUI<TextMeshProUGUI>(_weatherString);
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
        _weatherText.text = "����: ";
        int activeWeather = 0;

        foreach (int activeID in EventManager.Instance._activeEventsID)
        {
            string eventName = CSVManager.Instance.Events[activeID].event_name;
            switch (eventName)
            {
                case "��ǳ":
                case "����":
                case "����":
                    _weatherText.text += $"{CSVManager.Instance.Events[activeID].event_name} ";
                    activeWeather++;
                    break;
                default:
                    break;
            }
        }

        if (activeWeather == 0)
            _weatherText.text = "����: ����";
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
