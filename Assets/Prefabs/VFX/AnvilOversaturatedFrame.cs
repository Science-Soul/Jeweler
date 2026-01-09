using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AnvilOversaturatedFrame : MonoBehaviour
{
    SpriteRenderer rend;

    // Start is called before the first frame update
    void Start()
    {
        rend = gameObject.GetComponent<SpriteRenderer>();
        Color.RGBToHSV(rend.color, out float h, out float s, out float v);
        rend.DOColor(Color.HSVToRGB(h, s + 5f, v + 5f, true), 1f).SetEase(Ease.OutQuad);
    }
}
