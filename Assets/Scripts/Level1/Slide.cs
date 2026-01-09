using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Slide : MonoBehaviour
{
    Sequence sequence;

    private void OnEnable()
    {
        sequence = DOTween.Sequence();
    }
    public void FadeSlideChildren()
    {
        CanvasGroup[] canvasGroup = transform.GetComponentsInChildren<CanvasGroup>();
        foreach (var c in canvasGroup)
        {
            c.alpha = 0;
            c.blocksRaycasts = false;
            sequence.Append(c.DOFade(1, 1f).SetEase(Ease.Linear));
        }
    }

    private void OnDisable()
    {
        sequence?.Kill();
    }


}
