using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using GameData;
using Photon.Pun;

public class EventManager : MonoBehaviourPunCallbacks
{
    [Header("�̺�Ʈ ��� �ֱ�(��)")]
    [SerializeField] private float _checkInterval = 1.0f;

    public UnityEvent<EVENT> OnEventStarted;
    public UnityEvent<EVENT> OnEventEnded;

    // ���� ���� ���� �̺�Ʈ ���
    public List<int> _activeEventsID = new List<int>(); 

    private (int, int)[] _conflictPairs = new (int, int)[]
    {
        (421,441),
        (431,442),
    };

    public static EventManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(EventRoutine());
        }
    }

    #region test
    private void Update()
    {
        int[] testEvCode = new int[1];

        if (PhotonNetwork.IsMasterClient)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                testEvCode[0] = 411;
                photonView.RPC(nameof(RPC_StartEvents), RpcTarget.All, testEvCode);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                testEvCode[0] = 421;
                photonView.RPC(nameof(RPC_StartEvents), RpcTarget.All, testEvCode);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                testEvCode[0] = 431;
                photonView.RPC(nameof(RPC_StartEvents), RpcTarget.All, testEvCode);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                testEvCode[0] = 432;
                photonView.RPC(nameof(RPC_StartEvents), RpcTarget.All, testEvCode);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                testEvCode[0] = 441;
                photonView.RPC(nameof(RPC_StartEvents), RpcTarget.All, testEvCode);
            }
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                testEvCode[0] = 442;
                photonView.RPC(nameof(RPC_StartEvents), RpcTarget.All, testEvCode);
            }
        }
    }
    #endregion

    private IEnumerator EventRoutine()
    {
        // CSV �ٿ�ε尡 ���� ������ ���
        while (!CSVManager.Instance.downloadCheck)
            yield return null;

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

            photonView.RPC(nameof(RPC_StartEvents), RpcTarget.All, triggered.ToArray());
        }
    }

    [PunRPC]
    private void RPC_StartEvents(int[] eventIDs)
    {
        foreach (int eID in eventIDs)
        {
            StartEvent(eID);
        }
    }

    private void StartEvent(int eventID)
    {
        var eventDict = CSVManager.Instance.Events;

        Debug.Log($"{eventDict[eventID].event_name} �߻�");
        if (!_activeEventsID.Contains(eventID))
        {
            _activeEventsID.Add(eventID);

            EVENT evData = eventDict[eventID];

            OnEventStarted?.Invoke(evData);
            
            ParticleManager.Instance.PlayParticle(eventID.ToString(), evData.event_continueTime);

            // MessageDisplayManager.Instance.ShowMessage(eventDict[eventID].event_name);

            // �ڵ� ����
            if (evData.event_continueTime > 0) 
            {
                StartCoroutine(AutoEndRoutine(eventID, evData.event_continueTime));
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
                int r = Random.Range(0, 2);
                if (r == 0)
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


    private IEnumerator AutoEndRoutine(int eventID, float dur)
    {
        yield return new WaitForSeconds(dur);

        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(RPC_ResolveEvent), RpcTarget.All, eventID);
        }
    }

    /// <summary>
    /// �̺�Ʈ �ܺο��� ����
    /// </summary>
    /// <param name="eventID"></param>
    public void EndEvent(int eventID)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(RPC_ResolveEvent), RpcTarget.All, eventID);
        }
    }


    [PunRPC]
    private void RPC_ResolveEvent(int eventID)
    {
        ResolveEvent(eventID);

        // MessageDisplayManager.Instance.SendMessage($"{CSVManager.Instance.Events[eventID].event_name} �ذ� �Ϸ�");
    }

    public void ResolveEvent(int eventID)
    {
        if (_activeEventsID.Remove(eventID))
        {
            var evData = CSVManager.Instance.Events[eventID];

            OnEventEnded?.Invoke(evData);
        }
    }
}