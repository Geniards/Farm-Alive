using Photon.Pun;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlantGround : MonoBehaviourPun
{
    [SerializeField] private int _digCount; // �ʿ� ���� Ƚ��
    private int _currentDigCount = 0; // ���� ���� Ƚ��
    private bool _isInteractable = true; // ��ȣ�ۿ� ���� ����
    private XRSocketInteractor _socketInteractor;

    private void Awake()
    {
        _socketInteractor = GetComponentInChildren<XRSocketInteractor>();

        if (_socketInteractor != null)
        {
            _socketInteractor.enabled = false; // �ʱ� ���� ��Ȱ��ȭ
            _socketInteractor.hoverEntered.AddListener(OnHoverEntered); // ������ ���� �̺�Ʈ ����
        }
        else
        {
            Debug.LogWarning("XRSocketInteractor�� ã�� �� �����ϴ�.");
        }
    }

    /// <summary>
    /// ���� ���� �޼ҵ�
    /// </summary>
    public void Dig()
    {
        if (!_isInteractable) return;

        // Dig() ������ ��Ʈ��ũ���� ����ȭ
        photonView.RPC(nameof(SyncDig), RpcTarget.AllBuffered);
    }

    /// <summary>
    /// ���� ���� �޼ҵ�
    /// </summary>
    [PunRPC]
    public void SyncDig()
    {
        if (!_isInteractable) return;

        _currentDigCount++;
        Debug.Log($"���� Ƚ��: {_currentDigCount} / {_digCount}");

        if (_currentDigCount >= _digCount)
        {
            Transform disappearGround = transform.Find("DisappearingGround");
            if (disappearGround != null)
            {
                Destroy(disappearGround.gameObject);
                Debug.Log("DisappearingGround�� �����Ǿ����ϴ�.");
            }

            _isInteractable = false; // �߰� ���� ����

            // ���� ���ͷ��� Ȱ��ȭ (���� �˻�� OnHoverEntered���� ����)
            if (_socketInteractor != null)
            {
                _socketInteractor.enabled = true;
                _socketInteractor.showInteractableHoverMeshes = true;
                Debug.Log("���� ���ͷ��Ͱ� Ȱ��ȭ�Ǿ����ϴ�.");
            }
        }
    }

    // ������Ʈ�� ���� ������ ���� �� ���� �˻�
    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        // �����ٴ� ������Ʈ���� PlantDigCount ������Ʈ�� ������
        // TODO: �Ĺ� ��ũ��Ʈ�� �ڵ带 �ű�ٸ� �ؿ� �ڵ嵵 ������ �־�� ��
        Crop plant = args.interactableObject.transform.GetComponent<Crop>();

        if (plant == null)
        {
            _socketInteractor.enabled = false;
            return;
        }

        // ���� �˻�: ���� _digCount�� �Ĺ��� _plantDigCount�� ���ƾ� ��
        if (!CanPlant(plant))
        {
            _socketInteractor.enabled = false;
        }
        else
        {
            _socketInteractor.enabled = true;
        }
    }

    /// <summary>
    /// ���� ���� Ư�� �Ĺ��� �ɾ��� �� �ִ��� Ȯ��
    /// </summary>
    public bool CanPlant(Crop plant)
    {
        if (plant == null) return false;

        // �Ĺ��� �䱸 ���� Ƚ���� ���� DigCount ��
        return plant.DigCount == _digCount;
    }
}
