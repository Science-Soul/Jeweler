using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objectives : MonoBehaviour
{
    private Level level;

    [SerializeField] GameObject objectivePrefab;

    [Serializable]
    public struct NormalPieceSprites
    {
        public Sprite sprite;
        public ColorPiece.ColorType color;
    }
    public NormalPieceSprites[] normalPieceStructs;
    public Dictionary<ColorPiece.ColorType, Sprite> pieceSpritesDict;

    private void Awake()
    {
        level = FindAnyObjectByType<Level>();

        switch (level.Type)
        {
            case Level.LevelType.PieceColor:
                SetPieceColorObjectives();
                break;

            default: return;
        }

    }
    private void Start()
    {

    }

    private void SetPieceColorObjectives()
    {
        pieceSpritesDict = new Dictionary<ColorPiece.ColorType, Sprite>();
        foreach (var normalPieceStruct in normalPieceStructs)
        {
            if (!pieceSpritesDict.ContainsKey(normalPieceStruct.color))
            {
                pieceSpritesDict.Add(normalPieceStruct.color, normalPieceStruct.sprite);
            }
        }

        foreach (Level.RemainingScores objective in level.remainingScores)
        {
            UIScoreByColorTypeUpdater obj = Instantiate(objectivePrefab, this.gameObject.transform).GetComponent<UIScoreByColorTypeUpdater>();
            if (obj != null && pieceSpritesDict.ContainsKey(objective.color))
            {
                obj.Init(objective.color, pieceSpritesDict[objective.color], Color.white);
            }
        }
    }

    private void SetRowObjectives()
    {

    }

    private void SetColumnObjectives()
    {

    }

    private void SetRainbowObjectives()
    {

    }

    private void SetAnvil1Objectives()
    {

    }

    private void SetAnvil2Objectives()
    {

    }



}
