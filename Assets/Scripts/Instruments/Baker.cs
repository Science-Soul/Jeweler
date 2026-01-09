using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Baker : Instrument
{
    public const int BAKER_CHARGE = BASE_CHARGE * 2;

    public override int INSTRUMENT_CHARGE
    {
        get { return BAKER_CHARGE; }
        set { }
    }

    override protected void Update()
    {
        base.Update();

        if (isGrabbed)
        {
            if (!Application.isMobilePlatform && !isMobile)
            {
                if (currentAnvilTarget != null && currentAnvilTarget.isSelected && Input.GetMouseButtonDown(0))
                {
                    DeselectTarget();
                    currentAnvilTarget.Activate();
                    currentAnvilTarget = null;

                    UseInstrumentCharge();
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    GameObject anvilPiece = CheckTargetTagTap(Camera.main.ScreenToWorldPoint(Input.mousePosition), "Anvil", "AnvilInitial");
                    if (anvilPiece != null)
                    {
                        if (anvilPiece.TryGetComponent<AnvilPiece>(out var currentAnvil))
                        {
                            currentAnvil.Activate();
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
        col.offset = new Vector2(-0.25f, 0.3f);
        col.radius = 0.0001f;
    }
}
