using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public List<Text> questText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void UpdateQuestUI(List<QuestManager.Quest> questList)
    {
        if (QuestManager.Instance.questsList.Count < 0)
            return;

        for (int i = 0; i < questList.Count; i++)
        {
            string displayText = "";

            QuestManager.Quest quest = questList[i];
            displayText += $"<b>{i + 1}��° {quest.questName}</b>\n";

            foreach (var item in quest.requiredItems)
            {
                displayText += $"�۹� �̸� : {item.itemPrefab.name}\n\t-�ʿ��� ���� : {item.requiredcount}\n\t-���� ����: {(item.isSuccess ? "�Ϸ�" : "�̿Ϸ�")})\n";
            }

            displayText += $"����Ʈ �Ϸ� ����: {(quest.isSuccess ? "�Ϸ�" : "�̿Ϸ�")}\n\n";

            questText[i].text = displayText;
        }
    }
}