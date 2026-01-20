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
        base.Clear();
        piece.GridRef.ClearColor(color);

        ActivateVFX();
    }

    void ActivateVFX()
    {
        RainbowVFX.SetActive(true);

        float offsetX = gameObject.GetComponent<BoxCollider2D>().size.x / 2f;
        float offsetY = -gameObject.GetComponent<BoxCollider2D>().size.y / 2f;
        Vector3 offset = new Vector3((float)offsetX, (float)offsetY, 0); // Чтобы эффект появился не в углу, а в центре плитки
        Vector3 newPiecePosition = (Vector3)piece.GridRef.GetWorldPosition(piece.X, piece.Y);
        RainbowVFX.transform.position = newPiecePosition;
        RainbowVFX.transform.parent = null;
    }
}
