using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] List<ParticleSystem[]> _particleSystem;

    [SerializeField] SectionManager _sectionManager;

    private ParticleSystem[] _particles;

    // TODO : AWAKE . ADDLISTENER -> SECTIONMOVER
    //        ONDESTROY . REMOVELISTENER

    // ONSECTIONCHANGED -> ���� ��ƼŬ �ý��ۿ� �Ҵ�

    // ���� ���� STARTCOLOR �Ͼ�� ��ȯ �� ��� �ڷ�ƾ
    // ���� �ʷϻ� ,
}
