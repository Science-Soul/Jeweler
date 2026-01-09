using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIScoreByColorTypeUpdater : MonoBehaviour
{
    public ColorPiece.ColorType targetColorType;
    private TextMeshProUGUI scoreText;
    private int initialRequiredAmount;
    public Level level;
    private void Start()
    {
        scoreText = GetComponent<TextMeshProUGUI>();
        initialRequiredAmount = level.GetRemainingForColorType(targetColorType);
        level.ScoreUpdate += HandleScoreUpdate;
        UpdateUIText(initialRequiredAmount);
    }
    
    private void HandleScoreUpdate(ColorPiece.ColorType colorToUpdate, int newTotalScore) 
    {
        if (colorToUpdate == targetColorType)
        {
            UpdateUIText(newTotalScore);
        }
    }

    private void UpdateUIText(int remaining)
    {
        scoreText.text = remaining.ToString();
    }

    private void OnDestroy()
    {
        if (scoreText != null)
        {
            level.ScoreUpdate -= HandleScoreUpdate;
        }
    }
}
