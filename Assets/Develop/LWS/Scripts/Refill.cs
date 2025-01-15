using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using System.Collections;

public class Refill : MonoBehaviourPun
{
    [Header("������ ������ & ����")]
    [SerializeField] GameObject _refillPrefab; // ������� ������
    [SerializeField] int _maxCount = 10;       // �ִ� ���� Ƚ��
    [SerializeField] string _triggerZoneName;
    [SerializeField] List<int> idList = new List<int>();

    private int _curCount = 0;      // ���� �� �� �����ߴ���
    private Vector3 _originalPos;       // ���� ��ġ

    private void Start()
    {
        // ������Ʈ�� �ʱ� ��ġ
        _originalPos = transform.position;
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log($"{gameObject} <-> {other.gameObject} Ʈ���� Ż��");
        if (other.gameObject.CompareTag(_triggerZoneName))
        {
            Debug.Log("Ʈ���� �� Ż��");
            if (!PhotonNetwork.IsMasterClient)
                return;

            PhotonView objectID = gameObject.GetComponent<PhotonView>();
            if (idList.Contains(objectID.ViewID))
                return;

            StartCoroutine(TrySpawnRefill());
            idList.Add(objectID.ViewID);
        }
    }

    private IEnumerator TrySpawnRefill()
    {
        // 10ȸ �̻��̸� ����� x
        if (_curCount >= _maxCount)
        {
            Debug.Log("�߰� ��ȯ x");
            yield break;
        }
        _curCount++;

        yield return new WaitForSeconds(2f);

        GameObject NewObject = PhotonNetwork.Instantiate(_refillPrefab.name, _originalPos, Quaternion.identity);
        Refill refill = NewObject.GetComponent<Refill>();
        refill._curCount = _curCount;
        refill.idList = idList;

        Debug.Log($"���� Ƚ�� {_curCount} / �ƽ� Ƚ�� {_maxCount}");
    }

    private void OnDestroy()
    {
        PhotonView objectID = gameObject.GetComponent<PhotonView>();
        if (idList.Contains(objectID.ViewID))
        {
            idList.Remove(objectID.ViewID);
            //Debug.Log($"����Ʈ���� {objectID} ����");
        }
    }
}