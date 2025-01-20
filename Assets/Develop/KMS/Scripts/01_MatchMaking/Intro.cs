using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.XR;

public class Intro : MonoBehaviour
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


    private void Start()
    {
        introPanel.SetActive(true);
        introText.text = "���� ��Ʈ�ѷ��� A Ű�� �����ּ���";

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
            if (isAPressed && !_isVideoPlaying && !nickNameInputField.activeSelf)
            {
                introText.text = "";
                introPanel.transform.position = new Vector3(introPanel.transform.position.x, introPanel.transform.position.y - 1, introPanel.transform.position.z);
                PlayVideo();
            }
        }

        // B key
        if (rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool isPressed))
        {
            if (isPressed) // ������ ���۵Ǿ��� ���� ó��
            {
                _buttonPressDuration += Time.deltaTime;

                if(FirebaseManager.Instance.GetNickName() != "")
                    UpdateSkipGauge();

                if (!_isButtonPressed)
                {
                    _isButtonPressed = true;
                }

                if (_buttonPressDuration >= _requiredHoldTime && FirebaseManager.Instance.GetNickName() != "")
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

        if (videoPlayerObject.activeSelf)
        {
            FixHMD();

            if (FirebaseManager.Instance.GetNickName() != "")
                introText.text = $"{FirebaseManager.Instance.GetNickName()}�� ���� ��Ʈ�ѷ��� BŰ�� 1�ʵ��� ������ ��ø� Skip�� �����մϴ�.";
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
            SoundManager.Instance.PlayBGM("Intro", 0.4f);
        }
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
#if UNITY_EDITOR
        Debug.Log("MP4 ���� ��� �Ϸ�.");
#endif
        if (videoPlayerObject)
        {
            introText.gameObject.SetActive(false);
            rawImage.SetActive(false);
            videoPlayerObject.SetActive(false);
        }

        if (nickNameInputField)
        {
            nickNameInputField.SetActive(true);
            nickNameInputField.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0.5f);
        }

        _isVideoPlaying = false;
    }

    private void FixHMD()
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

    private void SkipToLobby()
    {
        Debug.Log("�ѹ� ������ ������ �κ� ������ �̵�!");
        MessageDisplayManager.Instance.ShowMessage($"{FirebaseManager.Instance.GetNickName()}�� �ٽ� �������ּż� �����մϴ�.", 0.5f, 3f);

        // ���� ��� ���̶�� �ߴ� �� ����
        if (videoPlayer && videoPlayer.isPlaying)
        {
            introText.gameObject.SetActive(false);
            Debug.Log("���� ��� �ߴ� �� ����...");
            videoPlayer.Stop(); // ��� �ߴ�
            SoundManager.Instance.StopBGM();
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
