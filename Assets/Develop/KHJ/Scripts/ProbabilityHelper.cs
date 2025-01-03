using System.Collections.Generic;
using UnityEngine;

public static class ProbabilityHelper
{
    /// <summary>
    /// ��÷ ���� Ȯ�� �Լ�
    /// </summary>
    /// <param name="percentage">��÷ Ȯ��</param>
    /// <returns>��÷ ����</returns>
    public static bool Draw(int percentage)
    {
        return Random.Range(0, 100) < percentage;
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
