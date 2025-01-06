using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;
using UnityEngine.SocialPlatforms;

public class TimeSystem : MonoBehaviour
{
    /// <summary>
    /// ���������� ���ѽð��� ���õ� Ÿ�� �ý���
    /// �Ϲ������� Ÿ���� ���ӿ��� ��ӱ��, �Ͻ����� ����� ����ϱ� ���� �ʿ��ϸ�
    /// �߰������� �ʿ����� ��쿡�� ����ϰ� �� ��ũ��Ʈ�� ���� ���������� ���ѽð��� �����ð��� �����.
    /// </summary>

    [Header("�������� �ð� ���� ����")]
    [Tooltip("�������� ���� �ð�(�� ����)")]
    [SerializeField] float _stageTimeLimit = 360f; // ����Ʈ��. 6��

    [Tooltip("���� ���� �ð� ��� ��� �� ����")]
    [SerializeField] float _timeScale = 1f; // = ���� �ӵ�

    [Header("Ÿ�̸� ���� �̺�Ʈ")]
    [Tooltip("�������� �ð��� ��� �����Ǿ��� �� ȣ���� �̺�Ʈ")]
    // �� ��ȯ ��
    [SerializeField] UnityEvent _onStageTimeOver;

    // ��� �ð�
    private float _curStageTime = 0f;
    private bool _isTimerRunnig = false;
    private bool _isPaused = false;

    // �ð����� �������� ��ũ��Ʈ ���
    // ex) UI��� ���� �� ���, ��Ȱ��ȭ �� ��� ���� �ʿ�
    public event Action<float, float> onTimeUpdate;

    /// <summary>
    /// stageManager���� ��ũ���ͺ� ������Ʈ�� �о�� ���ѽð��� �����ϱ� ���� public �Լ�
    /// timeSystem.SetTimeLimit(stageDataSO.timeLimit);
    /// timeSystem.StartStageTimer();
    /// </summary>
    /// <param name="seconds"></param>
    public void SetTimeLimit(float seconds)
    {
        _stageTimeLimit = seconds;
    }

    /// <summary>
    /// ī��Ʈ ���� �Լ�
    /// </summary>
    public void StartStageTimer()
    {
        _curStageTime = 0F;
        _isTimerRunnig = true;
        _isPaused = false;
    }

    /// <summary>
    /// �Ͻ����� �Լ� (�ʿ� �� ����� ��)
    /// </summary>
    public void PauseTimer()
    {
        _isPaused = true;
    }

    public void ResumeTimer()
    {
        _isPaused = false;
    }

    private void Update()
    {
        if (!_isTimerRunnig)
            return;
        if (_isPaused)
            return;

        // ���� deltaTime�� ��� ������ŭ
        float delta = Time.deltaTime * _timeScale;

        // �ð� ���
        _curStageTime += delta;

        // ��������
        onTimeUpdate?.Invoke(_curStageTime, delta);

        // ���ѽð� �������� ���
        if (_curStageTime >= _stageTimeLimit)
        {
            _isTimerRunnig = false;
            _onStageTimeOver?.Invoke(); // �������� ���� �̺�Ʈ ȣ��
        }
    }

    public float GetRemainTime()
    {
        // ���� �ð�
        return Mathf.Max(0, _stageTimeLimit - _curStageTime);
    }
}