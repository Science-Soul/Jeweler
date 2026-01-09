using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AegisAnimation : MonoBehaviour
{
    [SerializeField] CanvasGroup cg;
    
    void OnEnable()
    {
        cg.alpha = 1.0f;
        cg.DOFade(0.2f, 2).SetLoops(-1, LoopType.Yoyo);
    }

    private void OnDisable()
    {
        DOTween.Kill(gameObject);
    }
}
