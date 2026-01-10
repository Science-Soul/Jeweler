using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using static ColorPiece;

public class AnvilFlashPiece : AnvilPiece
{
    private Color mainSpriteColor;
    protected override void Awake()
    {
        base.Awake();

        mainSpriteColor = spriteRend.color;
    }

    protected override void Start()
    {
        base.Start();

        Grid.OnGridFilled += Activate;
        Grid.OnGridFilled += Deactivate;
    }

    public override void Activate()
    {
        if (!isActivated && grid != null && grid.GetPiece(x, y) != null && grid.GetPiece(x, y).ColorComponent.Color == anvilColor)
        {
            gameObject.tag = "AnvilActivated";
            spriteRend.sprite = anvilActivatedSprite;
            spriteRend.DOColor(activeAnvilColor, 0.5f).SetEase(Ease.InOutQuad);
            isActivated = true;
            if (GridRef.level.Type == Level.LevelType.FLASH)
            {
                GridRef.level.OnAnvilScoreAdd(gameObject.GetComponent<AnvilPiece>());
            }
        }
    }

    public void Deactivate()
    {
        if (isActivated && grid != null && grid.GetPiece(x, y) != null && grid.GetPiece(x, y).ColorComponent.Color != anvilColor)
        {
            gameObject.tag = "AnvilInitial";
            spriteRend.sprite = anvilSprite;
            spriteRend.DOColor(anvilSpriteDict[anvilColor], 0.5f).SetEase(Ease.InOutQuad);
            isActivated = false;
            if (GridRef.level.Type == Level.LevelType.FLASH)
            {
                GridRef.level.OnAnvilScoreSubtract(gameObject.GetComponent<AnvilPiece>());
            }
        }
    }

    public override void Oversaturate()
    {
        
    }

    public override void Lock()
    {
        
    }

    public override void Unlock()
    {
        
    }

    private void OnDisable()
    {
        Grid.OnGridFilled -= Activate;
        Grid.OnGridFilled -= Deactivate;
    }
}
