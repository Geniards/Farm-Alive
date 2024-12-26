using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Taping : MonoBehaviour
{
    [SerializeField] BoxTrigger currentBox;
    [SerializeField] bool isTaping = false;
    [SerializeField] float tapeProgress = 0f;
    [SerializeField] float requiredTapeProgress = 1f;
    [SerializeField] Vector3 firstPosition;
    [SerializeField] Vector3 secendPosition;
    
    [SerializeField] bool isCanSealed;
    [SerializeField] bool isStart = false;
    [SerializeField] bool isEnd = false;


    private void Update()
    {
        if (isTaping && currentBox != null)
        {
            isCanSealed = true;
        }
        else
        {
            isCanSealed = false;
        }
    }

    public void StartTaping(BoxTrigger box)
    {
        if (box == null || !box.IsCoverClosed()) return;

        currentBox = box;
        isTaping = true;
        tapeProgress = 0f;
        Debug.Log($"������ ����: {box.name}");
    }

    public void StopTaping()
    {
        isTaping = false;
        currentBox = null;
        Debug.Log("������ �ߴ�");
    }

    public void StartPosition()
    {
        if (isCanSealed)
        {
            firstPosition = this.gameObject.transform.position;
            Debug.Log(firstPosition);
            Debug.Log(gameObject.transform.position);
            Debug.Log(currentBox.boxCover1.transform.position);

            if (Vector3.Distance(firstPosition, currentBox.boxCover1.transform.position) < 0.2f)
            {
                Debug.Log("ù�Ÿ���0.1����");
                isStart = true;
            }
        }
        
    }

    public void EndPosition()
    {
        if (isCanSealed)
        {
            secendPosition = this.gameObject.transform.position;

            if (Vector3.Distance(secendPosition, currentBox.boxCover2.transform.position) < 0.2f)
            {
                Debug.Log("�ѰŸ���0.1����");
                isEnd = true;
            }

            if(isStart && isEnd)
            {
                currentBox.boxTape.SetActive(true);
            }
        }
    }

    private void CompleteTaping()
    {
        isTaping = false;
        if (currentBox != null)
        {
            currentBox.SealBox();
            Debug.Log($"������ �Ϸ�: {currentBox.name}");
        }
        currentBox = null;
    }
}