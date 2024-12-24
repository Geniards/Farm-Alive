using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;
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

    private XRKnob _knob;
    private XRLever _lever;

    [SerializeField] private Repair repair;
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
        // �ʱ�ȭ
        _knob = transform.root.GetComponentInChildren<XRKnob>();
        _lever = transform.root.GetComponentInChildren<XRLever>();

        _knob.onValueChange.AddListener(OnKnobValueChanged);
        _lever.onLeverActivate.AddListener(OnLeverActivate);
        _lever.onLeverDeactivate.AddListener(OnLeverDeactivate);

        if (cordObject != null)
        {
            initialCordPosition = cordObject.position;
            initialCordRotation = cordObject.rotation;
            initialCordScale = cordObject.localScale;
        }

        if (headLight == null)
        {
            headLight = FindObjectOfType<HeadLightInteractable>();
            if (headLight == null)
            {
                Debug.LogWarning("HeadLightInteractable�� ã�� �� �����ϴ�.");
            }
        }

        repair = GetComponent<Repair>();
        repair.enabled = false;
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
                if (headLight != null)
                {
                    headLight.RecoverFromBlackout(); // ���� ����
                }
                else
                {
                    Debug.LogWarning("HeadLightInteractable�� �������� �ʾҽ��ϴ�!");
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
            repair.enabled = true;
            isGeneratorRunning = false;

            if (headLight != null)
            {
                headLight.TriggerBlackout(); // ���� �߻�
            }
            else
            {
                Debug.LogWarning("HeadLightInteractable�� �������� �ʾҽ��ϴ�!");
            }
        }

        warningCoroutine = null;
    }
}
