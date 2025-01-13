using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance { get; private set; }

    public bool isAllParticleStoped = false;

    [System.Serializable]
    public class ParticleInfo
    {
        public string key;
        public ParticleSystem particle;
    }


    [Header("���� ��ƼŬ ���")]
    [SerializeField] private ParticleInfo[] _particleInfo;

    private Dictionary<string, ParticleSystem> _particleDict = new Dictionary<string, ParticleSystem>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        foreach (var info in _particleInfo)
        {
            _particleDict.Add(info.key, info.particle);
            info.particle.Stop();
        }

        Debug.Log("��� ��ƼŬ ��ųʸ�ȭ, ������� �Ϸ�");

        isAllParticleStoped = true;
    }

    /// <summary>
    /// ��ƼŬ �÷���, duration�� �� stop
    /// ����, Ư���ϰ� ���ӽð��� ������ ���� ���� ��ƼŬ�̶��,
    /// float���� = 0 ���� ����
    /// ����, ���� Ÿ�ֿ̹� stopParticle(key(name))ȣ��
    /// </summary>
    /// <param name="key"></param>
    /// <param name="duration"></param>
    public void PlayParticle(string key, float duration)
    {
        if (!_particleDict.ContainsKey(key))
            return;

        ParticleSystem particle = _particleDict[key];

        particle.Play();

        if (duration > 0)
        {
            StartCoroutine(StopParticleAfter(particle, duration));
        }
    }

    /// <summary>
    /// Ư�� ��ƼŬ Stop
    /// </summary>
    public void StopParticle(string key)
    {
        ParticleSystem particle = _particleDict[key];
        if (particle.isPlaying)
            particle.Stop();
    }

    /// <summary>
    /// ��� ��ƼŬ Stop
    /// ���� ���۽ÿ� ����
    /// </summary>
    public void StopAllParticles()
    {
        foreach (var kv in _particleDict)
        {
            var ps = kv.Value;
            if (ps.isPlaying)
                ps.Stop();
        }
    }

    private IEnumerator StopParticleAfter(ParticleSystem particle, float duration)
    {
        yield return new WaitForSeconds(duration);
        if (particle.isPlaying)
        {
            particle.Stop();
        }
    }
}
