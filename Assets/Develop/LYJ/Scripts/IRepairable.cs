using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRepairable
{
    void Symptom();         // ���� ���� ó��
    bool Broken();          // ���� �߻� ó��
    void SolveSymptom();    // ���� ���� �ذ�
    void SolveBroken();     // ���� �ذ�
    bool IsBroken();        // ���� ���� ��ȯ
}
