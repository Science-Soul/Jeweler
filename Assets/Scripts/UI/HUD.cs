using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Rendering;

public class HUD : MonoBehaviour
{
    public Level level;
    public Slider instrumentBar;

    // Start is called before the first frame update
    void Start()
    {

        instrumentBar.maxValue = Level.MAX_TOTAL_COLLECTED_PIECES;
        instrumentBar.value = 0;
    }

    public void SetInstrumentBar()
    {
        instrumentBar.DOValue(level.currentTotalPiecesCollected, 1f);
    }
}
