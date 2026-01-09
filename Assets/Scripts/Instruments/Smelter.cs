using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Smelter : Instrument
{
    public const int SMELTER_CHARGE = BASE_CHARGE * 1;

    public override int INSTRUMENT_CHARGE
    {
        get { return SMELTER_CHARGE; }
        set { }
    }

    // Update is called once per frame
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
                    currentAnvilTarget.Unlock();
                    currentAnvilTarget = null;

                    UseInstrumentCharge();
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    GameObject anvilPiece = CheckTargetTagTap(Camera.main.ScreenToWorldPoint(Input.mousePosition), "Anvil","AnvilLocked");
                    if (anvilPiece != null)
                    {
                        if (anvilPiece.TryGetComponent<AnvilPiece>(out var currentAnvil))
                        {
                            currentAnvil.Unlock();
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
        col.offset = new Vector2(0.4f, 0.38f);
        col.radius = 0.0001f;
    }
}
