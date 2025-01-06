using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class RobotController : MonoBehaviour
{
    PhotonView photonView;

    private Canvas _canvas;
    private Button _moveButton;

    private Transform _targetPlayer; // �κ��� ���� ����
    private int targetPhotonViewID = -1; // ���� ������ PhotonViewID
    [SerializeField] private float _followSpeed; // ���󰡴� �ӵ�
    [SerializeField] private float _followDistance; // ���� �ּ� �Ÿ�

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();

        _canvas = GetComponentInChildren<Canvas>();
        _moveButton = GetComponentInChildren<Button>();

        _moveButton.onClick.AddListener(OnMoveButtonClicked);
    }

    private void Update()
    {
        if (_targetPlayer != null)
        {
            // �κ��� �÷��̾� ������ �Ÿ� ���
            float distance = Vector3.Distance(transform.position, _targetPlayer.position);

            // �κ��� �÷��̾���� �ּ� �Ÿ����� �ָ� ������ ���� ���� �̵�
            if (distance > _followDistance)
            {
                // �κ��� Ÿ�� �÷��̾� �������� ���� �ӵ��� �̵�
                transform.position = Vector3.MoveTowards(transform.position, _targetPlayer.position, _followSpeed * Time.deltaTime);

                // �κ��� �׻� �÷��̾ �ٶ󺸵��� ȸ��
                Vector3 direction = (_targetPlayer.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5.0f);
            }
        }
    }

    private void OnMoveButtonClicked()
    {
        Debug.Log("��ư�� ���Ƚ��ϴ�!");

        // ���� �÷��̾��� GameObject ��������
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            PhotonView view = player.GetComponent<PhotonView>();
            if (view != null && view.IsMine) // ���� �÷��̾� Ȯ��
            {
                int photonViewID = view.ViewID;

                Debug.Log($"���� �÷��̾� PhotonViewID: {photonViewID}");

                // ��ư�� ���� Ŭ���̾�Ʈ�� PhotonViewID�� ��� Ŭ���̾�Ʈ�� ����ȭ
                photonView.RPC(nameof(SyncTargetPlayer), RpcTarget.AllBuffered, photonViewID);
                return;
            }
        }

        Debug.LogError("���� �÷��̾��� PhotonView�� ã�� �� �����ϴ�!");
    }

    [PunRPC]
    private void SyncTargetPlayer(int photonViewID)
    {
        if (targetPhotonViewID == photonViewID)
        {
            // �̹� ������ ���� ����̶�� �ߺ� ó������ ����
            Debug.Log($"�κ��� �̹� �÷��̾� {photonViewID} �� ���� ���Դϴ�.");
            return;
        }

        // Ÿ�� PhotonViewID ������Ʈ
        targetPhotonViewID = photonViewID;

        // PhotonViewID�� Ÿ�� ������ Transform ��������
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player"); // ��� �÷��̾� �˻� (Player �±� �ʿ�)
        foreach (GameObject player in players)
        {
            PhotonView view = player.GetComponent<PhotonView>();
            if (view != null && view.ViewID == targetPhotonViewID)
            {
                _targetPlayer = player.transform;
                Debug.Log($"�κ��� �÷��̾� {targetPhotonViewID} �� ���󰩴ϴ�.");
                break;
            }
        }
    }
}
