using Fusion.LagCompensation;
using Photon.Pun;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(PhotonView))]
public class ButtonInteractable : XRBaseInteractable
{
    [System.Serializable]
    public class ButtonPressedEvent : UnityEvent { }
    [System.Serializable]
    public class ButtonReleasedEvent : UnityEvent { }

    // ��ư ������ �� �ۿ��� AudioClip
    // public AudioClip ButtonPressAudioClip;
    // public AudioClip ButtonReleaseAudioClip;

    [Header("��ư �̺�Ʈ")]
    // ��ư�� ������ ������ ���� �������� �� �����ϴ� �̺�Ʈ
    public ButtonPressedEvent _onButtonPressed;
    public ButtonReleasedEvent _onButtonReleased;

    private bool _isPressed = false;
    private PhotonView _view;

    protected override void Awake()
    {
        base.Awake();

        _view = GetComponent<PhotonView>();
    }

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);

        
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);


    }

    protected override void OnActivated(ActivateEventArgs args)
    {
        base.OnActivated(args);

        if (!_isPressed)
        {
            _isPressed = true;

            _view.RPC(nameof(RPC_OnButtonPressed), RpcTarget.All); 
        }
    }

    protected override void OnDeactivated(DeactivateEventArgs args)
    {
        base.OnDeactivated(args);

        if (_isPressed)
        {
            _isPressed = false;

            _view.RPC(nameof(RPC_OnButtonReleased), RpcTarget.All);
        }
    }


    [PunRPC]
    private void RPC_OnButtonPressed()
    {
        _onButtonPressed.Invoke();
    }

    [PunRPC]
    private void RPC_OnButtonReleased()
    {
        _onButtonReleased.Invoke();
    }
}