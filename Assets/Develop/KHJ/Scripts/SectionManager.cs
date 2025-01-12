using GameData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SectionManager : MonoBehaviour
{
    private const int SECTION_NUM = 4;
    private const int GROUND_PER_SECTION = 8;

    public static SectionManager Instance { get; private set; }

    [SerializeField] private Crop[,] _sections;
    [SerializeField] private int _curSection;

    // ���Ǻ� ��ƼŬ �迭
    //[SerializeField] private SectionParticles[] _sectionParticles;
    [SerializeField] private ParticleSystem[] _sectionParticles;

    private EventManager _eventManager;

    public Crop[,] Sections { get { return _sections; } }
    public int CurSection { get { return _curSection; } set { _curSection = value; } }

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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += Init;

        foreach (ParticleSystem particle in _sectionParticles)
        {
            particle.Stop();
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= Init;
    }

    private void Init(Scene scene, LoadSceneMode mode)
    {
        _curSection = 0;

        _sections = new Crop[SECTION_NUM, GROUND_PER_SECTION];

        //_eventManager = GameObject.FindGameObjectWithTag("EventManager").GetComponent<EventManager>();
        //_eventManager.OnEventStarted.AddListener(OnDownpourStarted);
        //_eventManager.OnEventStarted.AddListener(OnBlightStarted);
        //_eventManager.OnEventStarted.AddListener(OnDroughtStarted);
        //_eventManager.OnEventStarted.AddListener(OnHighTemperatureStarted);
        //_eventManager.OnEventStarted.AddListener(OnLowTemperatureStarted);
        //_eventManager.OnEventEnded.AddListener(OnDownpourEnded);
        //_eventManager.OnEventEnded.AddListener(OnBlightEnded);
        //_eventManager.OnEventEnded.AddListener(OnDroughtEnded);
        //_eventManager.OnEventEnded.AddListener(OnHighTemperatureEnded);
        //_eventManager.OnEventEnded.AddListener(OnLowTemperatureEnded);
    }

    /// <summary>
    /// ���ֱ� ��ư�� �Լ�
    /// </summary>
    [SerializeField] LiquidContainer waterBarrel;
    public void IncreaseMoisture()
    {
        if (waterBarrel.FillAmount <= 0)
            return;

        Crop crop;
        for (int i = 0; i < GROUND_PER_SECTION; i++)
        {
            crop = _sections[_curSection, i];
            if (crop != null)
                crop.IncreaseMoisture(); 
        }

        PlayParticle(_curSection, false);

        waterBarrel.FillAmount -= 0.1f;

    }

    /// <summary>
    /// ����ֱ� ��ư�� �Լ�
    /// </summary>
    [SerializeField] LiquidContainer nutrientBarrel;
    public void IncreaseNutrient()
    {
        if (nutrientBarrel.FillAmount <= 0)
            return;

        Crop crop;
        for (int i = 0; i < GROUND_PER_SECTION; i++)
        {
            crop = _sections[_curSection, i];
            if (crop != null)
                crop.IncreaseNutrient();
        }

        PlayParticle(_curSection, true);

        nutrientBarrel.FillAmount -= 0.1f;
    }

    private void PlayParticle(int sectionIndex, bool isNutrient)
    {
        //if (_sectionParticles.Length <= sectionIndex)
        //    return;

        //var particles = _sectionParticles[sectionIndex];//.moistureParticles;
        
        foreach ( ParticleSystem particle in _sectionParticles )
        {
            StartCoroutine(PlayParticleRoutine(particle, isNutrient));
        }
    }

    private IEnumerator PlayParticleRoutine(ParticleSystem particle, bool isNutrient)
    {
        var main = particle.main;

        // isNutrient�� ��츸 ���� �ʷϻ����� ����
        if (isNutrient)
        {
            main.startColor = Color.green;
        }
        else
        {
            main.startColor = Color.white;
        }

        particle.Play();

        yield return new WaitForSeconds(5f);

        particle.Stop();
    }

    #region �����̺�Ʈ �����Լ�
    //private void OnDownpourStarted(GameData.EVENT eventData)
    //{
    //    if (eventData.event_name != "����")
    //        return;

    //    foreach (List<Crop> cropList in _sections)
    //    {
    //        foreach (Crop crop in cropList)
    //        {
    //            crop.OnDownpourStarted();
    //        }
    //    }
    //}

    //private void OnDownpourEnded(GameData.EVENT eventData)
    //{
    //    if (eventData.event_name != "����")
    //        return;

    //    foreach (List<Crop> cropList in _sections)
    //    {
    //        foreach (Crop crop in cropList)
    //        {
    //            crop.OnDownpourEnded();
    //        }
    //    }
    //}

    //private void OnBlightStarted(GameData.EVENT eventData)
    //{
    //    if (eventData.event_name != "������")
    //        return;

    //    foreach (List<Crop> cropList in _sections)
    //    {
    //        foreach (Crop crop in cropList)
    //        {
    //            crop.OnBlightStarted();
    //        }
    //    }
    //}

    //private void OnBlightEnded(GameData.EVENT eventData)
    //{
    //    if (eventData.event_name != "������")
    //        return;

    //    foreach (List<Crop> cropList in _sections)
    //    {
    //        foreach (Crop crop in cropList)
    //        {
    //            crop.OnBlightEnded();
    //        }
    //    }
    //}

    //private void OnDroughtStarted(GameData.EVENT eventData)
    //{
    //    if (eventData.event_name != "����")
    //        return;

    //    foreach (List<Crop> cropList in _sections)
    //    {
    //        foreach (Crop crop in cropList)
    //        {
    //            crop.OnDroughtStarted();
    //        }
    //    }
    //}

    //private void OnDroughtEnded(GameData.EVENT eventData)
    //{
    //    if (eventData.event_name != "����")
    //        return;

    //    foreach (List<Crop> cropList in _sections)
    //    {
    //        foreach (Crop crop in cropList)
    //        {
    //            crop.OnDroughtEnded();
    //        }
    //    }
    //}

    //private void OnHighTemperatureStarted(GameData.EVENT eventData)
    //{
    //    if (eventData.event_name != "�µ� ���")
    //        return;

    //    foreach (List<Crop> cropList in _sections)
    //    {
    //        foreach (Crop crop in cropList)
    //        {
    //            crop.OnHighTemperatureStarted();
    //        }
    //    }
    //}

    //private void OnHighTemperatureEnded(GameData.EVENT eventData)
    //{
    //    if (eventData.event_name != "�µ� ���")
    //        return;

    //    foreach (List<Crop> cropList in _sections)
    //    {
    //        foreach (Crop crop in cropList)
    //        {
    //            crop.OnHighTemperatureEnded();
    //        }
    //    }
    //}

    //private void OnLowTemperatureStarted(GameData.EVENT eventData)
    //{
    //    if (eventData.event_name != "�µ� �ϰ�")
    //        return;

    //    foreach (List<Crop> cropList in _sections)
    //    {
    //        foreach (Crop crop in cropList)
    //        {
    //            crop.OnLowTemperatureStarted();
    //        }
    //    }
    //}

    //private void OnLowTemperatureEnded(GameData.EVENT eventData)
    //{
    //    if (eventData.event_name != "�µ� �ϰ�")
    //        return;

    //    foreach (List<Crop> cropList in _sections)
    //    {
    //        foreach (Crop crop in cropList)
    //        {
    //            crop.OnLowTemperatureEnded();
    //        }
    //    }
    //}
    #endregion
}
