using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Board : MonoBehaviour
{
    [SerializeField] private float IntervalX;                      // Ÿ?ϰ? ????(X)
    [SerializeField] private float IntervalY;                      // Ÿ?ϰ? ????(Y)
    [SerializeField] private int EmptyCount;                     // ?󱸸? ???? (?? ??)
    [SerializeField] private Transform InitSpawnPoint;           // ???? ???? ??ġ
    [SerializeField] private StagesDB StagesDB;                  // ???????? CSV
    [SerializeField] private GameObject MunchkinPF;              // ??ġŲ ??????
    [SerializeField] private GameObject CandyPf;                 // ĵ?? ??????
    [SerializeField] private GameObject HolePf;                  // Ȧ ??????
    [SerializeField] private GameObject EmptyPf;                 // ?󱸸? ??????
    [SerializeField] private GameObject BackgroundPf;            // ?????? ?????? (??ǥ??)
    [SerializeField] private GameObject SpawnerPf;               // ?????? ??????
    [SerializeField] private Color[] CandyColors;                // ĵ?? ???? ????

    private PuzzleMatcher puzzleMatcher;                         // ??Ī ????
    private bool cycleComplete;                                  // ????Ŭ ?ϷῩ??
    private int[] nullCounts;                                    // Ư?? X??ǥ?? ?????? üũ

    public int AmountX;                                          // ???? ??ü ????
    public int AmountY;                                          // ???? ??ü ????

    public int CandyCountX;                                      // ???? ĵ?? ????
    public int CandyCountY;                                      // ???? ĵ?? ????

    public Candy[,] Candies;                                     // ???? ???????? ???? ĵ??
    public Background[,] Backgrounds;                            // ??ǥ????
    public bool[,] Marker;                                       // ĵ?? ???ſ? ??Ŀ???? ????

    public string[,] CandyInitials;                              // CSV -> ť?? ?̴ϼ? ????  
    public GameObject[] Spawners;                                // ???ο? ĵ?? ??????
    public Candy PickedCandy;                                    // ???ҿ????? ĵ??

    private void Awake()
    {
        puzzleMatcher = GetComponent<PuzzleMatcher>();
        Init();
    }

    #region MakePuzzle

    // ???? ????
    private void Init()
    {
        // ĵ??ī??Ʈ?? ?? ?????? ?? ?????? ?Ҵ?.
        CandyCountX = AmountX - EmptyCount;
        CandyCountY = AmountY - EmptyCount;

        // ???????? ???? ?ʱ?ȭ
        int cellCount = StagesDB.Entities.Count;
        Candies = new Candy[CandyCountX, CandyCountY];
        Spawners = new GameObject[CandyCountX];
        Backgrounds = new Background[CandyCountX, CandyCountY];
        CandyInitials = new string[AmountX, AmountY];
        Marker = new bool[CandyCountX, CandyCountY];
        nullCounts = new int[CandyCountX];
        cycleComplete = false;

        // DB?? ?ҷ??´?.
        // TODO: ?? ?? ?????ϰ? ?ۼ??ϱ?
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

        // DB?? ???????? ???????ҵ??? ??ġ?Ѵ?.
        for (int x = 0; x < AmountX; x++)
        {
            for (int y = 0; y < AmountY; y++)
            {
                Vector2 newPos = new Vector2(InitSpawnPoint.position.x + x * IntervalX, InitSpawnPoint.position.y - y * IntervalY);

                if (CandyInitials[x, y] == "r" || CandyInitials[x, y] == "y" || CandyInitials[x, y] == "g" || CandyInitials[x, y] == "p")
                {

                    // ĵ?? ????
                    GameObject candyObj = Instantiate(CandyPf, newPos, Quaternion.identity);
                    Candy candy = candyObj.GetComponent<Candy>();
                    candy.OnGround = true;
                    candy.InitCoord(x - 1, y - 1);
                    candyObj.transform.SetParent(transform);
                    candyObj.transform.localScale = Vector3.one;
                    Candies[x - 1, y - 1] = candy;

                    // ???׶????? ????
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
                    // Ȧ ????
                    GameObject hole = Instantiate(HolePf, newPos, Quaternion.identity);
                    hole.transform.SetParent(transform);
                    hole.transform.localScale = Vector3.one;

                    if (y == 0 && x != 0 && x != 8)
                    {
                        // ?????? ????
                        GameObject spawner = Instantiate(SpawnerPf, newPos, Quaternion.identity);
                        spawner.transform.SetParent(transform);
                        spawner.transform.localScale = Vector3.one;
                        Spawners[x - 1] = spawner;
                    }
                }
                else
                {
                    // ?????? ????
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

    // ???? ????
    public void ArrangeCandies()
    {
        GameManager.Instance.onButtonDisableEvent?.Invoke();
        DestructionCandies();
        FillEmpty();
        CreateCandies();
        cycleComplete = true;
    }

    // ?????????? ???? ?????̴? ĵ?????? ???ÿ? ?????Ѵ?.
    private Stack<Candy> savedCandies = new Stack<Candy>();


    // ???? ?ı?
    private void DestructionCandies()
    {
        for (int x = 0; x < CandyCountX; x++)
        {
            nullCounts[x] = 0;
            for (int y = 0; y < CandyCountY; y++)
            {
                if(true == Marker[x, y])
                {
                    // ??ŷ?? ť?????? ???Ž?Ų??.
                    Destroy(Candies[x, y].gameObject);

                    // ?????? ?ΰ??? ?????ش?.
                    ++nullCounts[x];

                    // ???ŵ? ??????Ʈ ??ǥ?? null?? ??????.
                    Candies[x, y] = null;

                    // ???ھ ??????Ʈ?Ѵ?.
                    int score = GameManager.Instance.blockDestructionScore;
                    GameManager.Instance.SetScore(score);
                }
                else
                {
                    // null?? ?????? ??ǥ?? ???? ť?????? ???´?. (??ŷ?? ???? üũ)
                    savedCandies.Push(Candies[x, y]);
                }
            }
        }
    }

    // ???? ????
    private void FillEmpty()
    {
        for (int x = CandyCountX - 1; x >= 0; x--)
        {
            for (int y = CandyCountY - 1; y >= 0; y--)
            {
                // ?????ߴ? ť?긦 ??????.
                Candy candy = savedCandies?.Pop();

                // ??ǥ?? ???Ƶд?.
                int posX = candy.X;

                // ?????ôµ? ???? x??ǥ?? ?????ʴ´ٸ? ?ٽ? ?־??ش?.
                if (candy.X != x)
                {
                    savedCandies.Push(candy); 
                    break;
                }

                // ????ť?긦 ?ٴں??? ??ġ?Ѵ?. (????????)
                candy.SetPosition(Backgrounds[posX, y].transform.position);

                // ?迭?? ?ٽ? ?????Ѵ?.
                Candies[posX, y] = candy;

                // ť???? ?????? ??????Ʈ ???ش?.
                candy.InitCoord(posX, y);

                // ?????? ??????????, ???ĵ? ??ǥ?? ?????? ???? ???? null?? ??????.
                if (savedCandies.Count == 0)
                {
                    return;
                }
            }
        }
    }

    // ???? ????
    public void CreateCandies()
    {
        StartCoroutine(SpawnCandy());
    }

    // ???? ????
    private IEnumerator SpawnCandy()
    {
        for (int x = 0; x < CandyCountX; x++)
        {
            // ???? x??ǥ?? ???????? ?????? ???? x??ǥ?? ?ǳʶڴ?.
            if (nullCounts[x] == 0)
            {
                continue;
            }

            for (int y = nullCounts[x] - 1; y >= 0; y--)
            {
                // ?????ִ? ??ǥ?? ?η? ???????ش?.
                Candies[x, y] = null;

                // ???? ???? ????
                int ColorIndex = UnityEngine.Random.Range(0, CandyColors.Length);

                // ĵ?? ???? & ??????Ʈ ?θ? ????
                GameObject candyObj = Instantiate(CandyPf, Spawners[x].transform.position, Quaternion.identity);      
                candyObj.transform.SetParent(transform);
                candyObj.transform.localScale = Vector3.one;

                // ĵ?? ??ġ & ?ʱ?ȭ
                Candy candy = candyObj.GetComponent<Candy>();
                candy.DisableButton();
                candy.SetPosition(Backgrounds[x, y].transform.position);                                             
                candy.InitCoord(x, y);                                                                           
                candy.SetColor(CandyColors[ColorIndex], ColorIndex);                                                 

                // ???ο? ť?긦 ?Ҵ??Ѵ?.
                Candies[x, y] = candy;

                // ??ġ?? ?ʰ? ?????̸? ??.
                yield return new WaitForSeconds(0.01f);
            }
        }
        InitMarker();
    }

    // ???? ????
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

    // ??ǥ?? ?ǽð????? ?м??Ѵ?.
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

                // ĵ???? ???? ?????ְų?, ?ٴڿ? ?????????? ??ġī??Ʈ?? 1?? ?????ش?.
                if (true == Candies[x, y].OnGround)
                {
                    ++matchPosCount;
                }
            }
        }

        // ????Ŭ?? ???? ?Ϸ??Ǿ???, ???? ??ǥ?? ???? ť????ǥ?? ???? ??ġ?Ҷ? ????Ŭ?? ?ٽ? ?????????ش?.
        if (cycleComplete == true && matchPosCount == CandyCountX * CandyCountY)
        {
            cycleComplete = false;
            matchPosCount = 0;
            StartCoroutine(DelayInvoke());
        }
    }

    // ???? ????Ŭ?? ?????ϱ??? ???? ?????????ش?.
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

    // ?????? ĵ???? ???ҽ????ش?.
    public void SwapObj(Candy firstCandy, Candy secondCandy)
    {
        Candy tempCandy = Candies[firstCandy.X, firstCandy.Y];
        Candies[firstCandy.X, firstCandy.Y] = Candies[secondCandy.X, secondCandy.Y];
        Candies[secondCandy.X, secondCandy.Y] = tempCandy;
    }

    // ?ּҸ? ?????ͼ? ???????? ???? ?ٲ??ش?.
    public void SwapPos(ref int x1, ref int y1, ref int x2, ref int y2)
    {
        int tempX1 = x1;
        x1 = x2;
        x2 = tempX1;

        int tempY1 = y1;
        y1 = y2;
        y2 = tempY1;
    }

    #endregion

    #region SpecialCandy

    // ??ġŲ ????
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