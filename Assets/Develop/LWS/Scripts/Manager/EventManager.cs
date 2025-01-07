using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using GameData;

public class EventManager : MonoBehaviour
{
    [Header("�̺�Ʈ ��� �ֱ�")]
    [SerializeField] float checkInterval = 1.0f;

    // �̺�Ʈ�� ���� ���ΰ�
    private bool _isEventPlaying = false;

    [Header("�̺�Ʈ �߻�/���� �˸�")]
    [SerializeField] UnityEvent onEventStarted;
    [SerializeField] UnityEvent onEventEnded;

    private void Start()
    {
        StartCoroutine(EventRoutine());
    }

    private IEnumerator EventRoutine()
    {
        // csv �ٿ�ε尡 �Ϸ�ɶ����� ��ٸ���.
        while (!CSVManager.Instance.downloadCheck)
        {
            yield return null;
        }

        List<EVENT> events = CSVManager.Instance.Events;

        while (true)
        {
            yield return new WaitForSeconds(checkInterval);

            // �̺�Ʈ �������̸� �н�
            if (_isEventPlaying)
                continue;

            float sumWeight = 0f;
            foreach(var ev in events)
            {
                float finalChance = ev.event_occurPercent + ev.event_occurPlusPercent;
                sumWeight += finalChance;
            }

            // 0�ۼ�Ʈ�� �н�
            if (sumWeight <= 0f)
                continue;

            float rand = Random.value * sumWeight;

            float cumulative = 0f;
            EVENT selectedevent = default;
            bool found = false;

            foreach (var ev in events)
            {
                float finalChance = ev.event_occurPercent + ev.event_occurPlusPercent;
                cumulative += finalChance;
                if (rand <= cumulative)
                {
                    selectedevent = ev;
                    found = true;
                    break;
                }
            }

            if(found)
            {
                // TODO : �̺�Ʈ ����
            }
        }
    }

    // TODO : �̺�Ʈ ���� �޼���

    // TODO : �ذ� �޼���

    // TODO : �̺�Ʈ �ذ� �˸���
}
