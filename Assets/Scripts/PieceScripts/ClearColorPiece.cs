using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ColorPiece;

public class ClearColorPiece : ClearablePiece
{
    private ColorPiece colorComponent;
    private ColorType color;

    public ColorType Color
    {
        get { return color; }
        set
        {
            color = value;
            colorComponent.SetColor(value);
        }
    }
    /*private ColorPiece.ColorType color;

    public ColorPiece.ColorType Color
    {
        get { return color; }
        set { ColorPiece.SetColor(value); }
    }*/

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
        base.Clear();

        piece.GridRef.ClearColor(color);
    }
}
