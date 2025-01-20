using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;

public class LoadingController : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("�ε� ȭ�� UI")]
    public GameObject loadingUI;          
    [Tooltip("���α׷��� ��")]
    public Slider progressBar;
    [Tooltip("���൵")]
    public TMP_Text progressText;

    [Tooltip("Tip �ؽ�Ʈ")]
    public TMP_Text tipText;

    [Header("Tip �ؽ�Ʈ ���")]
    [Tooltip("Tip ���")]
    public List<string> tipTexts;
    private int currentTipIndex = 0;

    private bool _isSceneReady = false;
    private bool isButtonPressed = false;
    private XRNode _leftControllerNode = XRNode.LeftHand;
    private void Start()
    {
        loadingUI.SetActive(true);

        if (tipTexts == null || tipTexts.Count == 0)
        {
            Debug.LogWarning("Tip �ؽ�Ʈ ����� ��� �ֽ��ϴ�.");
            tipTexts = new List<string> { "Tip�� �����ϴ�." };
        }

        currentTipIndex = Random.Range(0, tipTexts.Count);

        UpdateTipText();

        string targetSceneName = SceneLoader.TargetScene;

        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogError("TargetScene�� �������� �ʾҽ��ϴ�.");
            return;
        }
        StartCoroutine(LoadSceneAsync(targetSceneName));
    }

    private void Update()
    {
        HandleControllerInput();
    }

    private void HandleControllerInput()
    {
        InputDevice leftController = InputDevices.GetDeviceAtXRNode(_leftControllerNode);
        if (leftController.TryGetFeatureValue(CommonUsages.primaryButton, out bool isPressed))
        {
            if (isPressed && !isButtonPressed)
            {
                isButtonPressed = true;
                ShowNextTip();
            }
        }
        else
        {
            isButtonPressed = false;
        }
    }

    private void ShowNextTip()
    {
        currentTipIndex = (currentTipIndex + 1) % tipTexts.Count;
        UpdateTipText();
    }

    private void UpdateTipText()
    {
        if (tipText != null)
        {
            tipText.text = tipTexts[currentTipIndex];
        }
    }

    private IEnumerator LoadSceneAsync(string targetSceneName)
    {
        // FusionLobby ���� �񵿱�� �ε�
        var asyncLoad = SceneManager.LoadSceneAsync(targetSceneName);

        // �� �ڵ� Ȱ��ȭ ��Ȱ��ȭ
        asyncLoad.allowSceneActivation = false;

        float simulatedProgress = 0f;
        float smoothSpeed = 0.2f;

        while (!asyncLoad.isDone)
        {
            float realProgress = asyncLoad.progress / 0.9f;

            simulatedProgress = Mathf.MoveTowards(simulatedProgress, realProgress, smoothSpeed * Time.deltaTime);

            progressBar.value = simulatedProgress;
            progressText.text = $"{(simulatedProgress * 100f):0}%";

            // �� �ε尡 �Ϸ�Ǿ��� ��
            if (simulatedProgress >= 1f && !_isSceneReady)
            {
                Debug.Log("�� �ε� �Ϸ�. �� Ȱ��ȭ �غ�.");
                _isSceneReady = true;

                // �ణ�� ���� �ð� �� �� Ȱ��ȭ
                yield return new WaitForSeconds(0.5f);
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
