using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class RobotController : MonoBehaviour
{
    PhotonView photonView;

    private Transform _targetPlayer; // �κ��� ���� ����
    private int targetPhotonViewID = -1; // ���� ������ PhotonViewID
    private bool isFollowing = false; // ���� ���� �������� ���� ����
    private bool isReturning = false; // �ʱ� ��ġ�� ���ư��� �������� ���� ����

    private Vector3 initialPosition; // �κ��� ó�� ��ġ ����
    private Quaternion initialRotation; // �κ��� ó�� ȸ���� ����

    [SerializeField] private float _followDistance = 3.0f; // ���� �ּ� �Ÿ�
    [SerializeField] private float _returnDistance = 0.1f; // �ʱ� ��ġ ��ó�� ���ƿ� �Ÿ�

    private NavMeshAgent navMeshAgent;

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
        if (!photonView.IsMine)
        {
            return;
        }

        if (isReturning)
        {
            // �ʱ� ��ġ�� �̵� ���� ��
            float distanceToInitial = Vector3.Distance(transform.position, initialPosition);
            if (distanceToInitial <= _returnDistance)
            {
                CompleteReturnToInitial();
            }
        }
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

    public void OnMoveButtonClicked()
    {
        PhotonView localPlayerPhotonView = GetLocalPlayerPhotonView();
        if (localPlayerPhotonView == null)
        {
            Debug.LogError("���� �÷��̾��� PhotonView�� ã�� �� �����ϴ�!");
            return;
        }

        int photonViewID = localPlayerPhotonView.ViewID;
        Debug.Log($"��ư�� ���� �÷��̾��� PhotonViewID: {photonViewID}");

        // ������ ��û
        photonView.RPC(nameof(RequestOwnership), RpcTarget.MasterClient, photonViewID);
    }

    [PunRPC]
    private void RequestOwnership(int photonViewID, PhotonMessageInfo info)
    {
        // ������ Ŭ���̾�Ʈ���� ������ ����
        photonView.TransferOwnership(info.Sender.ActorNumber);

        // ������ ���� �� ��� ����ȭ
        photonView.RPC(nameof(SyncTargetPlayer), RpcTarget.AllBuffered, photonViewID);
    }

    [PunRPC]
    private void SyncTargetPlayer(int photonViewID)
    {
        // �κ��� �����ڸ� ����
        if (!photonView.IsMine)
        {
            return;
        }

        // �̹� ���� ���� �÷��̾ ��ư�� �ٽ� ���� ��� �ʱ� ���·� ����
        if (targetPhotonViewID == photonViewID && isFollowing)
        {
            StartReturnToInitial();
            Debug.Log("�κ��� �ʱ� ��ġ�� ���� ���Դϴ�.");
            return;
        }

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

        // ��Ȯ�� �ʱ� ��ġ�� ȸ�������� ����
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
            if (view != null && view.IsMine)
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
            if (view != null && view.ViewID == photonViewID)
            {
                return player;
            }
        }

        return null;
    }
}
