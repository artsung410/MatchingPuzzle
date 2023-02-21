using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Board : MonoBehaviour
{
    [SerializeField] private int IntervalX;                      // 타일간 간격(X)
    [SerializeField] private int IntervalY;                      // 타일간 간격(Y)
    [SerializeField] private int EmptyCount;                     // 빈구멍 개수 (한 행)
    [SerializeField] private Transform InitSpawnPoint;           // 퍼즐 스폰 위치
    [SerializeField] private StagesDB StagesDB;                  // 스테이지 CSV
    [SerializeField] private GameObject MunchkinPF;              // 먼치킨 프리팹
    [SerializeField] private GameObject CandyPf;                 // 캔디 프리팹
    [SerializeField] private GameObject HolePf;                  // 홀 프리팹
    [SerializeField] private GameObject EmptyPf;                 // 빈구멍 프리팹
    [SerializeField] private GameObject BackgroundPf;            // 투명한 프리팹 (좌표용)
    [SerializeField] private GameObject SpawnerPf;               // 스포너 프리팹
    [SerializeField] private Color[] CandyColors;                // 캔디 색상 종류

    private PuzzleMatcher puzzleMatcher;                         // 매칭 정보
    private bool cycleComplete;                                  // 사이클 완료여부
    private int[] nullCounts;                                    // 특정 X좌표의 빈공간 체크

    public int AmountX;                                          // 가로 전체 수량
    public int AmountY;                                          // 세로 전체 수량

    public int CandyCountX;                                      // 가로 캔디 수량
    public int CandyCountY;                                      // 세로 캔디 수량

    public Candy[,] Candies;                                     // 현재 운용중인 모든 캔디
    public Background[,] Backgrounds;                            // 좌표보드
    public bool[,] Marker;                                       // 캔디 제거용 마커보드 생성

    public string[,] CandyInitials;                              // CSV -> 큐브 이니셜 저장  
    public GameObject[] Spawners;                                // 새로운 캔디 생성점
    public List<Candy> PickedCandies;                            // 현재 쥐고있는 캔디 (스왑용)

    private void Awake()
    {
        puzzleMatcher = GetComponent<PuzzleMatcher>();
        Init();
    }

    #region MakePuzzle

    // 퍼즐 생성
    private void Init()
    {
        // 캔디카운트는 빈 공간을 뺀 수량을 할당.
        CandyCountX = AmountX - EmptyCount;
        CandyCountY = AmountY - EmptyCount;


        // 퍼즐관련 변수 초기화
        int cellCount = StagesDB.Entities.Count;
        Candies = new Candy[CandyCountX, CandyCountY];
        Spawners = new GameObject[CandyCountX];
        Backgrounds = new Background[CandyCountX, CandyCountY];
        CandyInitials = new string[AmountX, AmountY];
        Marker = new bool[CandyCountX, CandyCountY];
        nullCounts = new int[CandyCountX];
        cycleComplete = false;

        // DB를 불러온다.
        // TODO: 좀 더 간결하게 작성하기
        for (int y = 0; y < cellCount; y++)
        {
            CandyInitials[0, y] = StagesDB.Entities[y].C0;
            CandyInitials[1, y] = StagesDB.Entities[y].C1;
            CandyInitials[2, y] = StagesDB.Entities[y].C2;
            CandyInitials[3, y] = StagesDB.Entities[y].C3;
            CandyInitials[4, y] = StagesDB.Entities[y].C4;
            CandyInitials[5, y] = StagesDB.Entities[y].C5;
            CandyInitials[6, y] = StagesDB.Entities[y].C6;
            CandyInitials[7, y] = StagesDB.Entities[y].C7;
            CandyInitials[8, y] = StagesDB.Entities[y].C8;
        }

        // DB를 바탕으로 퍼즐요소들을 설치한다.
        for (int x = 0; x < AmountX; x++)
        {
            for (int y = 0; y < AmountY; y++)
            {
                Vector2 newPos = new Vector2(InitSpawnPoint.position.x + x * IntervalX, InitSpawnPoint.position.y - y * IntervalY);

                if (CandyInitials[x, y] == "r" || CandyInitials[x, y] == "y" || CandyInitials[x, y] == "g" || CandyInitials[x, y] == "p")
                {

                    // 캔디 세팅
                    GameObject candyObj = Instantiate(CandyPf, newPos, Quaternion.identity);
                    Candy candy = candyObj.GetComponent<Candy>();
                    candy.OnGround = true;
                    candy.InitCoord(x - 1, y - 1);
                    candyObj.transform.SetParent(transform);
                    candyObj.transform.localScale = Vector3.one;
                    Candies[x - 1, y - 1] = candy;

                    // 백그라운드 세팅
                    GameObject backGroundObj = Instantiate(BackgroundPf, newPos, Quaternion.identity);
                    Background background = backGroundObj.GetComponent<Background>();
                    background.InitCoord(x - 1, y - 1);
                    Backgrounds[x - 1, y - 1] = background;
                    backGroundObj.transform.SetParent(transform);
                    backGroundObj.transform.localScale = Vector3.one;

                    switch (CandyInitials[x, y])
                    {
                        case "g":
                            candy.GetComponent<Image>().color = CandyColors[0];
                            candy.Type = 0;
                            break;
                        case "p":
                            candy.GetComponent<Image>().color = CandyColors[1];
                            candy.Type = 1;
                            break;
                        case "r":
                            candy.GetComponent<Image>().color = CandyColors[2];
                            candy.Type = 2;
                            break;
                        case "y":
                            candy.GetComponent<Image>().color = CandyColors[3];
                            candy.Type = 3;
                            break;
                    }
                }
                else if (CandyInitials[x, y] == "h")
                {
                    // 홀 세팅
                    GameObject hole = Instantiate(HolePf, newPos, Quaternion.identity);
                    hole.transform.SetParent(transform);
                    hole.transform.localScale = Vector3.one;

                    if (y == 0 && x != 0 && x != 8)
                    {
                        // 스포너 세팅
                        GameObject spawner = Instantiate(SpawnerPf, newPos, Quaternion.identity);
                        spawner.transform.SetParent(transform);
                        spawner.transform.localScale = Vector3.one;
                        Spawners[x - 1] = spawner;
                    }
                }
                else
                {
                    // 빈공간 세팅
                    GameObject empty = Instantiate(EmptyPf, newPos, Quaternion.identity);
                    empty.transform.SetParent(transform);
                    empty.transform.localScale = Vector3.one;
                }
            }
        }

        InitMarker();
    }
    #endregion

    #region Arrangement

    // 퍼즐 정리
    public void ArrangeCandies()
    {
        GameManager.Instance.onButtonDisableEvent?.Invoke();
        DestructionCandies();
        FillEmpty();
        CreateCandies();
        cycleComplete = true;
    }

    // 빈공간으로 인해 움직이는 캔디들을 스택에 저장한다.
    private Stack<Candy> savedCandies = new Stack<Candy>();


    // 퍼즐 파괴
    private void DestructionCandies()
    {
        for (int x = 0; x < CandyCountX; x++)
        {
            nullCounts[x] = 0;
            for (int y = 0; y < CandyCountY; y++)
            {
                if(true == Marker[x, y])
                {
                    // 마킹된 큐브들을 제거시킨다.
                    Destroy(Candies[x, y].gameObject);

                    // 열마다 널값을 더해준다.
                    ++nullCounts[x];

                    // 제거된 오브젝트 좌표를 null로 만든다.
                    Candies[x, y] = null;

                    // 스코어를 업데이트한다.
                    int score = GameManager.Instance.blockDestructionScore;
                    GameManager.Instance.SetScore(score);
                }
                else
                {
                    // null을 제외한 좌표상 모든 큐브들을 담는다. (마킹된 열만 체크)
                    savedCandies.Push(Candies[x, y]);
                }
            }
        }
    }

    // 퍼즐 정렬
    private void FillEmpty()
    {
        for (int x = CandyCountX - 1; x >= 0; x--)
        {
            for (int y = CandyCountY - 1; y >= 0; y--)
            {
                // 저장했던 큐브를 꺼낸다.
                Candy candy = savedCandies?.Pop();

                // 좌표를 담아둔다.
                int posX = candy.X;

                // 꺼내봤는데 현재 x좌표와 맞지않는다면 다시 넣어준다.
                if (candy.X != x)
                {
                    savedCandies.Push(candy); 
                    break;
                }

                // 꺼낸큐브를 바닥부터 배치한다. (역순으로)
                candy.SetPosition(Backgrounds[posX, y].transform.position);

                // 배열을 다시 세팅한다.
                Candies[posX, y] = candy;

                // 큐브의 정보도 업데이트 해준다.
                candy.InitCoord(posX, y);

                // 스택이 비어있으면, 정렬된 좌표를 제외한 곳을 모두 null로 만든다.
                if (savedCandies.Count == 0)
                {
                    return;
                }
            }
        }
    }

    // 퍼즐 생성
    public void CreateCandies()
    {
        StartCoroutine(SpawnCandy());
    }

    // 퍼즐 생성
    private IEnumerator SpawnCandy()
    {
        for (int x = 0; x < CandyCountX; x++)
        {
            // 현재 x좌표에 빈공간이 없으면 다음 x좌표로 건너뛴다.
            if (nullCounts[x] == 0)
            {
                continue;
            }

            for (int y = nullCounts[x] - 1; y >= 0; y--)
            {
                // 원래있던 좌표를 널로 만들어준다.
                Candies[x, y] = null;

                // 랜덤 색상 적용
                int ColorIndex = UnityEngine.Random.Range(0, CandyColors.Length);

                // 캔디 생성 & 오브젝트 부모 지정
                GameObject candyObj = Instantiate(CandyPf, Spawners[x].transform.position, Quaternion.identity);      
                candyObj.transform.SetParent(transform);
                candyObj.transform.localScale = Vector3.one;

                // 캔디 배치 & 초기화
                Candy candy = candyObj.GetComponent<Candy>();
                candy.DisableButton();
                candy.SetPosition(Backgrounds[x, y].transform.position);                                             
                candy.InitCoord(x, y);                                                                           
                candy.SetColor(CandyColors[ColorIndex], ColorIndex);                                                 

                // 새로운 큐브를 할당한다.
                Candies[x, y] = candy;

                // 겹치지 않게 딜레이를 줌.
                yield return new WaitForSeconds(0.01f);
            }
        }
        InitMarker();
    }

    // 퍼즐 생성
    private void InitMarker()
    {
        for (int x = 0; x < CandyCountX; x++)
        {
            for (int y = 0; y < CandyCountY; y++)
            {
                Marker[x, y] = false;
            }
        }
    }
    #endregion

    #region FinalCheck

    private int matchPosCount;

    // 좌표를 실시간으로 분석한다.
    private void ComparePosition()
    {
        matchPosCount = 0;
        for (int x = 0; x < CandyCountX; x++)
        {
            for (int y = 0; y < CandyCountY; y++)
            {
                if (Candies[x, y] == null)
                {
                    return;
                }

                // 캔디가 전부 멈춰있거나, 바닥에 도달했으면 매치카운트를 1씩 더해준다.
                if (true == Candies[x, y].OnGround)
                {
                    ++matchPosCount;
                }
            }
        }

        // 사이클이 전부 완료되었고, 배경 좌표와 실제 큐브좌표가 전부 일치할때 사이클을 다시 실행시켜준다.
        if (cycleComplete == true && matchPosCount == CandyCountX * CandyCountY)
        {
            cycleComplete = false;
            matchPosCount = 0;
            StartCoroutine(DelayInvoke());
        }
    }

    // 다음 사이클을 실행하기전 잠깐 대기시켜준다.
    private IEnumerator DelayInvoke()
    {
        yield return new WaitForSeconds(0.5f);
        puzzleMatcher.CheckAllPattern();
    }

    private void Update()
    {
        ComparePosition();
    }

    #endregion

    #region Swap

    // 인접한 캔디를 스왑시켜준다.
    public void SwapObj(Candy firstCandy, Candy secondCandy)
    {
        Candy tempCandy = Candies[firstCandy.X, firstCandy.Y];
        Candies[firstCandy.X, firstCandy.Y] = Candies[secondCandy.X, secondCandy.Y];
        Candies[secondCandy.X, secondCandy.Y] = tempCandy;

        Vector2 TempPos = firstCandy.transform.position;
        firstCandy.SetPositionForSwap(secondCandy.transform.position);
        secondCandy.SetPositionForSwap(TempPos);
    }

    // 주소를 가져와서 실질적인 값을 바꿔준다.
    public void SwapPos(ref int x1, ref int y1, ref int x2, ref int y2)
    {
        int tempX1 = x1;
        x1 = x2;
        x2 = tempX1;

        int tempY1 = y1;
        y1 = y2;
        y2 = tempY1;
    }

    // 스왑시킬 캔디를 리스트에 담는다.
    public void PickCandies(Candy candy)
    {
        PickedCandies.Add(candy);
    }
    #endregion

    #region SpecialCandy

    // 먼치킨 생성
    public void CreateBall(int x, int y)
    {
        Destroy(Candies[x, y].gameObject);
        Candies[x, y] = null;

        GameObject newObj = Instantiate(MunchkinPF, Backgrounds[x, y].transform.position, Quaternion.identity);
        Candy candy = newObj.GetComponent<Candy>();
        candy.transform.SetParent(transform);
        candy.transform.localScale = Vector3.one;
        candy.InitCoord(x, y);
        candy.SetColor(Color.white, 7);
        Candies[x, y] = candy;
    }
    #endregion
}