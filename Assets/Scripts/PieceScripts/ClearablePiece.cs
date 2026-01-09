using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Grid;

public class ClearablePiece : MonoBehaviour //Скрипт для удаления плиток
{
    private Grid _gridRef;

    public AnimationClip clearAnimation;

    public ParticlePrefab[] particlePrefabs;
    private Dictionary<ColorPiece.ColorType, ParticleSystem> particlePrefabDict; //Словарь для частиц

    [System.Serializable]
    public struct ParticlePrefab //Структура для передачи данных из инспектора в словарь частиц
    {
        public ColorPiece.ColorType color;
        public ParticleSystem particle;
    };



    private bool isBeingCleared = false;

    public bool IsBeingCleared
    {
        get { return isBeingCleared; }
    }

    protected GamePiece piece;

    private void Awake()
    {
        piece = GetComponent<GamePiece>();

        particlePrefabDict = new Dictionary<ColorPiece.ColorType, ParticleSystem>(); //Загрузили префабы частиц в словарь
        for (int i = 0; i < particlePrefabs.Length; i++)
        {
            if (!particlePrefabDict.ContainsKey(particlePrefabs[i].color))
            {
                particlePrefabDict.Add(particlePrefabs[i].color, particlePrefabs[i].particle);
            }
        }
    }

    public virtual void Clear()
    {
        piece.GridRef.level.OnPieceCleared(piece);
        piece.GridRef.PlayClearSound();
        isBeingCleared = true;
        StartCoroutine(ClearCoroutine());
    }

    private IEnumerator ClearCoroutine()
    {
        Animator animator = GetComponent<Animator>();

        if (animator)
        {
            animator.Play(clearAnimation.name);
            yield return new WaitForSeconds(piece.GridRef.fillTIme);


            if (PSPooler.Instance != null)
            {
                Vector2 center = new Vector2(this.gameObject.GetComponent<BoxCollider2D>().size.x / 2, -this.gameObject.GetComponent<BoxCollider2D>().size.y);
                ParticleSystem currentPS = PSPooler.Instance.SpawnParticle((Vector2)transform.position + center).GetComponent<ParticleSystem>();
                currentPS.GetComponent<PSReturnToPool>().SetPSColorFromPiece(piece);
            }


            Destroy(gameObject);
        }
    }
}

