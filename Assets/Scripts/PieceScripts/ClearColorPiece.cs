using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ColorPiece;

public class ClearColorPiece : ClearablePiece
{
    private ColorPiece colorComponent;
    private ColorType color;

    [SerializeField] GameObject RainbowVFX;

    public ColorType Color
    {
        get { return color; }
        set
        {
            color = value;
            colorComponent.SetColor(value);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        colorComponent = GetComponent<ColorPiece>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Clear()
    {
        RainbowVFX.SetActive(true);
        RainbowVFX.transform.parent = null;
        base.Clear();

        piece.GridRef.ClearColor(color);
    }
}
