using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class RobotFollower : MonoBehaviour
{
    [Header("�κ� �̵� ����")]
    [Tooltip("������ �÷��̾�")]
    public Transform targetPlayer;

    [Tooltip("�÷��̾�� ������ �Ÿ�")]
    public float followDistance = 2.0f;

    [Tooltip("�κ��� �̵� �ӵ�")]
    public float speed = 3.5f;

    [Tooltip("�κ��� �ʱ� ��ġ")]
    public Transform initialPosition;

    [Tooltip("���� �̵� ����")]
    public Collider allowedArea;

    [Tooltip("��������")]
    [SerializeField] private bool _isFollowing = false;
    private NavMeshAgent _agent;
    private PhotonView _photonView;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _photonView = GetComponent<PhotonView>();

        if (!_photonView)
        {
            Debug.LogError("PhotonView�� �� ������Ʈ�� �����ϴ�. PhotonView�� �߰��ϼ���.");
        }

        _agent.speed = speed;
    }

    private void Update()
    {
        if (_isFollowing && targetPlayer)
        {
            Vector3 targetPosition = new Vector3(targetPlayer.position.x, transform.position.y, targetPlayer.position.z);
            float distance = Vector3.Distance(transform.position, targetPosition);

            if (allowedArea && !allowedArea.bounds.Contains(targetPosition))
            {
                StopFollowingAndReturnToInitial();
                return;
            }

            if (distance > followDistance)
            {
                SetDestination(targetPosition);
            }
            else
            {
                SetDestination(transform.position);
            }
        }
    }

    [PunRPC]
    public void StartFollowingRPC(int playerViewID)
    {
        PhotonView playerPhotonView = PhotonView.Find(playerViewID);
        if (playerPhotonView)
        {
            GameObject player = playerPhotonView.gameObject;
            targetPlayer = player.transform;
            _isFollowing = true;
            Debug.Log($"�κ��� {player.name}��(��) ���󰡱� �����մϴ�.");
        }
    }

    [PunRPC]
    public void StopFollowingRPC()
    {
        _isFollowing = false;
        targetPlayer = null;
        SetDestination(transform.position);
        Debug.Log("�κ��� ������ ������ϴ�.");
    }

    private void StopFollowingAndReturnToInitial()
    {
        _isFollowing = false;
        targetPlayer = null;

        if (initialPosition)
        {
            SetDestination(initialPosition.position);
            Debug.Log("�÷��̾ ���� ������ ������ϴ�. �κ��� �ʱ� ��ġ�� ���ư��ϴ�.");
        }
        else
        {
            Debug.LogWarning("�ʱ� ��ġ�� �������� �ʾҽ��ϴ�.");
        }
    }

    private void SetDestination(Vector3 destination)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            _agent.SetDestination(destination);
            _photonView.RPC("SyncDestination", RpcTarget.Others, destination);
        }
    }

    [PunRPC]
    private void SyncDestination(Vector3 destination)
    {
        _agent.SetDestination(destination);
    }
}
