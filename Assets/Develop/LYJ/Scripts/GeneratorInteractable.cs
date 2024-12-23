using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GeneratorInteractable : XRGrabInteractable
{
    [Header("Generator Settings")]
    [Tooltip("�õ��� �ɸ������ �ʿ��� �õ� Ƚ��")]
    [SerializeField] private int startAttemptsRequired = 3; // m�� (�ʿ��� �õ� Ƚ��)

    [Header("Cord Settings")]
    [Tooltip("�õ����� ���� ���� ��ġ")]
    [SerializeField] private Transform cordStartPosition;
    [Tooltip("�õ����� �ִ� �� ��ġ")]
    [SerializeField] private Transform cordEndPosition;
    [Tooltip("�õ��� ������Ʈ")]
    [SerializeField] private Transform cordObject;

    private Vector3 initialCordPosition; // ���� �ʱ� ��ġ
    private Quaternion initialCordRotation; // ���� �ʱ� ȸ��
    private int currentAttempts = 0; // ���� �õ� Ƚ��
    private bool isBeingPulled = false; // ���� ������� �ִ� ��������
    private bool hasTriggered = false; // �ߺ� ó���� �����ϱ� ���� �÷���

    private void Start()
    {
        // ���� �ʱ� ��ġ �� �ʱ� ȸ�� ����
        if (cordObject != null)
        {
            initialCordPosition = cordObject.position;
            initialCordRotation = cordObject.rotation;
        }
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

    private void OnTriggerEnter(Collider other)
    {
        // cordObject�� cordEndPosition�� ��Ұ�, �ߺ� ó���� �� �� ���
        if (other.transform == cordEndPosition && !hasTriggered)
        {
            hasTriggered = true; // �ߺ� ó�� ����
            currentAttempts++; // �õ� Ƚ�� ����

            Debug.Log($"������ �õ� �õ�: {currentAttempts}/{startAttemptsRequired}");

            if (currentAttempts >= startAttemptsRequired)
            {
                Debug.Log("������ �õ� ����!");
                currentAttempts = 0; // ���� �� �ʱ�ȭ
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // cordObject�� cordEndPosition���� ����� �� �ߺ� ó���� �ٽ� ���
        if (other.transform == cordEndPosition)
        {
            hasTriggered = false; // �÷��� �ʱ�ȭ
        }
    }

    private void ResetCordPosition()
    {
        // ���� �ʱ� ��ġ�� �ʱ� ȸ������ �ǵ�����
        if (cordObject != null)
        {
            cordObject.position = initialCordPosition;
            cordObject.rotation = initialCordRotation;
        }
    }
}
