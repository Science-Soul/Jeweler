using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SlideHandler : MonoBehaviour
{
    public Button nextButton;
    public Button backButton;
    public Button continueButton;


    public GameObject blackScreen;

    public List<GameObject> slides = new List<GameObject>();

    private int currentSlideIndex = 0;
    private Tween buttonsTween;
    //private AsyncOperation sceneLoading;

    //[SerializeField] string sceneName;
    [SerializeField] GameObject buttons;

    // Start is called before the first frame update
    void Start()
    {
        /*if (!string.IsNullOrEmpty(sceneName))
        {
            sceneLoading = SceneManager.LoadSceneAsync(sceneName);
            sceneLoading.allowSceneActivation = false;
        }*/


        if (nextButton != null)
        {
            nextButton.onClick.AddListener(HandleNextButtonClick);
            nextButton.gameObject.SetActive(true);
        }

        if (backButton != null)
        {
            backButton.onClick.AddListener(HandleBackButtonClick);
            backButton.gameObject.SetActive(false);
        }

        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(false);
        }
        ShowCurrentSlide();
    }

    private void ShowCurrentSlide()
    {
        if (buttons != null)
        {
            buttons.GetComponent<CanvasGroup>().interactable = false;
        }
        foreach (var slide in slides)
        {
            slide.SetActive(false);
        }

        slides[currentSlideIndex].SetActive(true);
        slides[currentSlideIndex].GetComponent<Slide>().FadeSlideChildren();

        if (currentSlideIndex == slides.Count - 1)
        {
            nextButton.gameObject.SetActive(false);
            continueButton.gameObject.SetActive(true);
        }
        else
        {
            nextButton.gameObject.SetActive(true);
            continueButton.gameObject.SetActive(false);
        }

        if (currentSlideIndex == 0) backButton.gameObject.SetActive(false);
        else backButton.gameObject.SetActive(true);

        if (buttons != null)
        {
            buttonsTween?.Kill();
            buttons.GetComponent<CanvasGroup>().alpha = 0;
            buttonsTween = buttons.GetComponent<CanvasGroup>().DOFade(1, 1f).SetDelay(1.5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                buttons.GetComponent<CanvasGroup>().interactable = true;
            });
        }
    }

    public void HandleNextButtonClick()
    {
        if (currentSlideIndex < slides.Count - 1)
        {
            currentSlideIndex++;
            ShowCurrentSlide();
        }
    }

    public void HandleBackButtonClick()
    {
        if (currentSlideIndex > 0)
        {
            currentSlideIndex--;
            ShowCurrentSlide();
        }
    }

    private void OnDestroy()
    {
        if (nextButton != null)
        {
            nextButton.onClick.RemoveListener(HandleNextButtonClick);
        }

        if (backButton != null)
        {
            backButton.onClick.RemoveListener(HandleBackButtonClick);
        }
    }

    public void ShowBlackScreen()
    {
        blackScreen.SetActive(true);
        CanvasGroup canvasGroup = blackScreen.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        canvasGroup.DOFade(0, 3f).OnComplete(() => blackScreen.SetActive(false));
    }
}
