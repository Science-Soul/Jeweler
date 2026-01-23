using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using static ColorPiece;

public class Grid : MonoBehaviour
{
    public GameObject outlineCrystal;

    public enum PieceType
    {
        EMPTY,
        NORMAL,
        BUBBLE,
        ROW_CLEAR,
        COLUMN_CLEAR,
        RAINBOW,
        COUNT
    };

    [System.Serializable]
    public struct PiecePrefab //Структура для передачи данных из инспектора в словарь плиток
    {
        public PieceType type;
        public GameObject prefab;
    };

    [System.Serializable]
    public struct PiecePosition
    {
        public ColorType color;
        public int x;
        public int y;
    };

    [Header("Размер игрового поля")]
    public int xDim;
    public int yDim;

    public Level level;

    public float fillTIme; //Время заполнения игрового поля

    public PiecePrefab[] piecePrefabs;
    public GameObject backgroundPrefab;
    public AnvilPiece[,] anvilPieces;

    [System.Serializable]
    public struct AnvilPosition
    {
        public ColorType anvilColor;
        public int x;
        public int y;
    }
    public AnvilPosition[] anvilPositions;
    public GameObject anvilPrefab;

    public PiecePosition[] initialPieces;

    private Dictionary<PieceType, GameObject> piecePrefabDict; //Словарь для плиток

    private GamePiece[,] pieces; //Игровые плитки

    public GamePiece GetPiece(int x, int y)
    {
        if (pieces[x, y] != null)
            return pieces[x, y];
        else return null;
    } 

    private bool inverse = false; //изменить направление движения плитки

    private GamePiece pressedPiece; //Нажатая и выделенная плитки
    private GamePiece enteredPiece;

    public bool gameOver;

    private bool isFilling = false;
    public bool isBusy = false;

    public AudioSource gridAudioSource;
    public AudioClip[] clearPieceSFX;

    public bool IsFilling
    {
        get { return isFilling; }
    }

    private void OnDisable()
    {
        transform.DOKill();
    }
    void Awake()
    {
        DOTween.Init();
        gridAudioSource = GetComponent<AudioSource>();
        anvilPieces = new AnvilPiece[xDim, yDim];
        piecePrefabDict = new Dictionary<PieceType, GameObject>(); //Загрузили префабы плиток в словарь

        for (int i = 0; i < piecePrefabs.Length; i++)
        {
            if (!piecePrefabDict.ContainsKey(piecePrefabs[i].type))
            {
                piecePrefabDict.Add(piecePrefabs[i].type, piecePrefabs[i].prefab);
            }
        }

        for (int x = 0; x < xDim; x++) //Создали поле для плиток из фона
        {
            for (int y = 0; y < yDim; y++)
            {
                GameObject background = Instantiate(backgroundPrefab, GetWorldPosition(x, y), Quaternion.identity, this.transform);
                if (((x + y) % 2 == 0)) // раскрашиваем в шахматном порядке в 2 цвета
                {
                    background.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 1);
                }
                //anvilPieces[x, y] = background.GetComponent<AnvilPiece>();
            }
        }

        for (int i = 0; i < anvilPositions.Length; i++) //Заменили указанные в инспекторе плитки фона на плитки наковальни
        {
            int x = anvilPositions[i].x;
            int y = anvilPositions[i].y;
            GameObject newAnvilPiece = Instantiate(anvilPrefab, GetWorldPosition(x, y), Quaternion.identity, this.transform);
            anvilPieces[x, y] = newAnvilPiece.GetComponent<AnvilPiece>();
            anvilPieces[x, y].Init(x, y, this, anvilPositions[i].anvilColor);
        }

        pieces = new GamePiece[xDim, yDim]; //Создали массив плиток и расположили на поле

