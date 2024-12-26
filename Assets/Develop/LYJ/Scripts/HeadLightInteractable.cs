using Photon.Pun;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HeadLightInteractable : XRGrabInteractable
{
    private PhotonView photonView;

    private Light _headLight;
    private Light _directionalLight;
    private Camera _mainCamera;

    [Range(0f, 1f)][SerializeField] private float _lightingIntensity = 1f;
    [Range(0f, 1f)][SerializeField] private float _reflectionIntensity = 1f;
    private Color _blackoutColor = Color.black;

    private CameraClearFlags _defaultClearFlags;
    private Color _defaultBackgroundColor;
    private bool _cameraInitialized = false; // ī�޶� �ʱ�ȭ ����

    protected override void Awake()
    {
        base.Awake();

        photonView = GetComponent<PhotonView>();

        _headLight = GetComponentInChildren<Light>();
        if (_headLight == null)
        {
            Debug.LogError("HeadLight�� �������� �ʽ��ϴ�.");
        }

        if (_headLight != null)
        {
            _headLight.enabled = false;
        }

        // Directional Light ã��
        Light[] lights = FindObjectsOfType<Light>();
        foreach (Light light in lights)
        {
            if (light.type == LightType.Directional)
            {
                _directionalLight = light;
                break;
            }
        }

        if (_directionalLight == null)
        {
            Debug.LogError("���� Directional Light�� �����ϴ�.");
        }
    }

    private void Update()
    {
        RenderSettings.ambientIntensity = _lightingIntensity;
        RenderSettings.reflectionIntensity = _reflectionIntensity;
    }

    private void LateUpdate()
    {
        if (!_cameraInitialized && _mainCamera == null)
        {
            GameObject cameraOffset = GameObject.Find("Camera Offset");
            if (cameraOffset != null)
            {
                _mainCamera = cameraOffset.GetComponentInChildren<Camera>();
            }

            if (_mainCamera != null)
            {
                // ī�޶� �ʱ�ȭ
                _defaultClearFlags = _mainCamera.clearFlags;
                _defaultBackgroundColor = _mainCamera.backgroundColor;
                Debug.Log("Main Camera ã�ҽ��ϴ�.");
                _cameraInitialized = true;
            }
            else
            {
                Debug.Log("Main Camera�� ã�� �� �����ϴ�.");
            }
        }
    }

    /// <summary>
    /// ���� �߻� �� ����
    /// </summary>
    public void TriggerBlackout()
    {
        if (photonView.IsMine)
        {
            photonView.RPC(nameof(SyncTriggerBlackout), RpcTarget.AllBuffered);
        }
    }

    /// <summary>
    /// ���� ���� �� ����
    /// </summary>
    public void RecoverFromBlackout()
    {
        if (photonView.IsMine)
        {
            photonView.RPC(nameof(SyncRecoverFromBlackout), RpcTarget.AllBuffered);
        }
    }

    /// <summary>
    /// ���� �߻� �� ����
    /// </summary>
    [PunRPC]
    public void SyncTriggerBlackout()
    {
        Debug.Log("���� �߻�!");

        // 1. Directional Light ����
        if (_directionalLight != null)
        {
            _directionalLight.enabled = false;
        }

        // 2. Environment Lighting Intensity Multiplier ���� (0���� ����)
        _lightingIntensity = 0f;
        _reflectionIntensity = 0f;
        RenderSettings.ambientIntensity = _lightingIntensity;
        RenderSettings.reflectionIntensity = _reflectionIntensity;

        // 3. Main Camera Clear Flags�� Solid Color�� ����
        if (_mainCamera != null)
        {
            _mainCamera.clearFlags = CameraClearFlags.SolidColor;
            _mainCamera.backgroundColor = _blackoutColor;
        }

        // 4. HeadLight �ѱ�
        EnableHeadlight();
    }

    /// <summary>
    /// ���� ���� �� ����
    /// </summary>
    [PunRPC]
    public void SyncRecoverFromBlackout()
    {
        Debug.Log("���� ����!");

        // 1. Directional Light �ѱ�
        if (_directionalLight != null)
        {
            _directionalLight.enabled = true;
        }

        _lightingIntensity = 1f;
        _reflectionIntensity = 1f;
        RenderSettings.ambientIntensity = _lightingIntensity;
        RenderSettings.reflectionIntensity = _reflectionIntensity;

        // 3. Main Camera Clear Flags�� Background ����
        if (_mainCamera != null)
        {
            _mainCamera.clearFlags = _defaultClearFlags;
            _mainCamera.backgroundColor = _defaultBackgroundColor;
        }

        // 4. HeadLight ����
        DisableHeadlight();
    }

    public void EnableHeadlight()
    {
        if (_headLight != null)
        {
            _headLight.enabled = true;
        }
    }

    public void DisableHeadlight()
    {
        if (_headLight != null)
        {
            _headLight.enabled = false;
        }
    }
}
