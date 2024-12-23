using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;
using System.Collections;

public class GeneratorInteractable : XRGrabInteractable
{
    [Header("Generator Settings")]
    [Tooltip("�õ��� �ɸ������ �ʿ��� �õ� Ƚ��")]
    [SerializeField] private int startAttemptsRequired = 3;

    [Tooltip("������ �߻��ϱ������ �ð�")]
    [SerializeField] private float breakdownWarningDuration = 5f;
  
    [Tooltip("�õ����� ���� ���� ��ġ")]
    [SerializeField] private Transform cordStartPosition;
    [Tooltip("�õ����� �ִ� �� ��ġ")]
    [SerializeField] private Transform cordEndPosition;
    [Tooltip("�õ��� ������Ʈ")]
    [SerializeField] private Transform cordObject;

    [Header("Linked Components")]
    [SerializeField] private XRKnob knob;
    [SerializeField] private XRLever lever;

    [Header("Lighting")]
    [Tooltip("���� �� ���� ������Ʈ")]
    [SerializeField] private HeadLightInteractable headLight;

    private Vector3 initialCordPosition;
    private Quaternion initialCordRotation;
    private Vector3 initialCordScale;
    private int currentAttempts = 0;
    private bool isBeingPulled = false;
    private bool hasTriggered = false;

    private bool isGeneratorRunning = true;
    private Coroutine warningCoroutine = null;

    private bool isLeverDown = false;
    private float currentKnobValue = 0f;

    private void Start()
    {
        knob = transform.root.GetComponentInChildren<XRKnob>();
        lever = transform.root.GetComponentInChildren<XRLever>();

        knob.onValueChange.AddListener(OnKnobValueChanged);
        lever.onLeverActivate.AddListener(OnLeverActivate);
        lever.onLeverDeactivate.AddListener(OnLeverDeactivate);

        if (cordObject != null)
        {
            initialCordPosition = cordObject.position;
            initialCordRotation = cordObject.rotation;
            initialCordScale = cordObject.localScale;
        }
    }

    private void OnKnobValueChanged(float value)
    {
        currentKnobValue = value;

        if (currentKnobValue >= 1f && !isGeneratorRunning)
        {
            Debug.Log("���� �ִ� �������� ���ư�!");
        }
    }

    private void OnLeverActivate()
    {
        isLeverDown = true;
        Debug.Log("������ ���������ϴ�.");

        if (warningCoroutine != null)
        {
            StopCoroutine(warningCoroutine);
            warningCoroutine = null;
            Debug.Log("������ �������� ���� ������.");
        }
    }

    private void OnLeverDeactivate()
    {
        isLeverDown = false;
        Debug.Log("������ �ö󰬽��ϴ�.");
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        isBeingPulled = true;
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        isBeingPulled = false;
        ResetCordPosition();
    }

    private void Update()
    {
        if (isBeingPulled && cordObject != null && cordStartPosition != null && cordEndPosition != null)
        {
            float pullDistance = Vector3.Distance(cordObject.position, cordStartPosition.position);

            Vector3 newScale = initialCordScale;
            newScale.y = initialCordScale.y + pullDistance;
            cordObject.localScale = newScale;
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("���� ���� �׽�Ʈ ����");
            TriggerWarning();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == cordEndPosition && !hasTriggered)
        {
            hasTriggered = true;
            currentAttempts++;

            Debug.Log($"������ �õ� �õ�: {currentAttempts}/{startAttemptsRequired}");

            if (currentAttempts >= startAttemptsRequired && currentKnobValue >= 1f)
            {
                Debug.Log("������ �õ� ����!");
                isGeneratorRunning = true;
                currentAttempts = 0;

                // ���� ����
                Light[] lights = FindObjectsOfType<Light>();
                foreach (Light light in lights)
                {
                    if (light.type == LightType.Directional)
                    {
                        light.enabled = true;
                        Debug.Log("���� ����");
                    }
                }

                if (headLight != null)
                {
                    headLight.DisableHeadlight();
                    Debug.Log("������Ʈ ����");
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform == cordEndPosition)
        {
            hasTriggered = false;
        }
    }

    private void ResetCordPosition()
    {
        if (cordObject != null)
        {
            cordObject.position = initialCordPosition;
            cordObject.rotation = initialCordRotation;
            cordObject.localScale = initialCordScale;
        }
    }

    public void TriggerWarning()
    {
        if (warningCoroutine == null)
        {
            warningCoroutine = StartCoroutine(BreakdownWarning());
        }
    }

    private IEnumerator BreakdownWarning()
    {
        Debug.Log("���� ����! ������ ���� ������ �����ϼ���!!!");
        yield return new WaitForSeconds(breakdownWarningDuration);

        if (!isLeverDown)
        {
            Debug.Log("������ �߻��߽��ϴ�!");
            isGeneratorRunning = false;

            Light[] lights = FindObjectsOfType<Light>();
            foreach (Light light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    light.enabled = false;
                    Debug.Log("���� �߻�");
                }
            }

            if (headLight != null)
            {
                headLight.EnableHeadlight();
                Debug.Log("������Ʈ ����");
            }
            else
            {
                Debug.Log("headLight�� �����ϴ�.");
            }
        }

        warningCoroutine = null;
    }
}
