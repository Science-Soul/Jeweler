using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovablePiece : MonoBehaviour //Cкрипт для перемещения плиток
{
    private GamePiece piece; //Получаем доступ к плитке
    private IEnumerator moveCoroutine;
    private IEnumerator moveBackCoroutine;
    private Animator animator;
    //public AnimationCurve animationCurve;

    private void Awake()
    {
        piece = GetComponent<GamePiece>();
        animator = GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Move(int newX, int newY, float time) //Для перемещения плиток
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        moveCoroutine = MoveCoroutine(newX, newY, time);
        StartCoroutine(moveCoroutine);
    }

    private IEnumerator MoveCoroutine(int newX, int newY, float time) //Анимация перемещения
    {
        if (!piece.GridRef.isBusy) piece.GridRef.isBusy = true;
        piece.X = newX; 
        piece.Y = newY;

        Vector3 startPos = transform.position;
        Vector3 endPos = piece.GridRef.GetWorldPosition(newX,newY);

        /*piece.transform.position = endPos;
        yield return 0;*/
        for (float t = 0; t <= 1 * time; t += Time.deltaTime) //Способ СУПЕР! Интерполяция через цикл. Это же гениально.
        {
            //float evaluateValue = animationCurve.Evaluate(t / time);
            piece.transform.position = Vector3.Lerp(startPos, endPos, t / time);
            yield return 0; //Ждем 1 кадр
        }

        piece.transform.position = new Vector2(Mathf.Round(endPos.x * 2) / 2, Mathf.Round(endPos.y * 2) / 2); //После интерполяции координата немного отличается от целого, поэтому нужно округлить.

        piece.GridRef.isBusy = false;
    }

    public void MoveBack(int newX, int newY, float time)
    {
        if (moveBackCoroutine != null)
        {
            StopCoroutine(moveBackCoroutine);
        }

        moveBackCoroutine = MoveBackCoroutine(newX, newY, time);
        StartCoroutine(moveBackCoroutine);
    }

    private IEnumerator MoveBackCoroutine(int newX, int newY, float time)
    {
        if (!piece.GridRef.isBusy) piece.GridRef.isBusy = true;

        int pieceOldX = piece.X;
        int pieceOldY = piece.Y;

        piece.X = newX;
        piece.Y = newY;

        Vector3 startPos = transform.position;
        Vector3 endPos = piece.GridRef.GetWorldPosition(newX, newY);

        for (float t = 0; t <= 1 * time; t += Time.deltaTime) //Способ СУПЕР! Интерполяция через цикл. Это же гениально.
        {
            piece.transform.position = Vector3.Lerp(startPos, endPos, t / time);
            yield return 0; //Ждем 1 кадр
        }
        piece.transform.position = new Vector2(Mathf.Round(endPos.x * 2) / 2, Mathf.Round(endPos.y * 2) / 2); //После интерполяции координата немного отличается от целого, поэтому нужно округлить.

        yield return new WaitForSeconds(time);

        for (float t = 0; t <= 1 * time; t += Time.deltaTime) //Способ СУПЕР! Интерполяция через цикл. Это же гениально.
        {
            piece.transform.position = Vector3.Lerp(endPos, startPos, t / time);
            yield return 0; //Ждем 1 кадр
        }

        piece.X = pieceOldX;
        piece.Y = pieceOldY;
        piece.transform.position = new Vector2(Mathf.Round(startPos.x * 2) / 2, Mathf.Round(startPos.y * 2) / 2); //После интерполяции координата немного отличается от целого, поэтому нужно округлить.

        piece.GridRef.isBusy = false;
    }

    private void OnMouseEnter()
    {
        animator.SetBool("OnMouseEnter", true);
    }

    private void OnMouseExit()
    {
        animator.SetBool("OnMouseEnter", false);
    }
}
