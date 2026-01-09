using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class SceneStarter : MonoBehaviour
{
    [SerializeField] GameObject[] objectsToActivate;
    [SerializeField] GameObject blackSquare;
    [SerializeField] GameObject pauseMenu;
    // Start is called before the first frame update
    void Start()
    {
        if (DebugManager.instance != null)
        {
            DebugManager.instance.enableRuntimeUI = false;
        }
        foreach (GameObject obj in objectsToActivate)
        {
            obj.SetActive(false);
        }
        blackSquare.SetActive(true);
        CanvasGroup canvasGroup = blackSquare.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        canvasGroup.DOFade(0, 1.5f).OnComplete(() =>
        {
            blackSquare.SetActive(false);
            StartScene();
        });
        if (SceneManager.GetActiveScene().buildIndex == 0 || SceneManager.GetActiveScene().buildIndex == 1)
        {
            pauseMenu.SetActive(false);
        }
    }

    public void StartScene()
    {
        foreach (GameObject go in objectsToActivate)
        {
            go.SetActive(true);
        }
    }
}
