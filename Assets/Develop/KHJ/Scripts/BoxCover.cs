using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxCover : MonoBehaviour
{
    [Header("�ڽ� Ŀ��")]
    [SerializeField] private BoxCoverInteractable _front;
    [SerializeField] private BoxCoverInteractable _back;
    [SerializeField] private BoxCoverInteractable _right;
    [SerializeField] private BoxCoverInteractable _left;

    [Header("����")]
    public GameObject leftPoint;
    public GameObject rightPoint;
    public GameObject tape;

    public bool IsOpen {  get { return _front.IsOpen || _back.IsOpen || _left.IsOpen || _right.IsOpen; } }
    public bool IsPackaged { get; set; }
}
