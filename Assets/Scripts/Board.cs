using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Board : MonoBehaviour
{
    public int amountX; // 가로 수량
    public int amountY; // 세로 수량

    public int candyCountX = 0;
    public int candyCountY = 0;

    public int InitPosX;
    public int InitPosY;
    public int IntervalX;
    public int IntervalY;

    [SerializeField]
    private StagesDB stagesDB;                  // 엑셀 시트

    [SerializeField]
    private Transform InitSpawnPoint;

    [SerializeField]
    private GameObject ballPf;                  // 볼 프리팹

    [SerializeField]
    private GameObject candyPf;                  // 타일 프리팹

    [SerializeField]
    private GameObject holePf;                  // 홀 프리팹

    [SerializeField]
    private GameObject emptyPf;                 // 빈구멍 프리팹

    [SerializeField]
    private int emptyCount;                     // 빈구멍 개수 (한 행)

    [SerializeField]
    private GameObject backgroundPf;            // 투명한 프리팹 (좌표용)

    [SerializeField]
    private GameObject spawnerPf;               // 스포너 프리팹

    [SerializeField]
    private Color[] candyColors;                // 캔디 색상 종류
                                
    public bool[,] marker;                      // null값으로 만들기 위한 마커보드 생성
    public string[,] candyInitials;             // 큐브 이니셜 저장  
    public Candy[,] candies;                    // 모든 타일을 2차원 배열에 넣기
    public GameObject[] spawners;               // 새로운 스포너 1처원 배열에 넣기
    public Background[,] backgrounds;           // 좌표 전용객체 2차원 배열에 넣기

    public List<Candy> PickedCandies;                 // 현재 쥐고있는 캔디
    public PuzzleMatcher puzzleMatcher;         

    private bool cycleComplete = false;
    private int[] nullCounts;

    private bool onMoveAble = true;
    public bool OnMoveAble
    {
        get => onMoveAble;
        set => onMoveAble = value;
    }

    private void Awake()
    {
        puzzleMatcher = GetComponent<PuzzleMatcher>();
        init();
    }

    // 퍼즐 생성

    #region MakePuzzle
    private void init()
    {
        candyCountX = amountX - emptyCount;
        candyCountY = amountY - emptyCount;

        int cellCount = stagesDB.Entities.Count;
        candies = new Candy[candyCountX, candyCountY];
        spawners = new GameObject[candyCountX];
        backgrounds = new Background[candyCountX, candyCountY];
        candyInitials = new string[amountX, amountY];
        marker = new bool[candyCountX, candyCountY];
        nullCounts = new int[candyCountX];

        for (int y = 0; y < cellCount; y++)
        {
            candyInitials[0, y] = stagesDB.Entities[y].C0;
            candyInitials[1, y] = stagesDB.Entities[y].C1;
            candyInitials[2, y] = stagesDB.Entities[y].C2;
            candyInitials[3, y] = stagesDB.Entities[y].C3;
            candyInitials[4, y] = stagesDB.Entities[y].C4;
            candyInitials[5, y] = stagesDB.Entities[y].C5;
            candyInitials[6, y] = stagesDB.Entities[y].C6;
            candyInitials[7, y] = stagesDB.Entities[y].C7;
            candyInitials[8, y] = stagesDB.Entities[y].C8;
        }

        for (int x = 0; x < amountX; x++)
        {
            for (int y = 0; y < amountY; y++)
            {
                Vector2 newPos = new Vector2(InitSpawnPoint.position.x + x * IntervalX, InitSpawnPoint.position.y - y * IntervalY);

                if (candyInitials[x, y] == "r" || candyInitials[x, y] == "y" || candyInitials[x, y] == "g" || candyInitials[x, y] == "p")
                {
                    GameObject candyObj = Instantiate(candyPf, newPos, Quaternion.identity);
                    GameObject backGroundObj = Instantiate(backgroundPf, newPos, Quaternion.identity);
                    backGroundObj.transform.SetParent(transform);

                    Candy candy = candyObj.GetComponent<Candy>();
                    Background background = backGroundObj.GetComponent<Background>();

                    candy.onGround = true;                               // 처음에 큐브는 고정되어있으므로 onGround를 true로 해준다.
                    candy.InitCoord(x - 1 , y - 1);                      
                    candyObj.transform.SetParent(transform);
                    candies[x - 1, y - 1] = candy;

                    background.InitCoord(x - 1, y - 1);
                    backgrounds[x - 1, y - 1] = background;

                    switch (candyInitials[x, y])
                    {
                        case "g":
                            candy.GetComponent<Image>().color = candyColors[0];
                            candy.Type = 0;
                            break;
                        case "p":
                            candy.GetComponent<Image>().color = candyColors[1];
                            candy.Type = 1;
                            break;
                        case "r":
                            candy.GetComponent<Image>().color = candyColors[2];
                            candy.Type = 2;
                            break;
                        case "y":
                            candy.GetComponent<Image>().color = candyColors[3];
                            candy.Type = 3;
                            break;
                    }
                }
                else if (candyInitials[x, y] == "h")
                {
                    GameObject hole = Instantiate(holePf, newPos, Quaternion.identity);
                    hole.transform.SetParent(transform);

                    // 구멍 부분에 스포너 설치
                    if (y == 0 && x != 0 && x != 8)
                    {
                        GameObject spawner = Instantiate(spawnerPf, newPos, Quaternion.identity);
                        spawner.transform.SetParent(transform);
                        spawners[x - 1] = spawner;
                    }

                }
                else
                {
                    GameObject empty = Instantiate(emptyPf, newPos, Quaternion.identity);
                    empty.transform.SetParent(transform);
                }
            }
        }

        InitMarker();
    }




    #endregion

    #region Arrangement

    public void ArrangeCandies()
    {
        onMoveAble = false;

        destructionCandies();
        fillEmpty();
        createCandy();

        cycleComplete = true;
    }

    //빈공간으로 인해 움직이는 큐브들을 스택에 저장한다.
    Stack<Candy> savedCandies = new Stack<Candy>();
    


    private void destructionCandies()
    {
        for (int x = 0; x < candyCountX; x++)
        {
            nullCounts[x] = 0;
            for (int y = 0; y < candyCountY; y++)
            {
                if(true == marker[x, y])
                {
                    // 마킹된 큐브들을 제거시킨다.
                    Destroy(candies[x, y].gameObject);

                    // 열마다 널값을 더해준다.
                    ++nullCounts[x];

                    // 제거된 오브젝트 좌표를 null로 만든다.
                    candies[x, y] = null;

                    int score = GameManager.Instance.blockDestructionScore;
                    GameManager.Instance.SetScore(score);
                }
                else
                {
                    // null을 제외한 좌표상 모든 큐브들을 담는다. (마킹된 열만 체크)
                    savedCandies.Push(candies[x, y]);
                }
            }
        }
    }

    private void fillEmpty()
    {
        for (int x = candyCountX - 1; x >= 0; x--)
        {
            for (int y = candyCountY - 1; y >= 0; y--)
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
                candy.SetPosition(backgrounds[posX, y].transform.position);

                // 배열을 다시 세팅한다.
                candies[posX, y] = candy;

                // 큐브의 정보도 업데이트해준다.
                candy.InitCoord(posX, y);

                // 스택이 비어있으면, 정렬된 좌표를 제외한 곳을 모두 null로 만든다.
                if (savedCandies.Count == 0)
                {
                    return;
                }
            }
        }
    }

    // 새로운 큐브를 생성하는 코루틴
    public void createCandy()
    {
        StartCoroutine(spawnCandy());
    }

    private IEnumerator spawnCandy()
    {
        for (int x = 0; x < candyCountX; x++)
        {
            if (nullCounts[x] == 0)
            {
                continue;
            }

            for (int y = nullCounts[x] - 1; y >= 0; y--)
            {
                // 원래있던 좌표를 널로 만들어준다.`
                candies[x, y] = null;

                // 겹치지 않게 딜레이를 줌.
                int ColorIndex = UnityEngine.Random.Range(0, candyColors.Length);                                    // 색깔을 랜덤으로 뽑기
                GameObject candyObj = Instantiate(candyPf, spawners[x].transform.position, Quaternion.identity);      // 새로운 큐브 생성
                candyObj.transform.SetParent(transform);                                                               // 오브젝트 부모 지정

                Candy candy = candyObj.GetComponent<Candy>();
                candy.SetPosition(backgrounds[x, y].transform.position);                                             // 빈공간으로 큐브 배치
                candy.InitCoord(x, y);                                                                               // 백그라운드 필드좌표와 동일한 좌표 설정

                candy.SetColor(candyColors[ColorIndex], ColorIndex);                                                  // 색깔 바꿔주기

                // 새로운 큐브를 할당한다.
                candies[x, y] = candy;
                yield return new WaitForSeconds(0.1f);
            }
        }

        InitMarker();
    }

    private void InitMarker()
    {
        for (int x = 0; x < candyCountX; x++)
        {
            for (int y = 0; y < candyCountY; y++)
            {
                marker[x, y] = false;
            }
        }
    }
    #endregion

    // 좌표를 실시간으로 분석한다.
    int matchPosCount;

    private void comparePosition()
    {
        matchPosCount = 0;
        for (int x = 0; x < candyCountX; x++)
        {
            for (int y = 0; y < candyCountY; y++)
            {
                if (candies[x, y] == null)
                {
                    return;
                }

                if (true == candies[x, y].onGround)
                {
                    ++matchPosCount;
                }
            }
        }

        Debug.Log(matchPosCount);

        // 사이클이 전부 완료되었고, 배경 좌표와 실제 큐브좌표가 전부 일치할때 사이클을 다시 실행시켜준다.
        if (cycleComplete == true && matchPosCount == candyCountX * candyCountY)
        {
            cycleComplete = false;
            matchPosCount = 0;
            puzzleMatcher.CheckAllPattern();
        }
    }
    #region Swap

    public void SwapObj(Candy firstCandy, Candy secondCandy)
    {
        Candy tempCandy = candies[firstCandy.X, firstCandy.Y];
        candies[firstCandy.X, firstCandy.Y] = candies[secondCandy.X, secondCandy.Y];
        candies[secondCandy.X, secondCandy.Y] = tempCandy;

        Vector2 TempPos = firstCandy.transform.position;
        firstCandy.SetPositionForSwap(secondCandy.transform.position);
        secondCandy.SetPositionForSwap(TempPos);
    }

    public void SwapPos(ref int x1, ref int y1, ref int x2, ref int y2)
    {
        int tempX1 = x1;
        x1 = x2;
        x2 = tempX1;

        int tempY1 = y1;
        y1 = y2;
        y2 = tempY1;
    }

    public void CreateBall(int x, int y)
    {
        Destroy(candies[x, y].gameObject);
        candies[x, y] = null;

        GameObject newObj = Instantiate(ballPf, backgrounds[x, y].transform.position, Quaternion.identity);
        Candy cnady = newObj.GetComponent<Candy>();
        cnady.transform.SetParent(transform);

        cnady.InitCoord(x, y);
        cnady.SetColor(Color.white, 7);
        candies[x, y] = cnady;
    }

    public void PickCandies(Candy candy)
    {
        PickedCandies.Add(candy);
    }

    private void Update()
    {
        comparePosition();
    }
    #endregion
}