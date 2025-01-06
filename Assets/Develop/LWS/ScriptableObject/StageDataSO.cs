using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "Game/StageData")]
public class StageDataSO : ScriptableObject
{
    public int stageNum; // �������� �ѹ�
    public E_WeatherType weatherType; // �������� �� ����
    public int questCount; // �ŷ�ó ��
    public float eventBaseChanse; // �̺�Ʈ �߻� Ȯ��
    public float timeLimit; // �������� ���� �ð�
}