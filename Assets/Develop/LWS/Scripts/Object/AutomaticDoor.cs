using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticDoor : MonoBehaviourPun
{
    [Tooltip("���� ���� ��ġ")]
    [SerializeField] Transform _destinationPos;

    private Vector3 _initPos;

    [SerializeField] float _moveSpeed = 5f;

    [Tooltip("���� ������ ���ð�")]
    [SerializeField] float _delay = 2f;

    private Coroutine _moveCoroutine;

    private void Start()
    {
        _initPos = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (_moveCoroutine != null)
                StopCoroutine(_moveCoroutine);

            _moveCoroutine = StartCoroutine(MoveDoor(_destinationPos.position));
        }
    }

    private IEnumerator MoveDoor(Vector3 targetPos)
    {
        while ((transform.position - targetPos).sqrMagnitude > 0.001f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, _moveSpeed * Time.deltaTime);
            yield return null;
        }

        // �̵� �Ϸ�
        _moveCoroutine = null;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_moveCoroutine != null)
                StopCoroutine(_moveCoroutine);

            _moveCoroutine = StartCoroutine(WaitAndCloseDoor());
        }
    }

    private IEnumerator WaitAndCloseDoor()
    {
        yield return new WaitForSeconds(_delay);

        _moveCoroutine = StartCoroutine(MoveDoor(_initPos));
    }
}