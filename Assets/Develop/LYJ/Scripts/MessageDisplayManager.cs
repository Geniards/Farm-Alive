using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageDisplayManager : MonoBehaviour
{
    public static MessageDisplayManager Instance { get; private set; }

    [SerializeField] private Canvas canvas;              // �ؽ�Ʈ�� ��� ĵ����
    [SerializeField] private TextMeshProUGUI messageText; // �ؽ�Ʈ ������Ʈ
    [SerializeField] private float displayTime = 2f;     // �ؽ�Ʈ ǥ�� �ð�
    [SerializeField] private float distanceFromCamera = 2f; // �ؽ�Ʈ�� ī�޶� �տ� ��Ÿ�� �Ÿ�

    private Transform playerCamera;
    private float timer;
    private bool isShowing;

    private void Awake()
    {
        // �̱��� ����
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (canvas != null)
        {
            canvas.enabled = false; // �ʱ� ���´� ��Ȱ��ȭ
        }
        else
        {
            Debug.Log("Canvas�� �����ؾ� �մϴ�.");
        }

        if (messageText == null)
        {
            Debug.Log("Text�� �����ؾ� �մϴ�.");
        }
    }

    private void Start()
    {
        FindPlayerCamera();
    }

    private void Update()
    {
        // �޽��� Ÿ�̸� ������Ʈ
        if (isShowing)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                HideMessage();
            }
        }

        // ĵ���� ��ġ�� �÷��̾� ī�޶� �������� ������Ʈ
        if (playerCamera != null && canvas.enabled)
        {
            UpdateCanvasPosition();
        }
    }

    private void FindPlayerCamera()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            playerCamera = mainCamera.transform;
        }
        else
        {
            Debug.Log("Camera�� ã�� �� �����ϴ�.");
        }
    }

    /// <summary>
    /// �޽����� ǥ��
    /// </summary>
    /// <param name="message">�÷��̾�� ǥ���� �޽���</param>
    public void ShowMessage(string message)
    {
        if (playerCamera == null)
        {
            FindPlayerCamera(); // ī�޶� �������� �ʾҴٸ� �ٽ� Ž��
        }

        if (messageText != null && canvas != null)
        {
            messageText.text = message;
            canvas.enabled = true;
            timer = displayTime;
            isShowing = true;

            // �޽����� Ȱ��ȭ�� �� ĵ���� ��ġ ������Ʈ
            UpdateCanvasPosition();
        }
    }

    private void HideMessage()
    {
        if (canvas != null)
        {
            canvas.enabled = false;
            isShowing = false;
        }
    }

    // ĵ������ �÷��̾��� ���鿡 ��ġ��
    private void UpdateCanvasPosition()
    {
        if (playerCamera == null || canvas == null) return;

        // �÷��̾� ī�޶��� ��ġ�� ȸ��
        Vector3 cameraPosition = playerCamera.position;
        Quaternion cameraRotation = playerCamera.rotation;

        // ī�޶� ���� �� �Ÿ���ŭ ĵ���� ��ġ�� ����
        Vector3 canvasPosition = cameraPosition + cameraRotation * Vector3.forward * distanceFromCamera;
        canvas.transform.position = canvasPosition;

        // ĵ������ �׻� ī�޶� ���ϵ��� ȸ��
        canvas.transform.rotation = Quaternion.LookRotation(canvas.transform.position - cameraPosition);
    }
}
