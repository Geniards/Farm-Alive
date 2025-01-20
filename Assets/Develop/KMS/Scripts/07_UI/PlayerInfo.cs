using Fusion;
using System;
using TMPro;
using UnityEngine;

public class PlayerInfo : NetworkBehaviour
{
    [Header("UI References")]
    public TMP_Text nickNameText;
    public TMP_Text stageText;
    public TMP_Text starText;

    [Networked] public string PlayerNickName { get; set; }
    [Networked] public string HighStage { get; set; }
    [Networked] public int Stars { get; set; }

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            InitializePlayerInfo();
        }
        UpdateUI();
    }

    /// <summary>
    /// �÷��̾� ������ �ʱ�ȭ
    /// </summary>
    private void InitializePlayerInfo()
    {
        PlayerNickName = FirebaseManager.Instance.GetNickName();
        HighStage = FirebaseManager.Instance.GetHighStage();

        if (Enum.TryParse(HighStage, out E_StageMode stageMode))
        {
            int stageID = (int)stageMode;
            var stageData = FirebaseManager.Instance.GetCachedStageData(stageID);

            Stars = stageData != null ? stageData.stars : 0;
        }
        else
        {
            Debug.LogWarning($"HighStage '{HighStage}'�� �Ľ��� �� �����ϴ�.");
            Stars = 0;
        }
    }

    /// <summary>
    /// UI�� ������Ʈ.
    /// </summary>
    public void UpdateUI()
    {
        if (nickNameText)
            nickNameText.text = $"{PlayerNickName}";

        if (stageText)
            stageText.text = $"�ְ� ��������: {HighStage}";

        if (starText)
            starText.text = $"��Ÿ ����: {Stars}";
    }
}
