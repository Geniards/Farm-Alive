using System.Collections;
using Photon.Pun;
using UnityEngine;
using GameData;
using UnityEngine.Events;

public class StageManager : MonoBehaviourPunCallbacks
{
    [SerializeField] int _curStageID;

    private bool _isMapSetted = false;
    public UnityEvent OnGameStarted;

    [Header("�������� �ð� �Ӽ�")]
    [SerializeField] float _stageTimeLimit = 0;
    [SerializeField] float _curStageTime = 0;
    public float CurStageTime { get { if (PhotonNetwork.IsMasterClient) return _curStageTime; else return 0f; } }
    [SerializeField] bool _isTimerRunning = false;

    private STAGE _curStageData;
    private int _weatherID;
    public int WeatherID { get { return _weatherID; } }

    private int _maxBrokenMachineCount;
    private int _maxDamagedCropCount = 0; // 0���� ���� (���� ������ ���̺����� �������θ� ����)

    // ��谡 ���峭 Ƚ�� (�ٸ������� ���峪�� ++�ʿ�)
    public int brokenMachineCount = 0;
    // �۹��� �ջ�� Ƚ�� (�ٸ������� �ջ�Ǹ� ++�ʿ�)
    public int damagedCropCount = 0;

    [Tooltip("��ȯ�� �÷��̾� ������")]
    public GameObject newcharacterPrefab;
    public Vector3 PlayerSpawn;

    // ������ ��ƼŬ / ������Ʈ ��.
    [Tooltip("������ ��ī�̹ڽ� ���׸��� (������� ����)")]
    [SerializeField] Material[] _materials;
    //
    //

    public static StageManager Instance { get; private set; }

    private void Awake()
    {
        _curStageID = PunManager.Instance.selectedStage;

        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private IEnumerator Start()
    {
        // CSV �ٿ�ε� ���� ������ ���
        while (!CSVManager.Instance.downloadCheck)
            yield return null;

        var stageDict = CSVManager.Instance.Stages;

        _curStageData = stageDict[_curStageID];
        _weatherID = _curStageData.stage_seasonID;
        _maxBrokenMachineCount = _curStageData.stage_allowSymptomFacilityCount;

        _stageTimeLimit = 400f;


        while (!ParticleManager.Instance.isAllParticleStoped)
            yield return null; // ��� ��ƼŬ ���� ����, ��� �ߴ� �� ����

        SetSeason();

        while (!_isMapSetted)
            yield return null; // �� ���� �� ����

        SpawnPlayer();

        yield return new WaitForSeconds(5f);

        OnGameStarted?.Invoke();
        
        yield return new WaitForSeconds(5f);
        
        if (PhotonNetwork.IsMasterClient)
            StartStageTimer();
    }

    private void Update()
    {
        if (!_isTimerRunning)
            return;

        _curStageTime += Time.deltaTime;

        while (_stageTimeLimit - _curStageTime <= 60f)
        {
            SoundManager.Instance.PlaySFXLoop("BGM_StageOneMinute");

            if (_stageTimeLimit == _curStageTime)
                SoundManager.Instance.StopSFXLoop("BGM_StageOneMinute");
                break;
        }

        if (_stageTimeLimit > 0 && _curStageTime >= _stageTimeLimit)
        {
            photonView.RPC(nameof(EndStage), RpcTarget.All);
        }
    }

    private void SetSeason()
    {
        // �ʺ� ��ƼŬ setactive false

        switch (_weatherID)
        {
            case 0: // ��
                 RenderSettings.skybox = _materials[0];
                break;
            case 1: // ����
                 RenderSettings.skybox = _materials[1];
                break;
            case 2: // ����
                 RenderSettings.skybox = _materials[2];
                break;
            case 3: // �ܿ�
                 RenderSettings.skybox = _materials[3];
                break;
        }

        _isMapSetted = true;
    }

    private void SpawnPlayer()
    {
        GameObject Player = PhotonNetwork.Instantiate(newcharacterPrefab.name, PlayerSpawn, Quaternion.identity);
    }

    public void StartStageTimer()
    {
        QuestManager.Instance.FirstStart(_curStageID);
        SoundManager.Instance.PlayBGM(_curStageID.ToString());
        Debug.Log($"{_curStageID.ToString()} BGM ����!");
        _curStageTime = 0f;
        _isTimerRunning = true;
    }

    [PunRPC]
    /// <summary>
    /// ����Ʈ�� ��� ����Ǿ��� ��, ȣ���� �Լ�.
    /// </summary>
    public void EndStage()
    {
        _isTimerRunning = false;

        int star = EvaluateStar();
        float playTime = _curStageTime;

        FirebaseManager.Instance.SaveStageResult(_curStageID, _curStageTime, star);

        StartCoroutine(ReturnToFusion());
    }

    public IEnumerator ReturnToFusion()
    {
        yield return new WaitForSeconds(3f);

        if (PhotonNetwork.InRoom)
        {
            Debug.Log("Pun �� ������...");
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            Debug.LogWarning($"���� ���¿����� LeaveRoom�� ȣ���� �� �����ϴ�: {PhotonNetwork.NetworkClientState}");
        }


        Debug.Log("���� ��ü ��...");
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        Debug.Log($"PhotonNetwork.InRoom = {PhotonNetwork.InRoom}");
        Debug.Log($"PhotonNetwork.NetworkClientState = {PhotonNetwork.NetworkClientState}");
    }


    private int EvaluateStar()
    {
        int star = 0;

        int successCount = QuestManager.Instance.clearQuestCount;
        int totalDealer = QuestManager.Instance.totalQuestCount;

        if (totalDealer == 3)
        {
            if (successCount == 3) star = 3;
            else if (successCount == 2) star = 2;
            else if (successCount == 1) star = 1;
            else star = 0;
        }
        else if (totalDealer == 2)
        {
            if (successCount == 2) star = 3;
            else if (successCount == 1) star = 1;
            else star = 0;
        }
        else if (totalDealer == 1)
        {
            // 1�� �������� => (1=>3star, 0=>0star)
            if (successCount == 1) star = 3;
            else star = 0;
        }

        if (damagedCropCount <= _maxDamagedCropCount)
            star += 1;

        if (brokenMachineCount <= _maxBrokenMachineCount)
            star += 1;

        // �ִ� 5��
        if (star > 5) star = 5;
        return star;
    }
}