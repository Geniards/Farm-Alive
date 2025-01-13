using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CorrespondentUpdater : UIBinder
{
    public int correspondentID;

    private void Start()
    {
        SubscribeTruck();
    }

    private void SubscribeTruck()
    {
        QuestManager.Instance.OnTruckUpdated.AddListener(UpdateUI);
    }

    private void UpdateUI(List<QuestManager.Quest> questList, List<TruckQuest> truckList)
    {
        if (QuestManager.Instance.questsList.Count < 0)
            return;

        for (int i = 0; i < questList.Count; i++)
        {
            string limitTime = $"Info{i + 1}_LimitTime";
            string NPCImage = $"Info{i + 1}_NPCImage";
            string NPCName = $"Info{i + 1}_NPCName";
            string slot1 = $"Info{i + 1}_Slot1";
            string slot2 = $"Info{i + 1}_Slot2";
            string slot3 = $"Info{i + 1}_Slot3";

            GetUI<TextMeshProUGUI>(limitTime).text = GetLimitTime(questList[i].questTimer);
            // �̹��� ����
            GetUI<TextMeshProUGUI>(NPCName).text = CSVManager.Instance.Correspondents[truckList[i].correspondentId].correspondent_name;
            GetUI<QuestItemSlot>(slot1).OnUpdate(questList, i, 0);
            GetUI<QuestItemSlot>(slot2).OnUpdate(questList, i, 1);
            GetUI<QuestItemSlot>(slot3).OnUpdate(questList, i, 2);
        }



        for (int i = 0; i < QuestManager.Instance.questCount; i++)
        {
            string displayText = "";

            QuestManager.Quest quest = questList[i];
            displayText += $"<b>{i + 1}��° {quest.questName}</b>\n";

            foreach (var item in quest.requiredItems)
            {
                displayText += $"�۹� �̸� : {item.itemPrefab.name}\n\t-�ʿ��� ���� : {item.requiredcount}\n\t-���� ����: {(item.isSuccess ? "�Ϸ�" : "�̿Ϸ�")})\n";
            }

            displayText += $"����Ʈ �Ϸ� ����: {(quest.isSuccess ? "�Ϸ�" : "�̿Ϸ�")}\n\n";

            QuestManager.Instance.truckList[i].questText.text = displayText;
        }
    }

    private string GetLimitTime(float seconds)
    {
        int min = (int)(seconds / 60f);
        int sec = (int)(seconds % 60f);

        return $"{min:00}:{sec:00}";
    }
}
