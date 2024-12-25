using System.Collections;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;

public class GeneratorInteractable : XRBaseInteractable
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

    [SerializeField] private XRKnobGenerator _knob;
    [SerializeField] private XRLever _lever;

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

    private Rigidbody rigid;
    private Vector3 startPos;

    private bool warningActive = false;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        startPos = transform.position;

        repair = GetComponentInParent<Repair>();
        repair.enabled = false;

        headLight = FindObjectOfType<HeadLightInteractable>();

        _knob = transform.root.GetComponentInChildren<XRKnobGenerator>();
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
            Debug.LogWarning("HeadLightInteractable�� ã�� �� �����ϴ�.");
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
        // ���� ������ Ȱ��ȭ�� ��쿡�� ���� �����Ⱑ ������
        if (warningActive)
        {
            isLeverDown = true;
            Debug.Log("������ ���������ϴ�.");

            if (warningCoroutine != null)
            {
                StopCoroutine(warningCoroutine);
                warningCoroutine = null;
                warningActive = false;
                Debug.Log("������ �������� ���� ������.");
            }
        }
        else
        {
            Debug.Log("���� ������ �߻��� ���� �������� ���� ���� X");
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
        rigid.isKinematic = false;
        isBeingPulled = true;
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        isBeingPulled = false;
        rigid.isKinematic = true;
        transform.position = startPos;
    }

    private void Update()
    {
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
            warningActive = true; // ���� ���� Ȱ��ȭ
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

        warningActive = false; // ���� ������ �� �̻� ������� ����
        warningCoroutine = null;
    }
}
