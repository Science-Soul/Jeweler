using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using static ColorPiece;

public class AnvilPiece : MonoBehaviour
{
    protected bool isActivated = false;
    protected bool isLocked = false;
    protected SpriteRenderer spriteRend;
    protected int x;
    protected int y;
    protected Color activeAnvilColor;
    protected Color lockedAnvilColor = Color.white;

    [SerializeField] protected Sprite anvilSprite;
    [SerializeField] protected Sprite anvilActivatedSprite;
    [SerializeField] protected GameObject anvilOversaturatedFrame;
    [SerializeField] protected GameObject anvilVFX;

    public bool isSelected = false;
    public bool isOversaturated = false;

    public int X
    {
        get { return x; }
    }

    public int Y
    {
        get { return y; }
    }

    protected Grid grid;

    public Grid GridRef
    {
        get { return grid; }
    }

    [System.Serializable]
    public struct AnvilSprite
    {
        public Color baseColor;
        public ColorType color;
    };

    public AnvilSprite[] anvilSprites;

    protected ColorType anvilColor;
    public ColorType AnvilColor
    {
        get { return anvilColor; }
        private set { }
    }

    public int NumColors
    {
        get { return anvilSprites.Length; }
    }

    protected Dictionary<ColorType, Color> anvilSpriteDict;

    protected virtual void Awake()
    {
        anvilSpriteDict = new Dictionary<ColorType, Color>();

        for (int i = 0; i < anvilSprites.Length; i++) //Заполняем словарь из массива
        {
            if (!anvilSpriteDict.ContainsKey(anvilSprites[i].color))
            {
                anvilSpriteDict.Add(anvilSprites[i].color, anvilSprites[i].baseColor);
            }
        }

        spriteRend = gameObject.GetComponent<SpriteRenderer>();
    }

    protected virtual void Start()
    {
        spriteRend.sprite = anvilSprite;
        Color.RGBToHSV(anvilSpriteDict[anvilColor], out float h, out float s, out float v);
        activeAnvilColor = Color.HSVToRGB(h, s + 1f, v + 1f, true);
    }



    public void Init(int _x, int _y, Grid _grid, ColorType _anvilColor) //Инициализация плитки
    {
        this.x = _x;
        this.y = _y;
        this.grid = _grid;
        this.anvilColor = _anvilColor;
        this.spriteRend.color = anvilSpriteDict[anvilColor];
    }

    public virtual void Activate()
    {
        if (!isActivated)
        {
            gameObject.tag = "AnvilActivated";
            spriteRend.sprite = anvilActivatedSprite;
            anvilVFX.SetActive(true);
            spriteRend.DOColor(activeAnvilColor, 2f).SetEase(Ease.InOutBack);
            isActivated = true;
            if (GridRef.level.Type == Level.LevelType.ANVIL1)
            {
                GridRef.level.OnAnvilScoreAdd(gameObject.GetComponent<AnvilPiece>());
            }
        }
    }

    public virtual void Oversaturate()
    {
        if (isActivated && !isOversaturated)
        {
            gameObject.tag = "Oversaturated";
            anvilOversaturatedFrame.SetActive(true);
            isOversaturated = true;
            if (GridRef.level.Type == Level.LevelType.ANVIL2)
            {
                GridRef.level.OnAnvilScoreAdd(gameObject.GetComponent<AnvilPiece>());
            }
        }
    }

    public virtual void Lock()
    {
        if (!isActivated)
        {
            gameObject.tag = "AnvilLocked";
            spriteRend.DOColor(lockedAnvilColor, 1f).SetEase(Ease.OutQuad);
            isActivated = true;
            isLocked = true;
        }
    }

    public virtual void Unlock()
    {
        if (isLocked)
        {
            gameObject.tag = "AnvilInitial";
            spriteRend.DOColor(anvilSpriteDict[anvilColor], 1f).SetEase(Ease.OutQuad);
            isLocked = false;
            isActivated = false;
        }
    }
}
