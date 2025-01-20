using PhotonHashtable = ExitGames.Client.Photon.Hashtable;
using TMPro;
using UnityEngine;
using Fusion;
using static FirebaseManager;
using System;

public class PlayerInfo : NetworkBehaviour
{
    [Header("UI References")]
    public TMP_Text nickNameText;
    public TMP_Text stageText;
    public TMP_Text starText;
    public GameObject infoPanel;

    private int stars;
    [Networked] public string PlayerNickName { get; set; }
    [Networked] public string HighStage { get; set; }

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            InitializePlayerInfo();
        }
        UpdateUI();
    }

    private void InitializePlayerInfo()
    {
        PlayerNickName = FirebaseManager.Instance.GetNickName();
        HighStage = FirebaseManager.Instance.GetHighStage();
        if (Enum.TryParse(HighStage, out E_StageMode stageMode))
        {
            int stageID = (int)stageMode;
            StageData stageData = FirebaseManager.Instance.GetCachedStageData(stageID);

            if (stageData != null)
            {
                stars = stageData.stars;
            }
            else
            {
                Debug.LogWarning($"�ְ� �������� {HighStage}�� StageData�� ã�� �� �����ϴ�.");
                stars = 0;
            }
        }
        else
        {
            Debug.LogWarning($"HighStage '{HighStage}'�� �Ľ��� �� �����ϴ�.");
            stars = 0;
        }
    }

    public void UpdateUI()
    {
        if (nickNameText)
            nickNameText.text = $"{PlayerNickName}";

        if (stageText)
            stageText.text = $"�ְ� �������� : {HighStage}";

        if (starText)
            starText.text = $"Stars Score : {stars}";
    }

    public void ShowInfo()
    {
        if (infoPanel)
            infoPanel.SetActive(true);
    }

    public void HideInfo()
    {
        if (infoPanel)
            infoPanel.SetActive(false);
    }
}
