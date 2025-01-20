using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public class PlantGround : MonoBehaviourPun
{
    public UnityEvent<int, Crop.E_CropState> OnMyPlantUpdated;

    public int section;
    public int ground;

    [SerializeField] private int _digCount; // �ʿ� ���� Ƚ��
    private int _currentDigCount = 0; // ���� ���� Ƚ��
    private bool _isInteractable = true; // ��ȣ�ۿ� ���� ����

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
        //Debug.Log($"���� Ƚ��: {_currentDigCount} / {_digCount}");

        if (_currentDigCount >= _digCount)
        {
            Transform disappearGround = transform.Find("DisappearingGround");
            if (disappearGround != null)
            {
                Destroy(disappearGround.gameObject);
                //Debug.Log("DisappearingGround�� �����Ǿ����ϴ�.");
            }

            _isInteractable = false; // �߰� ���� ����
        }
    }

    /// <summary>
    /// ���� ���� Ư�� �Ĺ��� �ɾ��� �� �ִ��� Ȯ��
    /// </summary>
    public bool CanPlant(Crop plant)
    {
        if (plant == null) return false;

        // �Ĺ��� �䱸 ���� Ƚ���� ���� DigCount ��
        return plant.DigCount <= _digCount;
    }
}
