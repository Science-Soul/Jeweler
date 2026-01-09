using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using System;

public class Level : MonoBehaviour
{
    public GameObject gameOver;
    public const int MAX_TOTAL_COLLECTED_PIECES = Instrument.BASE_CHARGE * 5;
    [HideInInspector] public int currentTotalPiecesCollected;
    public enum LevelType
    {
        PieceColor,
        ROW,
        COLUMN,
        RAINBOW,
        ANVIL1,
        ANVIL2,
    }

    public Grid grid;
    public HUD hud;

    public LevelType type;

    public LevelType Type
    {
        get
        {
            return type;
        }
    }

    protected int currentScoreOLD;
    public Dictionary<object, int> currentScoresByTypeDict;

    protected bool didWin;

    protected Dictionary<ColorPiece.ColorType, int> remainingScoresDict;
    protected Dictionary<ColorPiece.ColorType, int> remainingScoresDict0;
    [System.Serializable]
    public struct RemainingScores
    {
        public ColorPiece.ColorType color;
        public int remainingScore;
    };
    [SerializeField] protected RemainingScores[] remainingScores;
    public GameObject[] instruments;

    public event Action<ColorPiece.ColorType, int> ScoreUpdate;
    protected void OnScoreUpdate(ColorPiece.ColorType color, int score)
    {
        ScoreUpdate?.Invoke(color, score);
    }

    protected void Awake()
    {
        foreach (GameObject go in instruments)
        {
            go.SetActive(false);
        }

        remainingScoresDict = new();
        foreach (RemainingScores s in remainingScores)
        {
            if (!remainingScoresDict.ContainsKey(s.color))
            {
                remainingScoresDict.Add(s.color, s.remainingScore);
            }
        }

        remainingScoresDict0 = new(remainingScoresDict);
    }

    // Start is called before the first frame update
    public virtual void Start()
    {
        gameOver.SetActive(false);
        currentTotalPiecesCollected = 0;
    }

    protected virtual void OnScoreUpdate<T>(T type, int score)
    {

    }

    public virtual void OnPieceCleared(GamePiece piece)
    {
        currentScoreOLD += piece.score;
        if (currentTotalPiecesCollected < MAX_TOTAL_COLLECTED_PIECES)
        {
            currentTotalPiecesCollected++;

            foreach (GameObject go in instruments)
            {
                if (currentTotalPiecesCollected == go.GetComponent<Instrument>().INSTRUMENT_CHARGE)
                {
                    go.SetActive(true);
                }
            }
        }

        hud.SetInstrumentBar();
    }

    public virtual void OnAnvilScoreAdd(AnvilPiece anvil)
    {

    }

    public virtual void OnAnvilScoreSubtract(AnvilPiece anvil)
    {

    }

    public int GetRemainingForColorType(ColorPiece.ColorType colorType)
    {
        if (remainingScoresDict.ContainsKey(colorType))
        {
            return remainingScoresDict[colorType];
        }
        return 0;
    }
}
