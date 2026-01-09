using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainbowInlayer : Instrument
{
    public const int RAINBOW_CHARGE = BASE_CHARGE * 5;

    public override int INSTRUMENT_CHARGE
    {
        get { return RAINBOW_CHARGE; }
        set { }
    }

    private GamePiece currentTarget;
    protected override void Update()
    {
        base.Update();

        if (isGrabbed)
        {
            if (!Application.isMobilePlatform && !isMobile)
            {
                if (currentTarget != null && currentTarget.isSelected && Input.GetMouseButtonDown(0))
                {
                    ChangePiece();
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    GameObject crystal = CheckTargetTagTap(Camera.main.ScreenToWorldPoint(Input.mousePosition), "Pieces", "Crystal");
                    if (crystal != null)
                    {
                        currentTarget = crystal.GetComponent<GamePiece>();
                        ChangePiece();
                        AllMobileInstrumentReset();
                    }
                }
            }
        }
    }

    protected override void SelectTarget(Collider2D col)
    {
        currentTarget = col.gameObject.GetComponent<GamePiece>();
        currentTarget.isSelected = true;
        spriteOutline.transform.position = col.gameObject.GetComponent<BoxCollider2D>().bounds.center;
        spriteOutline.gameObject.SetActive(true);
    }

    protected override void DeselectTarget()
    {
        if (currentTarget != null)
        {
            currentTarget.isSelected = false;
        }

        spriteOutline.gameObject.SetActive(false);
    }

    public void ChangePiece()
    {
        if (currentTarget != null)
        {
            DeselectTarget();
            int x = currentTarget.X;
            int y = currentTarget.Y;
            GamePiece newPiece = currentTarget.GridRef.SpawnNewPiece(x, y, Grid.PieceType.RAINBOW);
            newPiece.ColorComponent.Color = ColorPiece.ColorType.ANY;

            currentTarget.GridRef.ClearAllValidMatches();
            Destroy(currentTarget.gameObject);
            currentTarget = null;
            UseInstrumentCharge();
            level.grid.InvokeOnGridFilled();
        }
    }

    override protected void ShrinkCollider()
    {
        col.offset = new Vector2(0.36f, 0.4f);
        col.radius = 0.0001f;
    }
}
