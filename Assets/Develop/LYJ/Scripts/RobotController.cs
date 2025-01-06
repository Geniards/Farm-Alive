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
    [SerializeField] private float followSpeed; // ���󰡴� �ӵ�
    [SerializeField] private float followDistance; // ���� �ּ� �Ÿ�

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
            if (distance > followDistance)
            {
                // �κ��� Ÿ�� �÷��̾� �������� ���� �ӵ��� �̵�
                transform.position = Vector3.MoveTowards(transform.position, _targetPlayer.position, followSpeed * Time.deltaTime);

                // �κ��� �׻� �÷��̾ �ٶ󺸵��� ȸ��
                Vector3 direction = (_targetPlayer.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5.0f);
            }
        }
    }

    private void OnMoveButtonClicked()
    {
        // ���� �÷��̾��� GameObject�� ��������
        GameObject localPlayerObject = GameObject.FindWithTag("Player");
        PhotonView localPlayerPhotonView = localPlayerObject.GetComponent<PhotonView>();
        int photonViewID = localPlayerPhotonView.ViewID;

        // PhotonViewID�� RPC�� ��� Ŭ���̾�Ʈ�� ����ȭ
        photonView.RPC(nameof(SyncTargetPlayer), RpcTarget.AllBuffered, photonViewID);
    }

    [PunRPC]
    private void SyncTargetPlayer(int photonViewID)
    {
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
                break;
            }
        }
    }
}
