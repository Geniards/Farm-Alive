using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubwayController : MonoBehaviour
{
    [Header("Light GameObjects")]
    public List<Light> lights;

    [Header("Light Control Settings")]
    [Tooltip("Light ��ȯ ����(��)")]
    public float interval = 5f;

    [Header("Shake Settings")]
    [Tooltip("Transform ��鸲 �߻� Ȯ�� (0 ~ 1 ���� ��)")]
    [Range(0f, 1f)]
    public float shakeProbability = 0.2f;
    [Tooltip("Transform ��鸲 ����")]
    public float shakeAmount = 0.5f;
    [Tooltip("Transform ��鸲 �ӵ�")]
    public float shakeSpeed = 5f;
    [Tooltip("��鸲 ���� �ð� (��)")]
    public float shakeDuration = 1f;

    private int currentIndex = 0;
    private int halfIndex = 0;

    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = transform.position;
        StartCoroutine(ControlLights());
        StartCoroutine(RandomShakeTrigger());
    }

    private IEnumerator ControlLights()
    {
        while (true)
        {
            SetAllLights(true);

            halfIndex = (currentIndex + lights.Count / 2) % lights.Count;
            lights[currentIndex].gameObject.SetActive(false);
            lights[halfIndex].gameObject.SetActive(false);

            yield return new WaitForSeconds(interval);

            currentIndex = (currentIndex + 1) % lights.Count;
        }
    }

    private void SetAllLights(bool state)
    {
        foreach (var light in lights)
        {
            light.gameObject.SetActive(state);
        }
    }

    private IEnumerator RandomShakeTrigger()
    {
        while (true)
        {
            if (Random.value <= shakeProbability)
            {
                yield return StartCoroutine(ShakePosition());
            }
            yield return new WaitForSeconds(interval);
        }
    }

    private IEnumerator ShakePosition()
    {
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            float offsetX = Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;
            transform.position = new Vector3(initialPosition.x + offsetX, initialPosition.y, initialPosition.z);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = initialPosition;
    }



}
