using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YG;

public class SceneFinisher : MonoBehaviour
{
    public GameObject sceneEnding;
    public GameObject progressPanel;
    public Slider progressBar;
    public TextMeshProUGUI progressText;


    public void LoadNextScene()
    {
        StartCoroutine(LoadAsyncScene());
        YG2.InterstitialAdvShow();
    }

    IEnumerator LoadAsyncScene()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (sceneIndex == SceneManager.sceneCountInBuildSettings - 1)
        {
            SceneManager.LoadScene(0);
            Debug.Log("Это была последняя сцена!");
            yield break;
        }

        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        int FirstExtraLevelSceneNumber = SceneUtility.GetBuildIndexByScenePath("Assets/Scenes/ExtraLevels/_ORDERED/1.1_StartExtraLevels.unity");
        if (sceneIndex >= YG2.saves.highestUnlockedLevelIndex && sceneIndex < FirstExtraLevelSceneNumber - 1)
        {
            YG2.saves.highestUnlockedLevelIndex = sceneIndex + 1;
            YG2.SaveProgress();
            YG2.SetLeaderboard("Leaderboard", YG2.saves.highestUnlockedLevelIndex);
        }
        if (sceneIndex >= YG2.saves.highestUnlockedExtraLevelIndexDelta + FirstExtraLevelSceneNumber - 1)
        {
            YG2.saves.highestUnlockedExtraLevelIndexDelta = sceneIndex + 2 - FirstExtraLevelSceneNumber;
            YG2.SaveProgress();
        }
        operation.allowSceneActivation = false;
        bool isFadeStarted = false;
        sceneEnding.SetActive(true);
        CanvasGroup endingCanvasGroup = sceneEnding.GetComponent<CanvasGroup>();
        endingCanvasGroup.alpha = 0;
        progressPanel.SetActive(true);
        CanvasGroup progressBarCanvasGroup = progressPanel.GetComponent<CanvasGroup>();
        progressBarCanvasGroup.alpha = 0;
        while (!operation.isDone)
        {
            // Прогресс загрузки от 0 до 0.9 (до фактической активации сцены)
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            if (progressBar != null)
            {
                progressBar.value = progress;
            }

            if (progressText != null)
            {
                progressText.text = $"Загрузка: {progress * 100f:F0}%";
            }

            if (operation.progress >= 0.9f && !isFadeStarted)
            {
                isFadeStarted = true;
                progressBarCanvasGroup.DOFade(1, 2f);
                endingCanvasGroup.DOFade(1, 2f).OnComplete(() => operation.allowSceneActivation = true);
            }
            // Ждем следующего кадра
            yield return null;
        }
    }
}
