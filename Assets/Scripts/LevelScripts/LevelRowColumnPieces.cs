using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelRowColumnPieces : Level
{
    public override void OnPieceCleared(GamePiece piece)
    {
        base.OnPieceCleared(piece);

        if (remainingScoresDict != null)
        {
            if (remainingScoresDict.ContainsKey(piece.ColorComponent.Color) && remainingScoresDict[piece.ColorComponent.Color] > 0 && (piece.Type == Grid.PieceType.ROW_CLEAR || piece.Type == Grid.PieceType.COLUMN_CLEAR))
            {
                int currentRemaining = --remainingScoresDict[piece.ColorComponent.Color];
                OnScoreUpdate(piece.ColorComponent.Color, currentRemaining);
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
