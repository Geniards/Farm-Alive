using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using GameData;
using System;

/// <summary>
/// �ý��� ��ȹ�� �� ���� �� ���ÿ� �Ͼ�� �ʴ� ���� �߰� �ʿ�
/// �������� �Ŵ����� ���� ���� �޾ƿͼ� occurPlusPercent ���� �ʿ�
/// </summary>
public class EventManager : MonoBehaviour
{
    [Header("�̺�Ʈ ��� �ֱ�(��)")]
    [SerializeField] private float _checkInterval = 1.0f;

    public UnityEvent<EVENT> OnEventStarted;
    public UnityEvent<EVENT> OnEventEnded;

    // �̺�Ʈ �߻�Ȯ�� �׽�Ʈ�� ����
    [SerializeField] Dictionary<int, EVENT> events;

    // ���� ���� ���� �̺�Ʈ ���
    private List<int> _activeEventsID = new List<int>();


    private (int, int)[] _conflictPairs = new (int, int)[]
    {
        (421,441),
        (431,442),
    };

    private void Start()
    {
        StartCoroutine(EventRoutine());
    }

    private IEnumerator EventRoutine()
    {
        // CSV �ٿ�ε尡 ���� ������ ���
        while (!CSVManager.Instance.downloadCheck)
            yield return null;

        events = CSVManager.Instance.Events;


        var eventDict = CSVManager.Instance.Events;
        var seasonDict = CSVManager.Instance.Events_Seasons;

        while (true)
        {
            yield return new WaitForSeconds(_checkInterval);

            // ��÷�� �̺�Ʈ ����Ʈ
            List<int> triggered = new List<int>();

            int seasonID = StageManager.Instance.WeatherID;

            foreach (var kv in eventDict)
            {
                int eventID = kv.Key;
                EVENT ev = kv.Value;

                // �̹� ���� ���� ���� �̺�Ʈ�� ��ŵ
                if (_activeEventsID.Contains(eventID))
                    continue;

                // �⺻ Ȯ��
                float finalRate = ev.event_occurPercent;

                // �̺�Ʈ�� ���� �����̸�,
                if (CheckSeasonMatch(eventID, seasonID, seasonDict))
                {
                    finalRate += ev.event_occurPlusPercent;
                }

                // Ȯ�� �˻�
                if (ProbabilityHelper.Draw(finalRate))
                {
                    triggered.Add(eventID);
                }
            }

            if (triggered.Count == 0)
                continue;

            ResolveConflicts(triggered);

            // ������ �̺�Ʈ ��� ���� �߻� ����
            foreach (var evID in triggered)
            {
                StartEvent(evID);
            }
        }
    }

    // ���� Ȯ��, true��ȯ
    private bool CheckSeasonMatch(int eventID, int seasonID, Dictionary<int, EVENT_SEASON> seasonDict)
    {
        if (!seasonDict.ContainsKey(eventID))
            return false;

        EVENT_SEASON es = seasonDict[eventID];
        for (int i = 0; i < es.event_seasonID.Length; i++)
        {
            if (es.event_seasonID[i] == seasonID)
                return true;
        }

        return false;
    }

    private void ResolveConflicts(List<int> triggered)
    {
        foreach (var pair in _conflictPairs)
        {
            // ���ÿ� �Ͼ�� �ȵǴ� ���� ���ÿ� �����ϴ��� Ȯ��
            int AeventID = pair.Item1;
            int BeventID = pair.Item2;
            bool ifHasA = triggered.Contains(AeventID);
            bool ifHasB = triggered.Contains(BeventID);

            // ���ÿ� �����ϸ� �ϳ� ����
            if (ifHasA && ifHasB)
            {
                int r = UnityEngine.Random.Range(0, 2);
                if ( r == 0)
                {
                    triggered.Remove(BeventID);
                }
                else
                {
                    triggered.Remove(AeventID);
                }
            }
        }
    }

    private void StartEvent(int eventID)
    {
        var eventDict = CSVManager.Instance.Events;
 
        EVENT evData = eventDict[eventID];

        _activeEventsID.Add(eventID);

        OnEventStarted?.Invoke(evData);

        // �ڵ� ����
        if (evData.event_continueTime > 0)
        {
            StartCoroutine(AutoEndRoutine(eventID, evData.event_continueTime));
        }
    }

    private IEnumerator AutoEndRoutine(int eventID, float dur)
    {
        yield return new WaitForSeconds(dur);
        ResolveEvent(eventID);
    }

    /// <summary>
    /// �̺�Ʈ ���� ���� �޼��� ȣ�� ��Ź�帳�ϴ�
    /// </summary>
    public void ResolveEvent(int eventID)
    {
        if (_activeEventsID.Remove(eventID))
        {
            EVENT evDATA = CSVManager.Instance.Events[eventID];

            OnEventEnded?.Invoke(evDATA);
        }
    }
}