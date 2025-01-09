using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class RobotController : MonoBehaviour
{
    PhotonView photonView;

    private Transform _targetPlayer; // �κ��� ���� ����
    private int targetPhotonViewID = -1; // ���� ������ PhotonViewID
    private bool isFollowing = false; // ���� ���� ������ Ȯ���ϴ� �÷���

    private Vector3 initialPosition; // �κ��� ó�� ��ġ ����

    [SerializeField] private float _followDistance = 3.0f; // ���� �ּ� �Ÿ�

    private NavMeshAgent navMeshAgent; // NavMeshAgent ������Ʈ

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        // �κ��� �ʱ� ��ġ ����
        initialPosition = transform.position;

        // ��ư ������Ʈ ã�� �� Ŭ�� �̺�Ʈ ���
        Button moveButton = GetComponentInChildren<Button>();
        if (moveButton != null)
        {
            moveButton.onClick.AddListener(OnMoveButtonClicked);
        }
    }

    private void Update()
    {
        // ���� ����� ������ �ƹ��͵� ���� ����
        if (_targetPlayer == null)
            return;

        // �κ��� �÷��̾� ������ �Ÿ� ���
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

    public void OnMoveButtonClicked()
    {
        // ���� �÷��̾��� PhotonViewID ��������
        PhotonView localPlayerPhotonView = GetLocalPlayerPhotonView();
        if (localPlayerPhotonView == null)
        {
            Debug.LogError("���� �÷��̾��� PhotonView�� ã�� �� �����ϴ�!");
            return;
        }

        int photonViewID = localPlayerPhotonView.ViewID;
        Debug.Log($"��ư�� ���� �÷��̾��� PhotonViewID: {photonViewID}");

        // ��ư�� ���� ���� �÷��̾��� PhotonViewID�� ������ ����
        photonView.RPC(nameof(SyncTargetPlayer), RpcTarget.All, photonViewID);
    }

    [PunRPC]
    private void SyncTargetPlayer(int photonViewID, PhotonMessageInfo info)
    {
        // �̹� ���� ���� �÷��̾ �� �� �� ��ư�� ���� ��� �ʱ� ��ġ�� ���ư�
        if (targetPhotonViewID == photonViewID && isFollowing)
        {
            _targetPlayer = null; // ���� ��� ����
            targetPhotonViewID = -1; // ��� ID �ʱ�ȭ
            isFollowing = false; // ���� ���� ��Ȱ��ȭ
            navMeshAgent.SetDestination(initialPosition); // �ʱ� ��ġ�� �̵�
            Debug.Log("�κ��� �ʱ� ��ġ�� ���ư��ϴ�.");
            return;
        }

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

    private PhotonView GetLocalPlayerPhotonView()
    {
        // ���� �÷��̾ �ش��ϴ� PhotonView ��������
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            PhotonView view = player.GetComponent<PhotonView>();
            if (view != null && view.IsMine) // ���� �÷��̾� Ȯ��
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
