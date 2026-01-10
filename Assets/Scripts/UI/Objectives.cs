using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Objectives : MonoBehaviour
{
    private Level level;

    [SerializeField] GameObject objectivePrefab;

    [Serializable]
    public struct PieceSprites
    {
        public Sprite sprite;
        public ColorPiece.ColorType color;
    }
    public PieceSprites[] normalPieceStructs;
    public Dictionary<ColorPiece.ColorType, Sprite> pieceSpritesDict;

    [Serializable]
    public struct ColumnPieceSprites
    {
        public Sprite sprite;
        public ColorPiece.ColorType color;
    }
    public PieceSprites[] columnPieceStructs;
    public Dictionary<ColorPiece.ColorType, Sprite> columnSpritesDict;

    [Serializable]
    public struct RowPieceSprites
    {
        public Sprite sprite;
        public ColorPiece.ColorType color;
    }
    public PieceSprites[] rowPieceStructs;
    public Dictionary<ColorPiece.ColorType, Sprite> rowSpritesDict;

    [Serializable]
    public struct AnvilColors
    {
        public Color color;
        public ColorPiece.ColorType colorType;
    }
    public AnvilColors[] anvil1ColorStructs;
    public Dictionary<ColorPiece.ColorType, Color> colorDict;

    public Sprite anvilSprite;
    public Sprite rainbowSprite;

    private void Awake()
    {
        level = FindAnyObjectByType<Level>();

        switch (level.Type)
        {
            case Level.LevelType.PieceColor:
                SetPieceColorObjectives(pieceSpritesDict, normalPieceStructs);
                break;

            case Level.LevelType.COLUMN:
                SetPieceColorObjectives(columnSpritesDict, columnPieceStructs);
                break;

            case Level.LevelType.ROW:
                SetPieceColorObjectives(rowSpritesDict, rowPieceStructs);
                break;

            case Level.LevelType.ANVIL1:
                SetAnvil1Objectives(colorDict, anvil1ColorStructs);
                break;

            default: return;
        }

    }
    private void Start()
    {

    }

    private void SetPieceColorObjectives(Dictionary<ColorPiece.ColorType, Sprite> spriteDict, PieceSprites[] spriteStructs)
    {
        spriteDict = new Dictionary<ColorPiece.ColorType, Sprite>();
        foreach (var spriteStruct in spriteStructs)
        {
            if (!spriteDict.ContainsKey(spriteStruct.color))
            {
                spriteDict.Add(spriteStruct.color, spriteStruct.sprite);
            }
        }

        foreach (Level.RemainingScores objective in level.remainingScores)
        {
            UIScoreByColorTypeUpdater obj = Instantiate(objectivePrefab, this.gameObject.transform).GetComponent<UIScoreByColorTypeUpdater>();
            if (obj != null && spriteDict.ContainsKey(objective.color))
            {
                obj.Init(objective.color, spriteDict[objective.color], Color.white);
            }
        }
    }

    private void SetRainbowObjectives()
    {

    }

    private void SetAnvil1Objectives(Dictionary<ColorPiece.ColorType, Color> colorDict, AnvilColors[] colorStructs)
    {
        colorDict = new Dictionary<ColorPiece.ColorType, Color>();
        foreach (var colorStruct in colorStructs)
        {
            if (!colorDict.ContainsKey(colorStruct.colorType))
            {
                colorDict.Add(colorStruct.colorType, colorStruct.color);
            }
        }

        foreach (Level.RemainingScores objective in level.remainingScores)
        {
            UIScoreByColorTypeUpdater obj = Instantiate(objectivePrefab, this.gameObject.transform).GetComponent<UIScoreByColorTypeUpdater>();
            if (obj != null && colorDict.ContainsKey(objective.color))
            {
                obj.Init(objective.color, anvilSprite, colorDict[objective.color]);
            }
        }
    }

    private void SetAnvil2Objectives()
    {

    }



}
