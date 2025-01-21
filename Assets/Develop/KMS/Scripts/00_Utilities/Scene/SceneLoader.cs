using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    [Tooltip("�̵��� �� �̸��� �����ϴ� ���� ����")]
    public static string TargetScene { get; set; }

    /// <summary>
    /// �ε������� ��ȯ�� �̵��� �� �̸��� ����
    /// </summary>
    /// <param name="targetSceneName">�̵��� �� �̸�</param>
    public static void LoadSceneWithLoading(string targetSceneName)
    {
        TargetScene = targetSceneName;
        SceneManager.LoadScene("LoadingScene");
    }

    /// <summary>
    /// ��Ʈ��ũ �� ��ȯ: PhotonNetwork�� ���� ��� Ŭ���̾�Ʈ �̵�
    /// </summary>
    public static void LoadNetworkSceneWithLoading(string targetSceneName)
    {
        PhotonNetwork.LoadLevel(targetSceneName);
    }
}
