using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HeadLightInteractable : XRGrabInteractable
{
    private Light _headLight;
    private Light _directionalLight;

    protected override void Awake()
    {
        base.Awake();

        if (_headLight == null)
        {
            _headLight = GetComponentInChildren<Light>();
            if (_headLight == null)
            {
                Debug.LogError("HeadLight�� �������� �ʽ��ϴ�.");
            }
        }

        if (_directionalLight == null)
        {
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

        HeadLightState();
    }

    private void Update()
    {
        HeadLightState();
    }

    private void HeadLightState()
    {
        if (_directionalLight == null || _headLight == null) return;

        if (_directionalLight.enabled)
        {
            _headLight.enabled = false;
        }
        else
        {
            _headLight.enabled = true;
        }
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
