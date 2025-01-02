using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
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
    }

    [SerializeField] public List<Quest> questsList = new List<Quest>();
    [SerializeField] public List<TruckQuest> truckList = new List<TruckQuest>();
    [SerializeField] public GameObject[] itemPrefabs;
    [SerializeField] public Quest currentQuest;
    [SerializeField] public int maxItemCount;

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
        photonView.RPC(nameof(QuestStart), RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void QuestStart()
    {
        maxItemCount = 0;
        if (PhotonNetwork.IsMasterClient)
        {
            //int rand = Random.Range(1, itemPrefabs.Length);
            int rand = Random.Range(2, 3);

            List<int> randomPrefabIndexes = new List<int>();
            int[] choseIndex = new int[rand];

            // ������ ���ȭ
            for (int i = 0; i < itemPrefabs.Length; i++)
            {
                randomPrefabIndexes.Add(i);
            }

            // ������ ���� ����
            int checkItemLength = 0;
            int[] maxItemCounts = new int[rand];
            for (int i = 0; i < maxItemCounts.Length; i++)
            {
                //maxItemCounts[i] = Random.Range(1, 15);
                maxItemCounts[i] = 1;
                maxItemCount += maxItemCounts[i];
                checkItemLength++;
                if (maxItemCount >= 30)
                {
                    int deleteCount = maxItemCounts.Max();
                    maxItemCount -= 30;

                    for (int j = 0; j < maxItemCounts.Length; j++)
                    {
                        if (maxItemCounts[j] == deleteCount)
                        {
                            if (maxItemCounts[j] > maxItemCount)
                            {
                                maxItemCounts[j] -= maxItemCount;
                            }
                        }
                    }
                    break;
                }
                    
            }

            int[] itemCounts = new int[checkItemLength];
            for (int i = 0; i < itemCounts.Length; i++)
            {
                itemCounts[i] = maxItemCounts[i];
            }

            // ������ ����
            for (int i = 0; i < itemCounts.Length; i++)
            {
                int randomIndex = Random.Range(0, randomPrefabIndexes.Count);
                choseIndex[i] = randomPrefabIndexes[randomIndex];
                randomPrefabIndexes.RemoveAt(randomIndex);

            }

            photonView.RPC(nameof(SetQuest), RpcTarget.AllBuffered, "�ù�����", itemCounts.Length, choseIndex, itemCounts);
        }
    }

    [PunRPC]
    public void SetQuest(string questName,int count, int[] itemIndexes, int[] itemCounts)
    {
        currentQuest = new Quest
        {
            questName = questName,
            requiredItems = new List<RequiredItem>()
        };

        for (int i = 0; i < count; i++)
        {
            GameObject itemPrefab = itemPrefabs[itemIndexes[i]];            
            int requiredCount = itemCounts[i];
            currentQuest.requiredItems.Add(new RequiredItem(itemPrefab, requiredCount));
        }

        questsList.Add(currentQuest);
        UpdateUI();
    }

    private void UpdateUI()
    {
        UIManager.Instance.UpdateQuestUI(questsList, questsList.Count-1);
    }

    public void CountUpdate(int id, int number, int count)
    {
        Debug.Log("ī��Ʈ ������Ʈ");
        photonView.RPC(nameof(CountCheck), RpcTarget.AllBuffered, id, number, count);
    }

    [PunRPC]
    private void CountCheck(int id, int number, int count)
    {
        Debug.Log("ī��Ʈ ����");
        questsList[id].requiredItems[number].requiredcount -= count;

        if (questsList[id].requiredItems[number].requiredcount <= 0)
        {
            Debug.Log("��ǰ�Ϸ�");
            //SuccessQuest(id, number);
            Debug.Log("����Ʈ ���� ���� ����ȭ!");
            questsList[id].requiredItems[number].isSuccess = true;

            int listNum = 0;
            List<int> listNums = new List<int>();
            foreach (QuestManager.Quest list in questsList)
            {
                for (int i = 0; i < list.requiredItems.Count; i++)
                {
                    if (list.requiredItems[i].isSuccess == false)
                        break;

                    if (i == list.requiredItems.Count - 1)
                    {
                        list.isSuccess = true;
                        listNums.Add(listNum);
                    }
                }
                listNum++;
            }

            if(listNums.Count != 0)
            {
                photonView.RPC(nameof(IsQuestComplete), RpcTarget.AllBuffered, listNums);
            }
            
            UpdateUI();
        }
    }

    /*public void SuccessQuest(int id, int number)
    {
        Debug.Log("����Ʈ �Ϸ�!");
        photonView.RPC(nameof(SuccessCheck), RpcTarget.AllBuffered, id, number);
    }

    [PunRPC]
    private void SuccessCheck(int id, int number)
    {
        Debug.Log("����Ʈ ���� ���� ����ȭ!");
        questsList[id].requiredItems[number].isSuccess = true;

        foreach (QuestManager.Quest list in questsList)
        {
            for (int i = 0; i < list.requiredItems.Count; i++)
            {
                if (list.requiredItems[i].isSuccess == false)
                    break;

                if (i == list.requiredItems.Count - 1)
                {
                    list.isSuccess = true;
                }
            }
        }

        UpdateUI();
    }*/

    [PunRPC]
    public void IsQuestComplete(List<int> listNums)
    {
        for (int i = 0; i < listNums.Count; i++)
        {
            questsList.RemoveAt(listNums[i]);
        }
        
        if(questsList.Count == 0)
        {
            SceneLoader.LoadSceneWithLoading("03_FusionLobby");
        }

        UpdateUI();
    }
}