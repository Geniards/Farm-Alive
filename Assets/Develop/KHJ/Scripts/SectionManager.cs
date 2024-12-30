using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SectionManager : MonoBehaviour
{
    private const int SECTION_NUM = 4;

    public static SectionManager Instance { get; private set; }

    [SerializeField] private List<Crop>[] _crops;
    [SerializeField] private int _curSection;

    // ���Ǻ� ��ƼŬ �迭
    [SerializeField] private SectionParticles[] _sectionParticles;

    public List<Crop>[] Crops { get { return _crops; } }
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
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= Init;
    }

    private void Init(Scene scene, LoadSceneMode mode)
    {
        _curSection = 0;

        _crops = new List<Crop>[SECTION_NUM];
        for (int i = 0; i < SECTION_NUM; i++)
            _crops[i] = new List<Crop>();
    }

    /// <summary>
    /// ���ֱ� ��ư�� �Լ�
    /// </summary>
    [SerializeField] LiquidContainer waterBarrel;
    public void IncreaseMoisture()
    {
        if (waterBarrel.FillAmount <= 0)
            return;

        foreach (Crop crop in _crops[_curSection])
        {
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

        foreach (Crop crop in _crops[_curSection])
        {
            crop.IncreaseNutrient();
        }

        PlayParticle(_curSection, true);

        nutrientBarrel.FillAmount -= 0.1f;
    }

    private void PlayParticle(int sectionIndex, bool isNutrient)
    {
        if (_sectionParticles.Length <= sectionIndex)
            return;

        var particles = _sectionParticles[sectionIndex].moistureParticles;
        
        foreach ( var particle in particles )
        {
            StartCoroutine(PlayParticleRoutine(particle, isNutrient));
        }
    }

    private IEnumerator PlayParticleRoutine(ParticleSystem particle, bool isNutrient)
    {
        var main = particle.main;
        // ���� �� ����
        var originColor = main.startColor;

        // isNutrient�� ��츸 ���� �ʷϻ����� ����
        if (isNutrient)
        {
            main.startColor = Color.green;
        }

        particle.Play();

        yield return new WaitForSeconds(5f);

        particle.Stop();

        // ��� ���ٸ� �� ���󺹱�
        if (isNutrient)
        {
            main.startColor = originColor;
        }
    }
}
