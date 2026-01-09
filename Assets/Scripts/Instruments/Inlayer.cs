using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inlayer : Instrument
{
    public const int INLAYER_CHARGE = BASE_CHARGE * 3;
    public override int INSTRUMENT_CHARGE
    {
        get { return INLAYER_CHARGE; }
        set { }
    }

    private GamePiece currentTarget;

    [SerializeField] GameObject menuPrefab;

    protected override void Update()
    {
        base.Update();

        if (isGrabbed)
        {
            if (!Application.isMobilePlatform && !isMobile)
            {
                if (currentTarget != null && currentTarget.isSelected && Input.GetMouseButtonDown(0))
                {
                    menuPrefab.SetActive(true);
                    col.enabled = false;
                    ResetInstrument();
                    Camera.main.eventMask &= ~LayerMask.GetMask("Pieces");
                    SetInstrumentsLayersInvisible();
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
                        menuPrefab.SetActive(true);
                        AllMobileInstrumentReset();
                        Camera.main.eventMask &= ~LayerMask.GetMask("Pieces");
                        SetInstrumentsLayersInvisible();
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            menuPrefab.SetActive(false);
            SetInstrumentsLayersVisible();
            ResetInstrument();
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

    public void ChangePiece(string color) //вызывается в UI через кнопку
    {
        Dictionary<string, ColorPiece.ColorType> colorDict = new()
        {
            { "RED", ColorPiece.ColorType.RED },
            { "BLUE", ColorPiece.ColorType.BLUE },
            { "YELLOW", ColorPiece.ColorType.YELLOW },
            { "GREEN", ColorPiece.ColorType.GREEN },
            { "PURPLE", ColorPiece.ColorType.PURPLE },
            { "CYAN", ColorPiece.ColorType.CYAN }
        };

        Camera.main.eventMask |= LayerMask.GetMask("Pieces");
        DeselectTarget();
        int x = currentTarget.X;
        int y = currentTarget.Y;
        GamePiece newPiece = currentTarget.GridRef.SpawnNewPiece(x, y, Grid.PieceType.NORMAL);
        newPiece.ColorComponent.Color = colorDict[color];

        currentTarget.GridRef.ClearAllValidMatches();
        StartCoroutine(currentTarget.GridRef.Fill(UseInstrumentCharge));
        Destroy(currentTarget.gameObject);
        currentTarget = null;
        menuPrefab.SetActive(false);
        SetInstrumentsLayersVisible();
        level.grid.InvokeOnGridFilled();
    }

    public void CloseMenu()
    {
        menuPrefab.SetActive(false);
        SetInstrumentsLayersVisible();
    }

    override protected void ShrinkCollider()
    {
        col.offset = new Vector2(0.36f, 0.4f);
        col.radius = 0.0001f;
    }

    void SetInstrumentsLayersInvisible()
    {
        Camera.main.eventMask &= ~LayerMask.GetMask("Smelter");
        Camera.main.eventMask &= ~LayerMask.GetMask("Baker");
        Camera.main.eventMask &= ~LayerMask.GetMask("Inlayer");
        Camera.main.eventMask &= ~LayerMask.GetMask("Soldering Iron");
        Camera.main.eventMask &= ~LayerMask.GetMask("RainbowInlayer");
    }

    void SetInstrumentsLayersVisible()
    {
        Camera.main.eventMask |= LayerMask.GetMask("Smelter");
        Camera.main.eventMask |= LayerMask.GetMask("Baker");
        Camera.main.eventMask |= LayerMask.GetMask("Inlayer");
        Camera.main.eventMask |= LayerMask.GetMask("Soldering Iron");
        Camera.main.eventMask |= LayerMask.GetMask("RainbowInlayer");
    }
}
