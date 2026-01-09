using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LevelHints : MonoBehaviour
{
    [SerializeField] GameObject tipsCanvas;
    [SerializeField] GameOver gameOverMenu;
    [SerializeField] Level level;

    // Start is called before the first frame update
    void Start()
    {
        // Выдать подсказки
        tipsCanvas.SetActive(true);
        gameOverMenu.SetGameLayersInvisible();
        CanvasGroup canvasGroup = tipsCanvas.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, 1.8f);
    }

    public void PauseGame()
    {
        gameOverMenu.SetGameLayersInvisible();
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        gameOverMenu.SetGameLayersVisible();
        Time.timeScale = 1;
    }
}
