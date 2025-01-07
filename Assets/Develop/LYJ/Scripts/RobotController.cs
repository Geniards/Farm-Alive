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

    [SerializeField] private float _followDistance = 3.0f; // ���� �ּ� �Ÿ�

    private NavMeshAgent navMeshAgent; // NavMeshAgent ������Ʈ

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        navMeshAgent = GetComponent<NavMeshAgent>();

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
        // �̹� ���� ���̸� �������� ����
        if (isFollowing)
        {
            Debug.Log("�̹� ���� ���Դϴ�.");
            return;
        }

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
        // �̹� ������ ���� ����̶�� �ߺ� ó������ ����
        if (targetPhotonViewID == photonViewID)
        {
            Debug.Log($"�κ��� �̹� �÷��̾� {photonViewID} �� ���� ���Դϴ�.");
            return;
        }

        // PhotonViewID�� ������� �ش� �÷��̾��� Transform ��������
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
