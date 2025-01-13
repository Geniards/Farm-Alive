using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.XR;

public class GameStartManager : MonoBehaviour
{
    [Header("���� ��ŸƮ ������Ʈ ����")]
    public GameObject introPanel;
    public TMP_Text introText;
    public Image skipGauge;

    [Header("���� ����")]
    [Header("VideoPlayer�� �پ� �ִ� GameObject")]
    public GameObject videoPlayerObject;
    public GameObject rawImage;
    [Tooltip("���� ȭ���� HMD���� �󸶳� ������ �ִ��� ����")]
    public float videoDistance = 2.0f;
    private VideoPlayer videoPlayer;
    
    [Header("�г��� InputField ����")]
    public GameObject nickNameInputField;

    private int _currentStep = 0;
    private bool _isButtonPressed = false;

    [Header("���� ��ŸƮ Skip ����")]
    [Tooltip("��ư ���� �ð�")]
    [SerializeField] private float _buttonPressDuration = 0.0f;
    [Tooltip("��ư�� ������ �ϴ� �ð�")]
    [SerializeField] private float _requiredHoldTime = 2.0f;
    [Tooltip("Firebase ���� �̷� ����")]
    [SerializeField] private bool _isFirebaseUser = false;
    [Tooltip("���� ��� Ȯ�� ����")]
    [SerializeField] private bool _isVideoPlaying = false;

    [SerializeField] private string[] _gameInstructions = new string[]
    {
        "Press B key",
        "�ѹ��̶� ���� �ϼ̴ٸ� \n B key�� 1�ʵ��� �����ּ���. \nIf you have logged in even once, \npress the B key for 1 seconds. ",
        "10",
        "9",
        "8",
        "7",
        "6",
        "5",
        "4",
        "3",
        "2",
        "1",
        "Next stage in Press B Button..."
    };

    private void Start()
    {
        introPanel.SetActive(true);
        introText.text = _gameInstructions[_currentStep];

        if (videoPlayerObject)
        {
            videoPlayer = videoPlayerObject.GetComponent<VideoPlayer>();
            videoPlayerObject.SetActive(false);
            rawImage.SetActive(false);
            videoPlayer.loopPointReached += OnVideoEnd;
        }

        CheckPlayUser();

        if (skipGauge)
        {
            skipGauge.fillAmount = 0.0f;
        }
    }

    private void Update()
    {
        InputDevice rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        // ��ư�� ���� �������� Ȯ��

        // A key
        if (rightController.TryGetFeatureValue(CommonUsages.primaryButton, out bool isAPressed)) // A ��ư �Է�
        {
            if (isAPressed && !_isVideoPlaying)
            {
                PlayVideo();
            }
        }

        // B key
        if (rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool isPressed))
        {
            if (isPressed) // ������ ���۵Ǿ��� ���� ó��
            {
                _buttonPressDuration += Time.deltaTime;

                if(_isFirebaseUser)
                    UpdateSkipGauge();

                if (!_isButtonPressed)
                {
                    _isButtonPressed = true; // ��ư ���� ���� ���
                    ShowNextInstruction();
                }

                if (_buttonPressDuration >= _requiredHoldTime && _isFirebaseUser)
                {
                    SkipToLobby();
                }
            }
            else if (!isPressed) // ��ư�� �������� �� ���� �ʱ�ȭ
            {
                _isButtonPressed = false;
                _buttonPressDuration = 0f;
                skipGauge.fillAmount = 0f;
            }
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.A) && !_isVideoPlaying)
        {
            PlayVideo();
        }

        if (Input.GetKey(KeyCode.B))
        {
            isPressed = true;
            if (isPressed)
            {
                _buttonPressDuration += Time.deltaTime;

                if (_isFirebaseUser)
                    UpdateSkipGauge();

                if (!_isButtonPressed)
                {
                    _isButtonPressed = true;
                    ShowNextInstruction();
                }

                if (_buttonPressDuration >= _requiredHoldTime && _isFirebaseUser)
                {
                    SkipToLobby();
                }
            }
        }
        else if (Input.GetKeyUp(KeyCode.B))
        {
            isPressed = false;
            if (!isPressed)
            {
                _isButtonPressed = false;
                _buttonPressDuration = 0f;
                skipGauge.fillAmount = 0f;
            }
        }
#endif

        if (videoPlayerObject.activeSelf)
        {
            FollowHMD();
        }
    }

    private void PlayVideo()
    {
#if UNITY_EDITOR
        Debug.Log("AŰ�� ���Ƚ��ϴ�. ������ ����մϴ�.");
#endif
        if (videoPlayerObject != null)
        {
            rawImage.SetActive(true);
            videoPlayerObject.SetActive(true);
            videoPlayer.Play();
            _isVideoPlaying = true;
        }

        // �ȳ� �ؽ�Ʈ �����
        introPanel.SetActive(false);
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
#if UNITY_EDITOR
        Debug.Log("MP4 ���� ��� �Ϸ�.");
#endif
        if (videoPlayerObject)
        {
            rawImage.SetActive(false);
            videoPlayerObject.SetActive(false);
        }

        if (nickNameInputField)
        {
            nickNameInputField.SetActive(true);
        }

        _isVideoPlaying = false;
    }

    // HMD�� ��������� ����
    private void FollowHMD()
    {
        if (videoPlayerObject)
        {
            // HMD ��ġ�� �������� ���� ȭ���� ��ġ�� ����
            Vector3 cameraPosition = Camera.main.transform.position;
            Quaternion cameraRotation = Camera.main.transform.rotation;

            // ���� ȭ���� HMD �տ� ����
            Vector3 offsetPosition = cameraPosition + cameraRotation * Vector3.forward * videoDistance;
            videoPlayerObject.transform.position = offsetPosition;

            // HMD �������� ȭ���� �׻� �ٶ󺸵��� ����
            videoPlayerObject.transform.rotation = Quaternion.LookRotation(videoPlayerObject.transform.position - cameraPosition);
        }
    }

    private void ShowNextInstruction()
    {
        _currentStep++;
        if (_currentStep < _gameInstructions.Length)
        {
            introText.text = _gameInstructions[_currentStep];
        }
        else
        {
            Debug.Log("Ʃ�丮�� ������ �̵�.");
            SceneManager.LoadScene("02_Tutorial");
        }
    }

    private void SkipToLobby()
    {
        Debug.Log("�ѹ� ������ ������ �κ� ������ �̵�!");

        // ���� ��� ���̶�� �ߴ� �� ����
        if (videoPlayer && videoPlayer.isPlaying)
        {
            Debug.Log("���� ��� �ߴ� �� ����...");
            videoPlayer.Stop(); // ��� �ߴ�
            rawImage.SetActive(false);
            videoPlayerObject.SetActive(false); // ������Ʈ ��Ȱ��ȭ
        }

        SceneLoader.LoadSceneWithLoading("03_Lobby");
    }

    private void UpdateSkipGauge()
    {
        skipGauge.fillAmount = Mathf.Clamp01(_buttonPressDuration / _requiredHoldTime);
    }

    private void CheckPlayUser()
    {
        string userId = FirebaseManager.Instance?.GetUserId();

        if (!string.IsNullOrEmpty(userId))
        {
            Debug.Log($"Firebase ���� Ȯ�� �Ϸ�: {userId}");
            _isFirebaseUser = true;
        }
        else
        {
            Debug.Log("Firebase ������ �ƴմϴ�.");
            _isFirebaseUser = false;
        }
    }
}
