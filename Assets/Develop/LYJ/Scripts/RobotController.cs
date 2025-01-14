using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class RobotController : MonoBehaviour
{
    PhotonView photonView;

    private Transform _targetPlayer; // �κ��� ���� ����
    private int targetPhotonViewID = -1; // ���� ������ PhotonViewID
    private int lastPhotonViewID = -1; // ���������� �����ߴ� PhotonViewID
    private bool isFollowing = false; // ���� ���� �������� ���� ����
    private bool isReturning = false; // �ʱ� ��ġ�� ���ư��� �������� ���� ����

    private Vector3 initialPosition; // �κ��� ó�� ��ġ ����
    private Quaternion initialRotation; // �κ��� ó�� ȸ���� ����

    [SerializeField] private float _followDistance = 3.0f; // ���� �ּ� �Ÿ�
    [SerializeField] private float _returnDistance = 0.1f; // �ʱ� ��ġ ��ó�� ���ƿ� �Ÿ�
    [SerializeField] private float cooldownTime = 3.0f; // ��ٿ� �ð�

    private NavMeshAgent navMeshAgent;
    private float lastButtonClickTime = -Mathf.Infinity; // ���������� ��ư�� ���� �ð�

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        // �κ��� �ʱ� ��ġ�� ȸ���� ����
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        Button moveButton = GetComponentInChildren<Button>();
        if (moveButton != null)
        {
            moveButton.onClick.AddListener(OnMoveButtonClicked);
        }
    }

    private void Update()
    {
        // �� �κ��� ���� �����ϴ� �κ��� �ƴ϶�� ����
        if (!photonView.IsMine)
        {
            return;
        }

        // �κ��� ���� ��ġ�� ���ư��� �ִٸ�
        if (isReturning)
        {
            // �ʱ� ��ġ�� �̵� ���� ��
            float distanceToInitial = Vector3.Distance(transform.position, initialPosition);
            if (distanceToInitial <= _returnDistance)
            {
                CompleteReturnToInitial();
            }
        }

        // �κ��� ���� ����� �ִٸ�
        else if (_targetPlayer != null)
        {
            // ���� ����� ������ �Ÿ� ���
            float distance = Vector3.Distance(transform.position, _targetPlayer.position);

            // �ּ� �Ÿ����� �ָ� ���� ���� �̵�
            if (distance > _followDistance)
            {
                navMeshAgent.SetDestination(_targetPlayer.position);
            }
            else
            {
                navMeshAgent.ResetPath();
            }
        }
    }

    // �κ� ��ư�� Ŭ������ ��
    public void OnMoveButtonClicked()
    {
        if (Time.time - lastButtonClickTime < cooldownTime)
        {
            Debug.Log("��ư�� �ʹ� ���� ���Ƚ��ϴ�. ��ٿ� ���Դϴ�.");
            return;
        }

        lastButtonClickTime = Time.time;

        // ���� �÷��̾��� PhotonViewID ��������
        PhotonView localPlayerPhotonView = GetLocalPlayerPhotonView();
        if (localPlayerPhotonView == null)
        {
            Debug.LogError("���� �÷��̾��� PhotonView�� ã�� �� �����ϴ�!");
            return;
        }

        int photonViewID = localPlayerPhotonView.ViewID;
        Debug.Log($"��ư�� ���� �÷��̾��� PhotonViewID: {photonViewID}");

        photonView.RPC(nameof(RPC_ButtonClick), RpcTarget.MasterClient, photonViewID);
    }

    [PunRPC]
    private void RPC_ButtonClick(int photonViewID, PhotonMessageInfo info)
    {
        if (photonViewID == targetPhotonViewID && photonViewID == lastPhotonViewID)
        {
            photonView.RPC(nameof(StartReturnToInitial), RpcTarget.AllBuffered);
        }
        else
        {
            photonView.TransferOwnership(info.Sender.ActorNumber);
            photonView.RPC(nameof(SyncTargetPlayer), RpcTarget.AllBuffered, photonViewID);
        }

        lastPhotonViewID = photonViewID;
    }

    // ������ Ŭ���̾�Ʈ���� ������ ��û
    [PunRPC]
    private void RequestOwnership(int photonViewID, PhotonMessageInfo info)
    {
        // ������ Ŭ���̾�Ʈ���� ��û�� �÷��̾�� ������ ����
        photonView.TransferOwnership(info.Sender.ActorNumber);

        // ������ ���� �� ��� �÷��̾�� �κ� Ÿ�� �÷��̾� ��� ����ȭ
        photonView.RPC(nameof(SyncTargetPlayer), RpcTarget.AllBuffered, photonViewID);
    }

    [PunRPC]
    private void SyncTargetPlayer(int photonViewID, PhotonMessageInfo info)
    {
        //���� Ÿ������ ��� �ִ� �κ��� �ƴ϶��
        if (!photonView.IsMine)
        {
            return;
        }

        //// �̹� ���� ���� �÷��̾ �� �� �� ��ư�� ���� ��� �ʱ� ���·� ����
        //if (targetPhotonViewID == photonViewID && isFollowing)
        //{
        //    StartReturnToInitial();
        //    Debug.Log("�κ��� �ʱ� ��ġ�� ���� ���Դϴ�.");
        //    return;
        //}

        // ���ο� PhotonViewID�� ������� �ش� �÷��̾��� Transform ��������
        GameObject targetObject = GetPlayerGameObjectByPhotonViewID(photonViewID);

        if (targetObject != null)
        {
            _targetPlayer = targetObject.transform; // ���� ��� ����
            targetPhotonViewID = photonViewID; // ���� ��� ID ����
            isFollowing = true; // ���� ���� Ȱ��ȭ
            Debug.Log($"�κ��� �÷��̾� {photonViewID} �� ���󰩴ϴ�.");
        }
        else
        {
            Debug.LogError($"�ش� PhotonViewID {photonViewID} �� ���� �÷��̾ ã�� �� �����ϴ�!");
        }
    }

    private void StartReturnToInitial()
    {
        _targetPlayer = null; // ���� ��� ����
        targetPhotonViewID = -1; // ��� ID �ʱ�ȭ
        isFollowing = false; // ���� ���� ��Ȱ��ȭ
        isReturning = true; // �ʱ� ��ġ�� ���ư��� ���� Ȱ��ȭ

        // �ʱ� ��ġ�� �̵�
        navMeshAgent.SetDestination(initialPosition);
    }

    private void CompleteReturnToInitial()
    {
        isReturning = false; // �ʱ� ��ġ�� ���ƿ��� ���� ��Ȱ��ȭ

        // NavMeshAgent ��� �ʱ�ȭ
        navMeshAgent.ResetPath();

        // �ʱ� ��ġ�� ȸ�������� ����
        transform.position = initialPosition;
        transform.rotation = initialRotation;

        Debug.Log("�κ��� �ʱ� ���·� �����Ǿ����ϴ�.");
    }

    private PhotonView GetLocalPlayerPhotonView()
    {
        // ���� �÷��̾ �ش��ϴ� PhotonView ��������
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            PhotonView view = player.GetComponent<PhotonView>();
            if (view != null && view.IsMine) // �� �÷��̾��� PhotonView ��ȯ
            {
                return view;
            }
        }

        return null;
    }

    private GameObject GetPlayerGameObjectByPhotonViewID(int photonViewID)
    {
        // PhotonViewID�� GameObject ã��
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            PhotonView view = player.GetComponent<PhotonView>();
            if (view != null && view.ViewID == photonViewID) // photonViewID �� ���ٸ�
            {
                return player;
            }
        }

        return null;
    }
}