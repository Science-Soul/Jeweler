using System.Linq;
using UnityEngine;

public class LevelColorPiece : Level
{
    public override void OnPieceCleared(GamePiece piece)
    {
        base.OnPieceCleared(piece);

        if (remainingScoresDict != null)
        {
            void UpdateScore()
            {
                int currentRemaining = --remainingScoresDict[piece.ColorComponent.Color];
                OnScoreUpdate(piece.ColorComponent.Color, currentRemaining);
            }

            bool isInDict = remainingScoresDict.ContainsKey(piece.ColorComponent.Color);
            bool isRow = piece.Type == Grid.PieceType.ROW_CLEAR;
            bool isColumn = piece.Type == Grid.PieceType.COLUMN_CLEAR;
            bool isRainbow = piece.Type == Grid.PieceType.RAINBOW;
            if (type == LevelType.RAINBOW)
            {
                if (remainingScoresDict.ContainsKey(ColorPiece.ColorType.ANY) && remainingScoresDict[ColorPiece.ColorType.ANY] > 0 && isRainbow)
                {
                    int currentRemaining = --remainingScoresDict[ColorPiece.ColorType.ANY];
                    OnScoreUpdate(ColorPiece.ColorType.ANY, currentRemaining);
                }
            }
            else if (type == LevelType.ROW)
            {
                if (isInDict && remainingScoresDict[piece.ColorComponent.Color] > 0 && isRow)
                {
                    UpdateScore();
                }
            }
            else if (type == LevelType.COLUMN)
            {
                if (isInDict && remainingScoresDict[piece.ColorComponent.Color] > 0 && isColumn)
                {
                    UpdateScore();
                }
            }
            else if (type == LevelType.PieceColor)
            {
                if (isInDict && remainingScoresDict[piece.ColorComponent.Color] > 0)
                {
                    UpdateScore();
                }
            }
        }

        if (remainingScoresDict.All(x => x.Value == 0))
        {
            gameOver.SetActive(true);
            gameOver.GetComponent<GameOver>().SetGameLayersInvisible();
            grid.gameOver = true;
        }
    }
}
