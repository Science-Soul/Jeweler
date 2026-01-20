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
    [SerializeField] Sprite anvil1FrameSprite;

    private void Start()
    {
        
    }
    public void Init(ColorPiece.ColorType colorType, Sprite sprite, Color col)
    {
        image.sprite = sprite;
        image.color = col;
        targetColorType = colorType;
        level = FindAnyObjectByType<Level>();
        scoreText = GetComponentInChildren<TextMeshProUGUI>();
        level.ScoreUpdate += HandleScoreUpdate;
        initialRequiredAmount = level.GetRemainingForColorType(targetColorType);
        UpdateUIText(initialRequiredAmount);
    }

    public void AddAnvilFrame()
    {
        CreateEmptyChildWithImage(gameObject.transform);

        void CreateEmptyChildWithImage(Transform parent)
        {
            GameObject empty = new GameObject("anvil1Image");
            empty.transform.SetParent(parent, false);
            Image newImageComponent = empty.AddComponent<Image>();
            newImageComponent.sprite = anvil1FrameSprite;
            RectTransform rectTransform = empty.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.up;
            rectTransform.anchorMax = Vector2.up;
            rectTransform.localPosition = new Vector2(-88, 0);
            rectTransform.sizeDelta = new Vector2(130, 130);
        }
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
