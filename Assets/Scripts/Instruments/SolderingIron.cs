using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolderingIron : Instrument
{
    public const int SOLDERING_CHARGE = BASE_CHARGE * 4;

    public override int INSTRUMENT_CHARGE
    {
        get { return SOLDERING_CHARGE; }
        set { }
    }

    protected override void Update()
    {
        base.Update();

        if (isGrabbed)
        {
            if (!Application.isMobilePlatform && !isMobile)
            {
                if (currentAnvilTarget != null && currentAnvilTarget.isSelected && Input.GetMouseButtonDown(0))
                {
                    DeselectTarget();
                    currentAnvilTarget.Oversaturate();
                    currentAnvilTarget = null;

                    UseInstrumentCharge();
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    GameObject anvilPiece = CheckTargetTagTap(Camera.main.ScreenToWorldPoint(Input.mousePosition), "Anvil", "AnvilActivated");
                    if (anvilPiece != null)
                    {
                        if (anvilPiece.TryGetComponent<AnvilPiece>(out var currentAnvil))
                        {
                            currentAnvil.Oversaturate();
                            UseInstrumentCharge();
                            AllMobileInstrumentReset();
                        }
                    }
                }
            }
        }
    }

    override protected void ShrinkCollider()
    {
        col.offset = new Vector2(-0.13f, -0.31f);
        col.radius = 0.0001f;
    }
}
