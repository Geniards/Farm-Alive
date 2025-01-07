using GameData;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static QuestManager;

public class QuestManager : MonoBehaviourPun
{
    public static QuestManager Instance;

    [System.Serializable]
    public class RequiredItem
    {
        public GameObject itemPrefab;
        public int requiredcount;
        public bool isSuccess;

        public RequiredItem(GameObject prefab, int itemCount)
        {
            itemPrefab = prefab;
            requiredcount = itemCount;
        }
    }

    [System.Serializable]
    public class Quest
    {
        public string questName;
        public List<RequiredItem> requiredItems;
        public bool isSuccess;
        public float questTimer;
    }

    [SerializeField] public List<Quest> questsList = new List<Quest>();
    [SerializeField] public List<TruckQuest> truckList = new List<TruckQuest>();

    [SerializeField] public GameObject[] itemPrefabs;
    [SerializeField] public Quest currentQuest;
    [SerializeField] TruckController truckController;
    [SerializeField] QuestController questController;

    [SerializeField] public int maxRequiredCount;
    [SerializeField] public int questCount;
    [SerializeField] public int itemTypeCount;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void FirstStart()
    {
        int stageLevel = 10;
        if (questsList.Count < 4)
            photonView.RPC(nameof(QuestStart), RpcTarget.AllBuffered, stageLevel - 1);
    }

    [PunRPC]
    public void QuestStart(int stageLevel)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < CSVManager.Instance.Stages_Correspondents[stageLevel].stage_corCount; i++)
            {
                maxRequiredCount = CSVManager.Instance.Correspondents_CropsCount[i].correspondent_stage[stageLevel];
                Debug.Log($"max�����۰����� : {maxRequiredCount}");

                int rand = CSVManager.Instance.Stages_Correspondents[stageLevel].stage_corCount;

                List<int> randomPrefabIndexes = new List<int>();
                int[] choseIndex = new int[rand];

                // ������ ���ȭ
                for (int j = 0; j < 3; j++)
                {
                    randomPrefabIndexes.Add(j);
                }

                // ������ ���� ����
                int checkItemLength = 0;
                int[] curItemCounts = new int[CSVManager.Instance.Correspondents_CropsType[i].correspondent_stage[stageLevel]];
                int curCount = 0;
                for (int j = 0; j < curItemCounts.Length; j++)
                {
                    curItemCounts[j] = Random.Range(1, 8);
                    curCount += curItemCounts[j];
                    checkItemLength++;
                    if (curCount >= maxRequiredCount)
                    {
                        int deleteCount = curItemCounts.Max();
                        curCount -= maxRequiredCount;

                        for (int a = 0; a < curItemCounts.Length; a++)
                        {
                            if (curItemCounts[a] == deleteCount)
                            {
                                if (curItemCounts[a] > curCount)
                                {
                                    curItemCounts[a] -= curCount;
                                }
                            }
                        }
                        break;
                    }

                    if (j == curItemCounts.Length - 1 && curCount < maxRequiredCount)
                    {
                        int temp = maxRequiredCount - curCount;
                        curItemCounts[j] += temp;
                    }
                }

                int[] itemCounts = new int[checkItemLength];
                for (int j = 0; j < itemCounts.Length; j++)
                {
                    itemCounts[j] = curItemCounts[j];
                }

                // ������ ����
                for (int j = 0; j < itemCounts.Length; j++)
                {
                    int randomIndex = Random.Range(0, randomPrefabIndexes.Count);
                    choseIndex[j] = randomPrefabIndexes[randomIndex];
                    randomPrefabIndexes.RemoveAt(randomIndex);
                }

                int corTemp = CSVManager.Instance.Stages_Correspondents[stageLevel].stage_corList[i] - 311;
                int corList = 0;
                float qTimer = 0;
                if (corTemp < 20)
                {
                    corList = corTemp / 10;
                    qTimer = CSVManager.Instance.Correspondents[corList].correspondent_timeLimit;
                }
                else
                {
                    corList = corTemp - 18;
                    qTimer = CSVManager.Instance.Correspondents[corList].correspondent_timeLimit;
                }

                photonView.RPC(nameof(SetQuest), RpcTarget.AllBuffered, "�ù�����", itemCounts.Length, choseIndex, itemCounts, corList, qTimer);
            }

        }
    }

    [PunRPC]
    public void SetQuest(string questName, int count, int[] itemIndexes, int[] itemCounts, int x, float qTimer)
    {
        currentQuest = new Quest
        {
            questName = questName,
            requiredItems = new List<RequiredItem>(),
            questTimer = qTimer
        };

        for (int i = 0; i < count; i++)
        {
            int y = CSVManager.Instance.Correspondents_RequireCrops[x].correspondent_cropID[itemIndexes[i]];
            y = 4 * ((y % 100 - y % 10) / 10 - 1) + y % 10;


            GameObject itemPrefab = itemPrefabs[y - 1];
            int requiredCount = itemCounts[i];
            currentQuest.requiredItems.Add(new RequiredItem(itemPrefab, requiredCount));
        }

        questsList.Add(currentQuest);

        if (PhotonNetwork.IsMasterClient)
            truckController.CreateTruck();
    }

    public void UpdateUI()
    {
        UIManager.Instance.UpdateQuestUI(questsList);
    }

    public void CountUpdate(int[] questId, int[] number, int[] count, int boxView)
    {
        Debug.Log("ī��Ʈ ������Ʈ");
        photonView.RPC(nameof(CountCheck), RpcTarget.AllBuffered, questId, number, count, boxView);
    }

    [PunRPC]
    private void CountCheck(int[] questId, int[] number, int[] count, int boxView)
    {
        Debug.Log("ī��Ʈ ����");

        for (int i = 0; i < questId.Length; i++)
        {
            questsList[questId[i]].requiredItems[number[i]].requiredcount -= count[i];

            if (questsList[questId[i]].requiredItems[number[i]].requiredcount <= 0)
            {
                Debug.Log("��ǰ�Ϸ�");
                Debug.Log("����Ʈ ���� ���� ����ȭ!");
                questsList[questId[i]].requiredItems[number[i]].isSuccess = true;
            }
        }

        List<int> completedIndexes = new List<int>();
        for (int i = 0; i < questsList.Count; i++)
        {
            bool allCompleted = true;

            for (int j = 0; j < questsList[i].requiredItems.Count; j++)
            {
                if (!questsList[i].requiredItems[j].isSuccess)
                {
                    allCompleted = false;
                    break;
                }
            }

            if (allCompleted)
            {
                questsList[i].isSuccess = true;
                completedIndexes.Add(i);
            }
        }

        PhotonView box = PhotonView.Find(boxView);
        if (box != null && box.IsMine)
        {
            PhotonNetwork.Destroy(box.gameObject);
        }

        if (completedIndexes.Count > 0)
        {
            int[] listArray = completedIndexes.ToArray();

            IsQuestComplete(listArray);
        }

        UpdateUI();
    }

    public void IsQuestComplete(int[] completedIndexes)
    {
        foreach (int index in completedIndexes.OrderByDescending(x => x))
        {
            questsList.RemoveAt(index);
            PhotonNetwork.Destroy(truckList[index].gameObject);
        }

        if (questsList.Count == 0)
        {
            GameSpawn gameSpawn = FindObjectOfType<GameSpawn>();
            gameSpawn.ReturnToFusion();
            //SceneLoader.LoadSceneWithLoading("03_FusionLobby");
        }
        else
        {
            UpdateUI();
        }
    }
}