using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InlayerPanel : MonoBehaviour
{
    GameObject inlayer;
    private void Start()
    {
        inlayer = transform.parent.Find("Inlayer").gameObject;
    }

    void SetInstrumentsLayersVisible()
    {
        Camera.main.eventMask |= LayerMask.GetMask("Pieces");
        Camera.main.eventMask |= LayerMask.GetMask("Smelter");
        Camera.main.eventMask |= LayerMask.GetMask("Baker");
        Camera.main.eventMask |= LayerMask.GetMask("Inlayer");
        Camera.main.eventMask |= LayerMask.GetMask("Soldering Iron");
        Camera.main.eventMask |= LayerMask.GetMask("RainbowInlayer");
    }

    public void CloseMenu()
    {
        SetInstrumentsLayersVisible();
        gameObject.SetActive(false);
    }

    public void ChangePiece(string color)
    {
        inlayer.GetComponent<Inlayer>().ChangePiece(color);
    }
}
