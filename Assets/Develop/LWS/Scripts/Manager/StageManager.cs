using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameData;
using UnityEngine.Events;

public class StageManager : MonoBehaviour
{
    [SerializeField] int _curStageID;

    [Header("�������� �ð� �Ӽ�")]
    [SerializeField] float _stageTimeLimit = 0;
    [SerializeField] float _curStageTime = 0;
    [SerializeField] bool _isTimerRunning = false;

    public int WeatherID => _curStageData.stage_seasonID;
    private int _weatherID;

    private STAGE _curStageData;

    private int _maxSymptomObject;
    private int _curSymptomObject;
    public int CurSymptomObject { get { return _curSymptomObject; } set { _curSymptomObject = value; } }

    private void Awake()
    {
        _curStageID = PunManager.Instance.selectedStage;
    }

    private IEnumerator Start()
    {
        // CSV �ٿ�ε� ���� ������ ���
        while (!CSVManager.Instance.downloadCheck)
            yield return null;

        List<STAGE> stages = CSVManager.Instance.Stages;
        STAGE foundStage = default;
        bool found = false;
        for (int i = 0; i < stages.Count; i++)
        {
            if (stages[i].stage_ID == _curStageID)
            {
                foundStage = stages[i];
                found = true;
                break;
            }
        }
        if (!found)
        {
            Debug.LogError($"StageManager: Stage ID {_curStageID} not found!");
            yield break;
        }

        // currentStageData�� ����
        _curStageData = foundStage;
        
        _stageTimeLimit = 360f;
        _weatherID = _curStageData.stage_seasonID;
        _maxSymptomObject = _curStageData.stage_allowSymptomFacilityCount;

        // 2) �������� Ÿ�̸� ����
        StartStageTimer();

        Debug.Log($"[StageManager] ���� ��������={foundStage.stage_ID}, ����(seasonID)={foundStage.stage_seasonID}");
    }

    private void Update()
    {
        if (!_isTimerRunning)
            return;

        float delta = Time.deltaTime;
        _curStageTime += Time.deltaTime;


        if (_stageTimeLimit > 0 && _curStageTime >= _stageTimeLimit)
        {
            EndStage();
        }
    }

    public void StartStageTimer()
    {
        _curStageTime = 0f;
        _isTimerRunning = true;
    }
    public void EndStage()
    {
        _isTimerRunning=false;
        // todo : ���� ���̾�̽� ���� ���� �� ��ȯ
    }

    public int GetWeatherID()
    { 
       return _weatherID;
    }
}