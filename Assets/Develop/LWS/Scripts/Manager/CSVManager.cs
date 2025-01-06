using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using GameData;
using Unity.Collections.LowLevel.Unsafe;

public class CSVManager : MonoBehaviour
{
    const string cropCSVPath = "https://docs.google.com/spreadsheets/d/1Sd5x4Mt1fnIVY-X97tvYx1lTE8bh-nQ0/export?gid=815678089&format=csv"; // �۹� ��Ʈ
    const string facilityCSVPath = "https://docs.google.com/spreadsheets/d/1Sd5x4Mt1fnIVY-X97tvYx1lTE8bh-nQ0/export?gid=1924872652&format=csv"; // �ü� ��Ʈ
    const string correspondentCSVPath = "https://docs.google.com/spreadsheets/d/1Sd5x4Mt1fnIVY-X97tvYx1lTE8bh-nQ0/export?gid=1446733082&format=csv"; // �ŷ�ó ��Ʈ
    const string corNeedCropCSVPath = "https://docs.google.com/spreadsheets/d/1Sd5x4Mt1fnIVY-X97tvYx1lTE8bh-nQ0/export?gid=1954559638&format=csv"; // �ŷ�ó �䱸�۹� ��Ʈ
    const string corCropTypeCSVPath = "https://docs.google.com/spreadsheets/d/1Sd5x4Mt1fnIVY-X97tvYx1lTE8bh-nQ0/export?gid=1014353564&format=csv"; // �ŷ�ó�� �۹��������� ��Ʈ
    const string corCountCropCSVPath = "https://docs.google.com/spreadsheets/d/1Sd5x4Mt1fnIVY-X97tvYx1lTE8bh-nQ0/export?gid=760081684&format=csv"; // �ŷ�ó�� �䱸�۹����� ��Ʈ
    const string eventCSVPath = "https://docs.google.com/spreadsheets/d/1Sd5x4Mt1fnIVY-X97tvYx1lTE8bh-nQ0/export?gid=1411103725&format=csv"; // ���� �̺�Ʈ ��Ʈ
    const string eventSeasonCSVPath = "https://docs.google.com/spreadsheets/d/1Sd5x4Mt1fnIVY-X97tvYx1lTE8bh-nQ0/export?gid=446557917&format=csv"; // �̺�Ʈ ���� ��Ʈ
    const string stageCSVPath = "https://docs.google.com/spreadsheets/d/1Sd5x4Mt1fnIVY-X97tvYx1lTE8bh-nQ0/export?gid=1766045491&format=csv"; // �������� ��Ʈ
    const string stageCorCSVPath = "https://docs.google.com/spreadsheets/d/1Sd5x4Mt1fnIVY-X97tvYx1lTE8bh-nQ0/export?gid=169149848&format=csv"; // ���������� �ŷ�ó ��Ʈ

    public List<CROP> Crops;
    public bool downloadCheck;
    public static CSVManager Instance;    

    private void Start()
    {
        downloadCheck = false;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(gameObject);
        }

        StartCoroutine(DownloadRoutine());
    }

    IEnumerator DownloadRoutine()
    {
        UnityWebRequest request = UnityWebRequest.Get(cropCSVPath); // ��ũ�� ���ؼ� ������Ʈ�� �ٿ�ε� ��û
        yield return request.SendWebRequest();                  // ��ũ�� �����ϰ� �Ϸ�� ������ ���

        string receiveText = request.downloadHandler.text;      // �ٿ�ε� �Ϸ��� ������ �ؽ�Ʈ�� �б�

        Debug.Log(receiveText);

        string[] lines = receiveText.Split('\n');
        for (int y = 2; y < lines.Length; y++)
        {
            CROP crop = new CROP();

            string[] values = lines[y].Split(',', '\t');

            crop.cropID = int.Parse(values[0]);
            crop.cropName = values[1];
            crop.maxShovelCount = int.Parse(values[2]);
            crop.maxWaterCount = int.Parse(values[3]);
            crop.maxNutrientCount = int.Parse(values[4]);
            crop.maxGrowthTime = float.Parse(values[5]);
            crop.droughtMaxWaterCount = int.Parse(values[6]);
            crop.droughtMaxNutrientCount = int.Parse(values[7]);
            crop.damagePercent = float.Parse(values[8]);
            crop.damageTime = float.Parse(values[9]);
            crop.lowTemperTime = float.Parse(values[10]);
            crop.highTemperTime = float.Parse(values[11]);

            Crops.Add(crop);
        }
    }
}