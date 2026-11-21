using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : Instrument
{
    public const int HAMMER_CHARGE = BASE_CHARGE * 1;

    public override int INSTRUMENT_CHARGE
    {
        get { return HAMMER_CHARGE; }
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
                    RemovePiece();
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
                        RemovePiece();
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

    public void RemovePiece()
    {
        if (currentTarget != null)
        {
            DeselectTarget();
            int x = currentTarget.X;
            int y = currentTarget.Y;
            Grid grid = currentTarget.GridRef;
            grid.ClearPiece(x, y);
            grid.ClearAllValidMatches();

            currentTarget = null;
            StartCoroutine(grid.Fill(UseInstrumentCharge));
            level.grid.InvokeOnGridFilled();
        }
    }

    override protected void ShrinkCollider()
    {
        col.offset = new Vector2(0.4f, 0.38f);
        col.radius = 0.0001f;
    }
}
