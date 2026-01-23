using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GameOver : MonoBehaviour
{

    private void Start()
    {
        CanvasGroup canvasGroup = gameObject.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, 1.5f).SetDelay(1.5f).SetUpdate(true);

    }
    public void SetGameLayersInvisible()
    {
        Camera.main.eventMask &= ~LayerMask.GetMask("Pieces");
        Camera.main.eventMask &= ~LayerMask.GetMask("Smelter");
        Camera.main.eventMask &= ~LayerMask.GetMask("Baker");
        Camera.main.eventMask &= ~LayerMask.GetMask("Inlayer");
        Camera.main.eventMask &= ~LayerMask.GetMask("Soldering Iron");
        Camera.main.eventMask &= ~LayerMask.GetMask("RainbowInlayer");
    }

    public void SetGameLayersVisible()
    {
        Camera.main.eventMask |= LayerMask.GetMask("Pieces");
        Camera.main.eventMask |= LayerMask.GetMask("Smelter");
        Camera.main.eventMask |= LayerMask.GetMask("Baker");
        Camera.main.eventMask |= LayerMask.GetMask("Inlayer");
        Camera.main.eventMask |= LayerMask.GetMask("Soldering Iron");
        Camera.main.eventMask |= LayerMask.GetMask("RainbowInlayer");
    }
}
