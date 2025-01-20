using Photon.Pun;
using UnityEngine;

public class LightingManager : MonoBehaviourPun
{
    public static LightingManager Instance;

    [SerializeField][Range(0f, 1f)] private float _lightingIntensity = 1f;
    [SerializeField][Range(0f, 1f)] private float _reflectionIntensity = 1f;

    private Light[] _allLights;
    private Camera _mainCamera;
    private CameraClearFlags _defaultClearFlags;
    private Color _defaultBackgroundColor;

    private Color _blackoutColor = Color.black;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _allLights = FindObjectsOfType<Light>();

        _mainCamera = Camera.main;
        if (_mainCamera != null)
        {
            _defaultClearFlags = _mainCamera.clearFlags;
            _defaultBackgroundColor = _mainCamera.backgroundColor;
        }
        else
        {
            Debug.Log("Main Camera�� ã�� �� �����ϴ�.");
        }
    }

    private void Update()
    {
        RenderSettings.ambientIntensity = _lightingIntensity;
        RenderSettings.reflectionIntensity = _reflectionIntensity;
    }

    private void LateUpdate()
    {
        if (_mainCamera == null)
        {
            GameObject cameraOffset = GameObject.Find("Camera Offset");
            if (cameraOffset != null)
            {
                _mainCamera = cameraOffset.GetComponentInChildren<Camera>();
            }

            if (_mainCamera != null)
            {
                _defaultClearFlags = _mainCamera.clearFlags;
                _defaultBackgroundColor = _mainCamera.backgroundColor;
                Debug.Log("Main Camera ã�ҽ��ϴ�.");
            }
            else
            {
                Debug.Log("Main Camera�� ã�� �� �����ϴ�.");
            }
        }
    }


    /// <summary>
    /// ���� ȿ�� ����
    /// </summary>
    [PunRPC]
    public void SyncTriggerBlackout()
    {
        //Debug.Log("���� �߻�!");

        // 1. ��� Light ����
        foreach (Light light in _allLights)
        {
            light.enabled = false;
        }

        // 2. ȯ�� ���� ������ �ݻ� ���� 0���� ����
        RenderSettings.ambientIntensity = 0f;
        RenderSettings.reflectionIntensity = 0f;
        _lightingIntensity = 0f;
        _reflectionIntensity = 0f;

        // 3. ī�޶� ���� ���� (���� ���·� ��ȯ)
        if (_mainCamera != null)
        {
            _mainCamera.clearFlags = CameraClearFlags.SolidColor;
            _mainCamera.backgroundColor = _blackoutColor;
        }

        // 4. ��� HeadLightInteractable�� ������Ʈ �ѱ�
        EnableHeadlights();
    }

    /// <summary>
    /// ���� ȿ�� ����
    /// </summary>
    [PunRPC]
    public void SyncRecoverFromBlackout()
    {
        //Debug.Log("���� ����!");

        // 1. ��� Light �ѱ�
        foreach (Light light in _allLights)
        {
            light.enabled = true;
        }

        // 2. ȯ�� ���� ������ �ݻ� ���� ����
        RenderSettings.ambientIntensity = 1f;
        RenderSettings.reflectionIntensity = 1f;
        _lightingIntensity = 1f;
        _reflectionIntensity = 1f;

        // 3. ī�޶� ���� ����
        if (_mainCamera != null)
        {
            _mainCamera.clearFlags = _defaultClearFlags;
            _mainCamera.backgroundColor = _defaultBackgroundColor;
        }

        // 4. ��� HeadLightInteractable�� ������Ʈ ����
        DisableHeadlights();
    }

    /// <summary>
    /// ������ Ʈ���� (RPC ȣ��)
    /// </summary>
    public void StartBlackout()
    {
        photonView.RPC(nameof(SyncTriggerBlackout), RpcTarget.AllBuffered);
    }

    /// <summary>
    /// ���� ������ Ʈ���� (RPC ȣ��)
    /// </summary>
    public void EndBlackout()
    {
        photonView.RPC(nameof(SyncRecoverFromBlackout), RpcTarget.AllBuffered);
    }

    private void EnableHeadlights()
    {
        var headLights = FindObjectsOfType<HeadLightInteractable>();
        foreach (var headLight in headLights)
        {
            headLight.TriggerBlackout(); // ������Ʈ �ѱ�
        }
    }

    private void DisableHeadlights()
    {
        var headLights = FindObjectsOfType<HeadLightInteractable>();
        foreach (var headLight in headLights)
        {
            headLight.RecoverFromBlackout(); // ������Ʈ ����
        }
    }
}
