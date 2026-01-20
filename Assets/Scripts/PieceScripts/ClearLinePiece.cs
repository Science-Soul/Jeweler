using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearLinePiece : ClearablePiece
{
    public bool isRow;

    public ParticleSystem RowVFX;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Clear()
    {
        base.Clear();

        if (isRow)
        {
            piece.GridRef.ClearRow(piece.Y);
        }
        else
        {
            piece.GridRef.ClearColumn(piece.X);
        }

        InstantiateVFX();
    }

    private void InstantiateVFX()
    {
        float offsetX = gameObject.GetComponent<BoxCollider2D>().size.x / 2f;
        float offsetY = -gameObject.GetComponent<BoxCollider2D>().size.y / 2f;
        Vector3 offset = new Vector3((float)offsetX, (float)offsetY, 0); // Чтобы эффект появился не в углу, а в центре плитки
        Vector3 newPiecePosition = (Vector3)piece.GridRef.GetWorldPosition(piece.X, piece.Y);
        Instantiate(RowVFX, newPiecePosition + offset, RowVFX.transform.rotation);
    }
}
