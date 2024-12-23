using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ShovelInteractable : XRGrabInteractable
{
    private PlantGround _currentGround;
    private int _groundTriggerCount = 0;

    protected override void Awake()
    {
        base.Awake();
    }

    private void OnTriggerEnter(Collider other)
    {
        PlantGround ground = other.GetComponentInParent<PlantGround>();
        if (ground != null)
        {
            if (_currentGround == null)
            {
                _currentGround = ground;
                Debug.Log($"���� ����");
            }
            else
            {
                Debug.Log("���� ����: PlantGround ��ũ��Ʈ�� ����");
            }

            if (ground == _currentGround)
            {
                _groundTriggerCount++;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlantGround ground = other.GetComponentInParent<PlantGround>();
        if (ground != null && ground == _currentGround)
        {
            _groundTriggerCount--;

            if (_groundTriggerCount <= 0)
            {
                _currentGround.Dig();
                _currentGround = null;
                _groundTriggerCount = 0;
                Debug.Log("���� ����");
            }
        }
    }
}
