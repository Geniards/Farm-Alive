using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameData;

public class StageManager : MonoBehaviour
{
    [SerializeField] int _curStageID = 511;
    [SerializeField] TimeManager _timeManager;
    [SerializeField] EventManager _eventManager;

    private void Start()
    {
        StartCoroutine(SetupStageRoutine());

        _timeManager = GetComponent<TimeManager>();
        _eventManager = GetComponent<EventManager>();
    }

    private IEnumerator SetupStageRoutine()
    {
        // csv �ٿ�ε尡 �Ϸ�ɶ����� ��ٸ���.
        while (!CSVManager.Instance.downloadCheck)
        {
            yield return null;
        }

        _curStageID = PunManager.Instance.selectedStage;

        // ��������, ����Ʈ���� �ش� �������� ���� ��������.
        var stages = CSVManager.Instance.Stages;
        STAGE stageData = new STAGE();
        for (int i = 0; i < stages.Count; i++)
        {
            if (stages[i].stage_ID == _curStageID)
            {
                stageData = stages[i];
                break;
            }
        }

        yield return 10f;

        _timeManager.SetTimeLimit(360f);
        _timeManager.StartStageTimer();

        Debug.Log($"���� ��������={stageData.stage_ID}, ����={stageData.stage_seasonID}");

        //TODO : ������ ���� ����.
    }
}