using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingController : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("�ε� ȭ�� UI")]
    public GameObject loadingUI;          
    [Tooltip("���α׷��� ��")]
    public Slider progressBar;
    [Tooltip("���൵")]
    public TMP_Text progressText;

    private bool _isSceneReady = false;    // �� �ε� �Ϸ� ���� Ȯ��

    private void Start()
    {
        loadingUI.SetActive(true);
        StartCoroutine(LoadFusionLobbyAsync());
    }

    private IEnumerator LoadFusionLobbyAsync()
    {
        // FusionLobby ���� �񵿱�� �ε�
        var asyncLoad = SceneManager.LoadSceneAsync("03_FusionLobby");

        // �� �ڵ� Ȱ��ȭ ��Ȱ��ȭ
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            // �� �ε尡 �Ϸ�Ǿ��� ��
            if (asyncLoad.progress >= 0.9f && !_isSceneReady)
            {
                Debug.Log("�� �ε� �Ϸ�. ���α׷��� �� �غ�.");
                _isSceneReady = true;

                // 3�� ���� ���α׷��� �ٸ� �ڿ������� ����
                yield return StartCoroutine(SimulateProgressBar(3f));

                // �� Ȱ��ȭ
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    private IEnumerator SimulateProgressBar(float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / duration);

            // ���α׷��� �� �� �ؽ�Ʈ ������Ʈ
            progressBar.value = progress;
            progressText.text = $"{(progress * 100f):0}%";

            yield return null;
        }

        // ���� �Ϸ� ���� ����
        progressBar.value = 1f;
        progressText.text = "100%";
    }
}
