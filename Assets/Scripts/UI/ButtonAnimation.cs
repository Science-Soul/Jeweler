using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Tween Settings")]
    [SerializeField] private float hoverScale = 1.05f; // Легкое масштабирование при наведении
    [SerializeField] private float scaleDuration = 0.2f; // Длительность анимации масштабирования

    [Header("Hover (Up-Down) Animation")]
    [SerializeField] private float moveAmountY = 10f; // На сколько пикселей двигаться вверх/вниз
    [SerializeField] private float moveDuration = 1.0f; // Длительность одного цикла движения (вверх или вниз)
    [SerializeField] private Ease moveEase = Ease.InOutSine; // Тип сглаживания для плавного движения

    private Vector3 originalScale;
    private Vector3 originalPosition;
    private RectTransform rectTransform;
    private Tween scaleTween; // Для управления анимацией масштабирования
    private Tween moveTween;  // Для управления анимацией движения вверх-вниз

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("ButtonHoverEffect: RectTransform not found on this GameObject.", this);
            enabled = false;
            return;
        }
        originalScale = rectTransform.localScale;
        originalPosition = rectTransform.anchoredPosition; // Используем anchoredPosition для UI
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Останавливаем предыдущие анимации, если они активны
        scaleTween?.Kill();
        moveTween?.Kill();

        // Анимация масштабирования
        scaleTween = rectTransform.DOScale(originalScale * hoverScale, scaleDuration)
            .SetEase(Ease.OutBack);

        // Анимация покачивания вверх-вниз
        // Двигаем вверх, потом возвращаем, потом вниз, потом возвращаем.
        // Или просто вверх-вниз-вверх, используя SetLoops с Yoyo
        moveTween = rectTransform.DOAnchorPosY(originalPosition.y + moveAmountY, moveDuration / 2) // Двигаемся вверх
            .SetEase(moveEase)
            .SetLoops(-1, LoopType.Yoyo) // Бесконечное покачивание вверх-вниз
            .SetRelative(false); // Убеждаемся, что позиция абсолютная, не относительная к текущей
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Останавливаем все текущие анимации
        scaleTween?.Kill();
        moveTween?.Kill();

        // Возвращаем к оригинальному размеру
        scaleTween = rectTransform.DOScale(originalScale, scaleDuration)
            .SetEase(Ease.OutSine);

        // Возвращаем к оригинальной позиции
        moveTween = rectTransform.DOAnchorPosY(originalPosition.y, scaleDuration)
            .SetEase(Ease.OutSine);
    }

    void OnDisable()
    {
        // Гарантируем остановку анимаций при отключении объекта/скрипта
        scaleTween?.Kill();
        moveTween?.Kill();
    }
}