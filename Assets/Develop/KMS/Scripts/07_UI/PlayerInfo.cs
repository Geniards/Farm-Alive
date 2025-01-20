using System;
using TMPro;
using UnityEngine;
using Fusion;

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
            RpcSetPlayerInfo(PlayerNickName, HighStage, Stars);
        }
        else
        {
            RpcRequestPlayerInfo();
        }
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

        UpdateUI();
    }

    /// <summary>
    /// RPC�� ����Ͽ� �÷��̾� ������ ����ȭ
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RpcSetPlayerInfo(string playerName, string highStage, int stars)
    {
        PlayerNickName = playerName;
        HighStage = highStage;
        Stars = stars;

        UpdateUI();
    }

    /// <summary>
    /// ������ �÷��̾� ������ ��û
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RpcRequestPlayerInfo()
    {
        RpcSendPlayerInfo(PlayerNickName, HighStage, Stars);
    }

    /// <summary>
    /// Ŭ���̾�Ʈ�鿡�� �÷��̾� ���� ����
    /// </summary>
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcSendPlayerInfo(string playerName, string highStage, int stars)
    {
        PlayerNickName = playerName;
        HighStage = highStage;
        Stars = stars;

        RpcSetPlayerInfo(PlayerNickName, HighStage, Stars);
    }

    /// <summary>
    /// UI�� ������Ʈ
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