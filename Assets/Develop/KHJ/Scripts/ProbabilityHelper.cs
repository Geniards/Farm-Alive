using System.Collections.Generic;
using UnityEngine;

public static class ProbabilityHelper
{
    /// <summary>
    /// ��÷ ���� Ȯ�� �Լ�
    /// </summary>
    /// <param name="percentage">��÷ Ȯ��, [0, 100]�� ����</param>
    /// <returns>��÷ ����</returns>
    public static bool Draw(int percentage)
    {
        return Random.Range(0, 100) < percentage;
    }

    /// <summary>
    /// ��÷ ���� Ȯ�� �Լ�
    /// </summary>
    /// <param name="rate">��÷ Ȯ��, [0, 1]�� ��</param>
    /// <returns>��÷ ����</returns>
    public static bool Draw(float rate)
    {
        return Random.Range(0f, 1f) < rate;
    }

    /// <summary>
    /// ����Ʈ ������ ������ ���� ��ȯ �Լ�
    /// </summary>
    /// <typeparam name="T">element type</typeparam>
    /// <param name="list">list</param>
    /// <returns>������ ����</returns>
    public static T Draw<T>(List<T> list)
    {
        return list[Random.Range(0, list.Count)];
    }

    /// <summary>
    /// �迭 ������ ������ ���� ��ȯ �Լ�
    /// </summary>
    /// <typeparam name="T">element type</typeparam>
    /// <param name="array">array</param>
    /// <returns>������ ����</returns>
    public static T Draw<T>(T[] array)
    {
        return array[Random.Range(0, array.Length)];
    }
}