        for (int i = 0; i < initialPieces.Length; i++) // сначала особые плитки
        {
            if (initialPieces[i].x >= 0 && initialPieces[i].x < xDim
                && initialPieces[i].y >= 0 && initialPieces[i].y < yDim)
            {
                SpawnNewPiece(initialPieces[i].x, initialPieces[i].y, PieceType.NORMAL).ColorComponent.Color = initialPieces[i].color;
                
            }
        }

        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                if (pieces[x, y] == null)
                {
                    SpawnNewPiece(x, y, PieceType.EMPTY); //Заполняем поле пустыми клетками
                }
            }
        }

        StartCoroutine(Fill());
    }

    public static event Action OnGridFilled;
    public void InvokeOnGridFilled()
    {
        OnGridFilled?.Invoke();
    }
    public IEnumerator Fill()
    {
        bool needsRefill = true;
        isFilling = true;
        while (needsRefill)
        {
            //yield return fillTIme;
            yield return new WaitForSeconds(fillTIme);
            while (FillStep())
            {
                inverse = !inverse; //Меняем направление после каждого цикла заполнения
                yield return new WaitForSeconds(fillTIme);
            }

            needsRefill = ClearAllValidMatches();
        }

        isFilling = false;
        OnGridFilled?.Invoke();

        // проверка на возможные комбинации
        if (IsMatchesAvailable())
        {
            //Debug.Log("Matches available");
        }
        else
        {
            Sequence shakePiecesSequence = DOTween.Sequence();
            outlineCrystal.SetActive(false);
            isBusy = true;

            for (int x = 0; x < xDim; x++)
            {
                for (int y = 0; y < yDim; y++)
                {
                    int currentX = x;
                    int currentY = y;

                    Tweener currentScaleTween = pieces[currentX, currentY].transform.DOShakeScale(0.1f, 0.2f, 1);
                    Tweener currentPositionTween = pieces[currentX, currentY].transform.DOShakePosition(1, 0.1f, 10).OnComplete(() =>
                    {
                        if (pieces[currentX, currentY] != null)
                        {
                            Destroy(pieces[currentX, currentY].gameObject);
                            SpawnNewPiece(currentX, currentY, PieceType.EMPTY);
                        }
                    });
                    shakePiecesSequence.Join(currentScaleTween);
                    shakePiecesSequence.Join(currentPositionTween);
                }
            }

            shakePiecesSequence.OnComplete(() =>
            {
                if (gameObject.activeInHierarchy)
                {
                    StartCoroutine(Fill());
                }
                isBusy = false;
            });

        }
    }

    public IEnumerator PiecesReplace(GamePiece piece, int x, int y)
    {
        Destroy(piece);
        yield return new WaitForSeconds(3);

        //SpawnNewPiece(x, y, PieceType.EMPTY);
    }

    public IEnumerator Fill(Action onComplete)
    {
        StartCoroutine(Fill());
        yield return 0;
        //yield return new WaitForSeconds(fillTIme);
        onComplete?.Invoke();
    }
    //Делает один проход по всему полю. А надо прогонять, пока все пустоты не уйдут.
    bool FillStep() //Проверяет, есть ли в ряду на уровень ниже пустые клетки, перемещает плитку в пустое место, возвращает тру
    {
        bool movedPiece = false; //Для всех рядов, начиная с предпоследнего (второй снизу), опускаем плитки в пустые ячейки
        for (int y = yDim - 2; y >= 0; y--)
        {
            for (int loopX = 0; loopX < xDim; loopX++)
            {
                int x = loopX;

                if (inverse)
                {
                    x = xDim - 1 - loopX; //При инверсии идем от конца строки к началу
                }
                GamePiece piece = pieces[x, y];

                if (piece.IsMovable())
                {
                    GamePiece pieceBelow = pieces[x, y + 1];

                    if (pieceBelow.Type == PieceType.EMPTY)
                    {
                        Destroy(pieceBelow.gameObject);
                        piece.MovableComponent.Move(x, y + 1, fillTIme);
                        pieces[x, y + 1] = piece;
                        SpawnNewPiece(x, y, PieceType.EMPTY);
                        movedPiece = true;
                    }
                    else //Если код для движения вниз не сработает, идем по диагонали
                    {
                        for (int diag = -1; diag <= 1; diag++)
                        {
                            if (diag != 0)
                            {
                                int diagX = x + diag;

                                if (inverse)
                                {
                                    diagX = x - diag;
                                }

                                if (diagX >= 0 && diagX < xDim)
                                {
                                    GamePiece diagonalPiece = pieces[diagX, y + 1];

                                    if (diagonalPiece.Type == PieceType.EMPTY)
                                    {
                                        bool hasPieceAbove = true;

                                        for (int aboveY = y; aboveY >= 0; aboveY--) //Все элементы над пустым пространством
                                        {
                                            GamePiece pieceAbove = pieces[diagX, aboveY];

                                            if (pieceAbove.IsMovable())
                                            {
                                                break;
                                            }
                                            else if (!pieceAbove.IsMovable() && pieceAbove.Type != PieceType.EMPTY)
                                            {
                                                hasPieceAbove = false;
                                                break;
                                            }
                                        }

                                        if (!hasPieceAbove)
                                        {
                                            Destroy(diagonalPiece.gameObject);
                                            piece.MovableComponent.Move(diagX, y + 1, fillTIme);
                                            pieces[diagX, y + 1] = piece;
                                            SpawnNewPiece(x, y, PieceType.EMPTY);
                                            movedPiece = true;
                                            break;
                                        }
                                    }
                                }
                            }

                        }
                    }
                }
            }
        }

        for (int x = 0; x < xDim; x++) //Верхний ряд заполняется отдельно, потому что над ним ничего нет. Спавним новые плитки.
        {
            GamePiece pieceBelow = pieces[x, 0];

            if (pieceBelow.Type == PieceType.EMPTY)
            {
                Destroy(pieceBelow.gameObject);
                GameObject newPiece = Instantiate(piecePrefabDict[PieceType.NORMAL], GetWorldPosition(x, -1), Quaternion.identity, this.transform);

                pieces[x, 0] = newPiece.GetComponent<GamePiece>();
                pieces[x, 0].Init(x, -1, this, PieceType.NORMAL);
                pieces[x, 0].MovableComponent.Move(x, 0, fillTIme);
                pieces[x, 0].ColorComponent.SetColor((ColorPiece.ColorType)UnityEngine.Random.Range(0, pieces[x, 0].ColorComponent.NumColors));

                ColorPiece.ColorType previousColorHorizontal = ColorPiece.ColorType.ANY;
                ColorPiece.ColorType previousColorVertical = ColorPiece.ColorType.ANY;
                if (x > 1 && pieces[x, 0].ColorComponent.Color == pieces[x - 1, 0].ColorComponent.Color && pieces[x, 0].ColorComponent.Color == pieces[x - 2, 0].ColorComponent.Color)
                {
                    previousColorHorizontal = pieces[x - 2, 0].ColorComponent.Color;
                }

                if (pieces[x, 1].ColorComponent != null && pieces[x, 2].ColorComponent != null)
                {
                    if (pieces[x, 0].ColorComponent.Color == pieces[x, 1].ColorComponent.Color && pieces[x, 0].ColorComponent.Color == pieces[x, 2].ColorComponent.Color)
                    {
                        previousColorVertical = pieces[x, 2].ColorComponent.Color;
                    }
                }

                if (previousColorHorizontal != ColorPiece.ColorType.ANY || previousColorVertical != ColorPiece.ColorType.ANY)
                {
                    do
                    {
                        pieces[x, 0].ColorComponent.SetColor((ColorPiece.ColorType)UnityEngine.Random.Range(0, pieces[x, 0].ColorComponent.NumColors));
                    }
                    while (x > 1 && pieces[x, 0].ColorComponent.Color == pieces[x - 1, 0].ColorComponent.Color || pieces[x, 1].ColorComponent != null && pieces[x, 0].ColorComponent.Color == pieces[x, 1].ColorComponent.Color);
                }
                movedPiece = true;
            }
        }

        return movedPiece;
    }

    public Vector2 GetWorldPosition(int x, int y) //Сдвигаем игровое поле по центру
    {
        return new Vector2(transform.position.x - xDim / 2.0f + x,
            transform.position.y + yDim / 2.0f - y);
    }

    public GamePiece SpawnNewPiece(int x, int y, PieceType type) //Создаем новую плитку типа type в позиции х,у
    {
        GameObject newPiece = Instantiate(piecePrefabDict[type], GetWorldPosition(x, y), Quaternion.identity, this.transform);
        pieces[x, y] = newPiece.GetComponent<GamePiece>();
        pieces[x, y].Init(x, y, this, type);

        return pieces[x, y];
    }

    public bool IsAdjacent(GamePiece piece1, GamePiece piece2)
    {
        return (piece1.X == piece2.X && (int)Mathf.Abs(piece1.Y - piece2.Y) == 1)
            || (piece1.Y == piece2.Y && (int)Mathf.Abs(piece1.X - piece2.X) == 1);
    }

    public void SwapPieces(GamePiece piece1, GamePiece piece2)
    {
        if (gameOver)
        {
            return;
        }

        if (piece1.IsMovable() && piece2.IsMovable() && !isBusy)
        {
            outlineCrystal.SetActive(false);
            pieces[piece1.X, piece1.Y] = piece2;
            pieces[piece2.X, piece2.Y] = piece1;


            if (GetMatch(piece1, piece2.X, piece2.Y) != null || GetMatch(piece2, piece1.X, piece1.Y) != null
                || piece1.Type == PieceType.RAINBOW || piece2.Type == PieceType.RAINBOW)
            {
                int piece1X = piece1.X;
                int piece1Y = piece1.Y;

                piece1.MovableComponent.Move(piece2.X, piece2.Y, fillTIme);
                piece2.MovableComponent.Move(piece1X, piece1Y, fillTIme);


                if (piece1.Type == PieceType.RAINBOW && piece1.IsClearable() && piece2.IsColored())
                {
                    ClearColorPiece clearColor = piece1.GetComponent<ClearColorPiece>();

                    if (clearColor)
                    {
                        clearColor.Color = piece2.ColorComponent.Color;
                    }

                    ClearPiece(piece1.X, piece1.Y);
                }

                if (piece2.Type == PieceType.RAINBOW && piece2.IsClearable() && piece1.IsColored())
                {
                    ClearColorPiece clearColor = piece2.GetComponent<ClearColorPiece>();

                    if (clearColor)
                    {
                        clearColor.Color = piece1.ColorComponent.Color;
                    }

                    ClearPiece(piece2.X, piece2.Y);
                }

                ClearAllValidMatches();

                // Это было для того, чтобы специальные фишки взрывались при обмене, даже если
                // комбинацию образует только вторая фишка???
                // Автор странный... А из-за плохого перевода я не понял, зачем это делалось.
                // Мне это не нужно.
                /*if (piece1.Type == PieceType.ROW_CLEAR || piece1.Type == PieceType.COLUMN_CLEAR)
                {
                    ClearPiece(piece1.X, piece1.Y);
                }

                if (piece2.Type == PieceType.ROW_CLEAR || piece2.Type == PieceType.COLUMN_CLEAR)
                {
                    ClearPiece(piece2.X, piece2.Y);
                }*/

                pressedPiece = null;
                enteredPiece = null;

                StartCoroutine(Fill());

                //level.OnMove();
            }
            else
            {
                //start

                int piece1X = piece1.X;
                int piece1Y = piece1.Y;

                int piece2X = piece2.X;
                int piece2Y = piece2.Y;

                piece1.MovableComponent.MoveBack(piece2X, piece2Y, fillTIme);
                piece2.MovableComponent.MoveBack(piece1X, piece1Y, fillTIme);

                pressedPiece = null;
                enteredPiece = null;



                //end

                pieces[piece1X, piece1Y] = piece1;
                pieces[piece2X, piece2Y] = piece2;

            }


        }
    }

    public void PressPiece(GamePiece piece)
    {
        pressedPiece = piece;
    }

    public void EnterPiece(GamePiece piece)
    {
        enteredPiece = piece;
    }

    public void DragPiece()
    {
        if (pressedPiece != null
            && enteredPiece != null
            && IsAdjacent(pressedPiece, enteredPiece)
            && !isFilling)
        {
            SwapPieces(pressedPiece, enteredPiece);
        }
    }

    public List<GamePiece> GetMatch(GamePiece piece, int newX, int newY)
    {
        if (piece.IsColored())
        {
            ColorPiece.ColorType color = piece.ColorComponent.Color;
            List<GamePiece> horizontalPieces = new List<GamePiece>();
            List<GamePiece> verticalPieces = new List<GamePiece>();
            List<GamePiece> matchingPieces = new List<GamePiece>();

            //First check horizontal
            horizontalPieces.Add(piece);

            for (int dir = 0; dir <= 1; dir++)
            {
                for (int xOffset = 1; xOffset < xDim; xOffset++)
                {
                    int x;
                    if (dir == 0) //Влево
                    {
                        x = newX - xOffset;
                    }
                    else //Вправо
                    {
                        x = newX + xOffset;
                    }

                    if (x < 0 || x >= xDim)
                    {
                        break;
                    }

                    if (pieces[x, newY].IsColored() && pieces[x, newY].ColorComponent.Color == color)
                    {//Если плитки совпали, добавляем в список
                        horizontalPieces.Add(pieces[x, newY]);
                    }
                    else //Как только наткнулись на другую плитку, в этом направлении больше не идем, заканчиваем цикл
                    {
                        break;
                    }
                }
            }

            if (horizontalPieces.Count >= 3)
            {
                for (int i = 0; i < horizontalPieces.Count; i++)
                {
                    matchingPieces.Add(horizontalPieces[i]);
                }
            }

            //Поиск вертикальных совпадений для L-T фигур
            if (horizontalPieces.Count >= 3)
            {
                for (int i = 0; i < horizontalPieces.Count; i++)
                {
                    for (int dir = 0; dir <= 1; dir++)
                    {
                        for (int yOffset = 1; yOffset < yDim; yOffset++)
                        {
                            int y;
                            if (dir == 0) //Вверх
                            {
                                y = newY - yOffset;
                            }
                            else //Вниз
                            {
                                y = newY + yOffset;
                            }

                            if (y < 0 || y >= yDim)
                            {
                                break;
                            }

                            if (pieces[horizontalPieces[i].X, y].IsColored() && pieces[horizontalPieces[i].X, y].ColorComponent.Color == color)
                            {
                                verticalPieces.Add(pieces[horizontalPieces[i].X, y]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    if (verticalPieces.Count < 2)
                    {
                        verticalPieces.Clear();
                    }
                    else
                    {
                        for (int j = 0; j < verticalPieces.Count; j++)
                        {
                            matchingPieces.Add(verticalPieces[j]);
                        }

                        break;
                    }
                }
            }

            if (matchingPieces.Count >= 3)
            {
                return matchingPieces;
            }

            //Did not find anything going horizontally first,
            //so now check vertically
            horizontalPieces.Clear();
            verticalPieces.Clear();
            verticalPieces.Add(piece);

            for (int dir = 0; dir <= 1; dir++)
            {
                for (int yOffset = 1; yOffset < yDim; yOffset++)
                {
                    int y;
                    if (dir == 0) //Вверх
                    {
                        y = newY - yOffset;
                    }
                    else //Влево
                    {
                        y = newY + yOffset;
                    }

                    if (y < 0 || y >= yDim)
                    {
                        break;
                    }

                    if (pieces[newX, y].IsColored() && pieces[newX, y].ColorComponent.Color == color)
                    {
                        verticalPieces.Add(pieces[newX, y]);
                    }
                    else
                    {
                        break;
                    }

                }
            }

            if (verticalPieces.Count >= 3)
            {
                for (int i = 0; i < verticalPieces.Count; i++)
                {
                    matchingPieces.Add(verticalPieces[i]);
                }
            }

            //Поиск горизонтальных совпадений для L-T фигур
            if (verticalPieces.Count >= 3)
            {
                for (int i = 0; i < verticalPieces.Count; i++)
                {
                    for (int dir = 0; dir <= 1; dir++)
                    {
                        for (int xOffset = 1; xOffset < xDim; xOffset++)
                        {
                            int x;
                            if (dir == 0) //Влево
                            {
                                x = newX - xOffset;
                            }
                            else //Вправо
                            {
                                x = newX + xOffset;
                            }

                            if (x < 0 || x >= xDim)
                            {
                                break;
                            }

                            if (pieces[x, verticalPieces[i].Y].IsColored() && pieces[x, verticalPieces[i].Y].ColorComponent.Color == color)
                            {
                                horizontalPieces.Add(pieces[x, verticalPieces[i].Y]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    if (horizontalPieces.Count < 2)
                    {
                        horizontalPieces.Clear();
                    }
                    else
                    {
                        for (int j = 0; j < horizontalPieces.Count; j++)
                        {
                            matchingPieces.Add(horizontalPieces[j]);
                        }

                        break;
                    }
                }
            }

            if (matchingPieces.Count >= 3)
            {
                return matchingPieces;
            }
        }

        return null;
    }

    public bool ClearAllValidMatches()
    {
        bool needsRefill = false;

        for (int y = 0; y < yDim; y++)
        {
            for (int x = 0; x < xDim; x++)
            {
                if (pieces[x, y].IsClearable())
                {
                    List<GamePiece> match = GetMatch(pieces[x, y], x, y);

                    if (match != null)
                    {
                        PieceType specialPieceType = PieceType.COUNT;
                        GamePiece randomPiece = match[UnityEngine.Random.Range(0, match.Count)];
                        int specialPieceX = randomPiece.X;
                        int specialPieceY = randomPiece.Y;

                        if (match.Count == 4)
                        {
                            if (pressedPiece == null || enteredPiece == null)
                            {
                                specialPieceType = (PieceType)UnityEngine.Random.Range((int)PieceType.ROW_CLEAR, (int)PieceType.COLUMN_CLEAR);
                            }
                            else if (pressedPiece.Y == enteredPiece.Y)
                            {
                                specialPieceType = PieceType.ROW_CLEAR;
                            }
                            else
                            {
                                specialPieceType = PieceType.COLUMN_CLEAR;
                            }
                        }
                        else if (match.Count >= 5)
                        {
                            specialPieceType = PieceType.RAINBOW;
                        }

                        for (int i = 0; i < match.Count; i++)
                        {
                            if (ClearPiece(match[i].X, match[i].Y))
                            {
                                needsRefill = true;

                                if (match[i] == pressedPiece || match[i] == enteredPiece)
                                {
                                    specialPieceX = match[i].X;
                                    specialPieceY = match[i].Y;
                                }
                            }
                        }

                        if (specialPieceType != PieceType.COUNT)
                        {
                            Destroy(pieces[specialPieceX, specialPieceY]);
                            GamePiece newPiece = SpawnNewPiece(specialPieceX, specialPieceY, specialPieceType);

                            if ((specialPieceType == PieceType.ROW_CLEAR || specialPieceType == PieceType.COLUMN_CLEAR)
                                && newPiece.IsColored() && match[0].IsColored())
                            {
                                newPiece.ColorComponent.SetColor(match[0].ColorComponent.Color);

                            }
                            else if (specialPieceType == PieceType.RAINBOW && newPiece.IsColored())
                            {
                                newPiece.ColorComponent.SetColor(ColorPiece.ColorType.ANY);
                            }
                        }
                    }
                }
            }
        }

        return needsRefill;
    }

    public bool ClearPiece(int x, int y)
    {
        if (pieces[x, y].IsClearable() && !pieces[x, y].ClearableComponent.IsBeingCleared)
        {
            // Сначала работаем с наковальней
            if (anvilPieces[x, y] != null && pieces[x, y].ColorComponent.Color == anvilPieces[x, y].AnvilColor)
            {
                anvilPieces[x, y].Activate();
            }

            if (anvilPieces[x, y] != null && pieces[x, y].ColorComponent.Color != anvilPieces[x, y].AnvilColor)
            {
                anvilPieces[x, y].Lock();
            }

            // Теперь удаляем фишку
            pieces[x, y].ClearableComponent.Clear();
            SpawnNewPiece(x, y, PieceType.EMPTY);

            ClearObstacles(x, y);

            return true;
        }

        return false;
    }

    public void ClearObstacles(int x, int y)
    {
        for (int adjacentX = x - 1; adjacentX <= x + 1; adjacentX++)
        {
            if (adjacentX != x && adjacentX >= 0 && adjacentX < xDim)
            {
                if (pieces[adjacentX, y].Type == PieceType.BUBBLE && pieces[adjacentX, y].IsClearable())
                {
                    pieces[adjacentX, y].ClearableComponent.Clear();
                    SpawnNewPiece(adjacentX, y, PieceType.EMPTY);
                }
            }
        }

        for (int adjacentY = y - 1; adjacentY <= y + 1; adjacentY++)
        {
            if (adjacentY != y && adjacentY >= 0 && adjacentY < yDim)
            {
                if (pieces[x, adjacentY].Type == PieceType.BUBBLE && pieces[x, adjacentY].IsClearable())
                {
                    pieces[x, adjacentY].ClearableComponent.Clear();
                    SpawnNewPiece(x, adjacentY, PieceType.EMPTY);
                }
            }
        }
    }

    public void ClearRow(int row)
    {
        for (int x = 0; x < xDim; x++)
        {
            ClearPiece(x, row);
        }
    }

    public void ClearColumn(int column)
    {
        for (int y = 0; y < yDim; y++)
        {
            ClearPiece(column, y);
        }
    }

    public void ClearColor(ColorPiece.ColorType color)
    {
        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                if (pieces[x, y].IsColored() && pieces[x, y].ColorComponent.Color == color
                    || color == ColorPiece.ColorType.ANY)
                {
                    ClearPiece(x, y);
                }
            }
        }
    }

    public void GameOver()
    {
        gameOver = true;

    }

    public List<GamePiece> GetPiecesOfType(PieceType type)
    {
        List<GamePiece> piecesOfType = new List<GamePiece>();

        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                if (pieces[x, y].Type == type)
                {
                    piecesOfType.Add(pieces[x, y]);
                }
            }
        }

        return piecesOfType;
    }

    public bool IsMatchesAvailable()
    {
        
        // Я ПОМЕНЯЛ МЕСТАМИ Х И У!!! 20.11.2025, поэтому в комментах строки и столбцы надо поменять местами
        //
        // 0*0
        // 0*0
        // 000
        // 0*0
        for (int x = 0; x < xDim; x++) // строка x = const
        {
            for (int y = 0; y < yDim - 3; y++)
            {
                if (pieces[x, y].ColorComponent.Color == pieces[x, y + 1].ColorComponent.Color
                 && pieces[x, y].ColorComponent.Color == pieces[x, y + 3].ColorComponent.Color)
                {
                    return true;
                }
            }

            for (int y = yDim - 1; y >= 3; y--)
            {
                if (pieces[x, y].ColorComponent.Color == pieces[x, y - 1].ColorComponent.Color
                 && pieces[x, y].ColorComponent.Color == pieces[x, y - 3].ColorComponent.Color)
                {
                    return true;
                }
            }
        }

        // 0*0 00*
        // 0*0 0*0
        // 00* 0*0
        for (int x = 0; x < xDim - 1; x++)
        {
            for (int y = 0; y < yDim - 2; y++)
            {
                if (pieces[x, y].ColorComponent.Color == pieces[x, y + 1].ColorComponent.Color
                 && pieces[x, y].ColorComponent.Color == pieces[x + 1, y + 2].ColorComponent.Color)
                {
                    return true;
                }
            }

            for (int y = yDim - 1; y >= 2; y--)
            {
                if (pieces[x, y].ColorComponent.Color == pieces[x, y - 1].ColorComponent.Color
                 && pieces[x, y].ColorComponent.Color == pieces[x + 1, y - 2].ColorComponent.Color)
                {
                    return true;
                }
            }
        }

        // *00 0*0
        // 0*0 0*0
        // 0*0 *00
        for (int x = xDim - 1; x >= 1; x--)
        {
            for (int y = 0; y < yDim - 2; y++)
            {
                if (pieces[x, y].ColorComponent.Color == pieces[x, y + 1].ColorComponent.Color
                 && pieces[x, y].ColorComponent.Color == pieces[x - 1, y + 2].ColorComponent.Color)
                {
                    return true;
                }
            }

            for (int y = yDim - 1; y >= 2; y--)
            {
                if (pieces[x, y].ColorComponent.Color == pieces[x, y - 1].ColorComponent.Color
                 && pieces[x, y].ColorComponent.Color == pieces[x - 1, y - 2].ColorComponent.Color)
                {
                    return true;
                }
            }
        }

        // 0*0
        // 00*
        // 0*0
        for (int x = 0; x < xDim - 1; x++)
        {
            for (int y = 0; y < yDim - 2; y++)
            {
                if (pieces[x, y].ColorComponent.Color == pieces[x, y + 2].ColorComponent.Color
                 && pieces[x, y].ColorComponent.Color == pieces[x + 1, y + 1].ColorComponent.Color)
                {
                    return true;
                }
            }
        }

        // 0*0
        // *00
        // 0*0
        for (int x = xDim - 1; x >= 1; x--)
        {
            for (int y = 0; y < yDim - 2; y++)
            {
                if (pieces[x, y].ColorComponent.Color == pieces[x, y + 2].ColorComponent.Color
                 && pieces[x, y].ColorComponent.Color == pieces[x - 1, y + 1].ColorComponent.Color)
                {
                    return true;
                }
            }
        }

        

        // **0*
        // 000
        // 000
        for (int y = 0; y < yDim; y++) // строка у = const
        {
            for (int x = 0; x < xDim - 3; x++)
            {
                if (pieces[x, y].ColorComponent.Color == pieces[x + 1, y].ColorComponent.Color
                 && pieces[x, y].ColorComponent.Color == pieces[x + 3, y].ColorComponent.Color)
                {
                    return true;
                }
            }

            for (int x = xDim - 1; x >= 3; x--)
            {
                if (pieces[x, y].ColorComponent.Color == pieces[x - 1, y].ColorComponent.Color
                 && pieces[x, y].ColorComponent.Color == pieces[x - 3, y].ColorComponent.Color)
                {
                    return true;
                }
            }
        }

        // **0
        // 00*
        // 000
        for (int y = 0; y < yDim - 1; y++)
        {
            for (int x = 0; x < xDim - 2; x++)
            {
                if (pieces[x, y].ColorComponent.Color == pieces[x + 1, y].ColorComponent.Color
                 && pieces[x, y].ColorComponent.Color == pieces[x + 2, y + 1].ColorComponent.Color)
                {
                    return true;
                }
            }

            // 0**
            // *00
            // 00
            for (int x = xDim - 1; x >= 2; x--)
            {
                if (pieces[x, y].ColorComponent.Color == pieces[x - 1, y].ColorComponent.Color
                 && pieces[x, y].ColorComponent.Color == pieces[x - 2, y + 1].ColorComponent.Color)
                {
                    return true;
                }
            }
        }

        
        for (int y = yDim - 1; y >= 1; y--)
        {
            // 000
            // 00*
            // **0
            for (int x = 0; x < xDim - 2; x++)
            {
                if (pieces[x, y].ColorComponent.Color == pieces[x + 1, y].ColorComponent.Color
                 && pieces[x, y].ColorComponent.Color == pieces[x + 2, y - 1].ColorComponent.Color)
                {
                    return true;
                }
            }

            // 000
            // *00
            // 0**
            for (int x = xDim - 1; x >= 2; x--)
            {
                if (pieces[x, y].ColorComponent.Color == pieces[x - 1, y].ColorComponent.Color
                 && pieces[x, y].ColorComponent.Color == pieces[x - 2, y - 1].ColorComponent.Color)
                {
                    return true;
                }
            }
        }

        // 000
        // *0*
        // 0*0
        for (int y = 0; y < yDim - 1; y++)
        {
            for (int x = 0; x < xDim - 2; x++)
            {
                if (pieces[x, y].ColorComponent.Color == pieces[x + 2, y].ColorComponent.Color
                 && pieces[x, y].ColorComponent.Color == pieces[x + 1, y + 1].ColorComponent.Color)
                {
                    return true;
                }
            }
        }
        
        // 000
        // 0*0
        // *0*
        for (int y = yDim - 1; y >= 1; y--)
        {
            for (int x = 0; x < xDim - 2; x++)
            {
                if (pieces[x, y].ColorComponent.Color == pieces[x + 2, y].ColorComponent.Color
                 && pieces[x, y].ColorComponent.Color == pieces[x + 1, y - 1].ColorComponent.Color)
                {
                    return true;
                }
            }
        }

        for (int y = 0; y < yDim; y++)
        {
            for (int x = 0; x < xDim; x++)
            {
                if (pieces[x, y].Type == PieceType.RAINBOW) return true;
            }
        }

        return false;
    }

    public void PlayClearSound()
    {
        gridAudioSource.Stop();
        gridAudioSource.PlayOneShot(clearPieceSFX[UnityEngine.Random.Range(0, clearPieceSFX.Length)]);
    }

    public void OnGameOver()
    {
        foreach (GamePiece piece in pieces)
        {
            piece.transform.DOMoveY(-6, 1f);
        }
    }

}

