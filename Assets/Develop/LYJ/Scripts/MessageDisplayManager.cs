using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageDisplayManager : MonoBehaviour
{
    public static MessageDisplayManager Instance { get; private set; }

    [SerializeField] private Canvas _messageCanvas;
    [SerializeField] private TextMeshProUGUI _messageText;
    [SerializeField] private float _displayTime = 3f;        // �ؽ�Ʈ ǥ�� �ð�
    [SerializeField] private float _distanceFromCamera = 3f; // �ؽ�Ʈ�� ī�޶� �տ� ��Ÿ�� �Ÿ�

    private Transform _playerCamera;
    private float _timer;
    private bool _isShowing;

    private void Awake()
    {
        _messageCanvas = GetComponentInChildren<Canvas>();
        _messageText = GetComponentInChildren<TextMeshProUGUI>();

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (_messageCanvas != null)
        {
            _messageCanvas.enabled = false;
        }
        else
        {
            Debug.Log("Canvas�� �����ؾ� �մϴ�.");
        }

        if (_messageText == null)
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
        if (_isShowing)
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0)
            {
                HideMessage();
            }
        }

        // ĵ���� ��ġ�� �÷��̾� ī�޶� �������� ������Ʈ
        if (_playerCamera != null && _messageCanvas.enabled)
        {
            UpdateCanvasPosition();
        }
    }

    private void FindPlayerCamera()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            _playerCamera = mainCamera.transform;
        }
        else
        {
            //Debug.Log("Camera�� ã�� �� �����ϴ�.");
        }
    }

    /// <summary>
    /// �޽����� ǥ��
    /// </summary>
    /// <param name="message">�÷��̾�� ǥ���� �޽���</param>
    /// <param name="displayTime">�޽��� ǥ�� �ð� (������)</param>
    /// <param name="distanceFromCamera">�޽����� ī�޶󿡼� ������ �Ÿ� (������)</param>
    public void ShowMessage(string message, float? displayTime = null, float? distanceFromCamera = null)
    {
        if (_playerCamera == null)
        {
            FindPlayerCamera(); // ī�޶� �������� �ʾҴٸ� �ٽ� Ž��
        }

        if (_messageText != null && _messageCanvas != null)
        {
            _messageText.text = message;

            _timer = displayTime ?? _displayTime;
            _distanceFromCamera = distanceFromCamera ?? _distanceFromCamera;

            _messageCanvas.enabled = true;
            _isShowing = true;

            // �޽����� Ȱ��ȭ�� �� ĵ���� ��ġ ������Ʈ
            UpdateCanvasPosition();
        }
    }

    private void HideMessage()
    {
        if (_messageCanvas != null)
        {
            _messageCanvas.enabled = false;
            _isShowing = false;
        }
    }

    // ĵ������ �÷��̾��� ���鿡 ��ġ��
    private void UpdateCanvasPosition()
    {
        if (_playerCamera == null || _messageCanvas == null) return;

        // �÷��̾� ī�޶��� ��ġ�� ȸ��
        Vector3 cameraPosition = _playerCamera.position;
        Quaternion cameraRotation = _playerCamera.rotation;

        // ī�޶� ���� �� �Ÿ���ŭ ĵ���� ��ġ�� ����
        Vector3 canvasPosition = cameraPosition + cameraRotation * Vector3.forward * _distanceFromCamera;
        _messageCanvas.transform.position = canvasPosition;

        // ĵ������ �׻� ī�޶� ���ϵ��� ȸ��
        _messageCanvas.transform.rotation = Quaternion.LookRotation(_messageCanvas.transform.position - cameraPosition);
    }
}
