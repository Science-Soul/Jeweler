using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIScoreByColorTypeUpdater : MonoBehaviour
{
    public ColorPiece.ColorType targetColorType;
    private TextMeshProUGUI scoreText;
    private int initialRequiredAmount;
    private Level level;

    [SerializeField] Image image;

    private void Start()
    {
        
    }
    public void Init(ColorPiece.ColorType colorType, Sprite sprite, Color col)
    {
        image.sprite = sprite;
        image.color = col;
        targetColorType = colorType;
        level = FindAnyObjectByType<Level>();
        initialRequiredAmount = level.GetRemainingForColorType(targetColorType);
        scoreText = GetComponentInChildren<TextMeshProUGUI>();
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
