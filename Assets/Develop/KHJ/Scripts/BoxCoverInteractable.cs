using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BoxCoverInteractable : XRGrabInteractable
{
    [Header("�ڽ� ��ü")]
    [SerializeField] private GameObject _body;

    [Header("�ڽ� Ŀ��")]
    [Tooltip("RigidBody")]
    [SerializeField] private Rigidbody _rigid;
    [Tooltip("�Ӱ� ���� ����")]
    [SerializeField] private float _angleRange;

    private Rigidbody _bodyRigid;

    [field: SerializeField]
    public bool IsOpen {  get; private set; }

    protected override void Awake()
    {
        base.Awake();

        _bodyRigid = _body.GetComponent<Rigidbody>();
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        _bodyRigid.isKinematic = true;
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        if (!CheckOpen())
        {
            Close();
        }

        _bodyRigid.isKinematic = false;
    }

    private bool CheckOpen()
    {
        

        return IsOpen;
    }

    private void Close()
    {
        _rigid.isKinematic = true;
        transform.rotation = Quaternion.identity;
    }
}