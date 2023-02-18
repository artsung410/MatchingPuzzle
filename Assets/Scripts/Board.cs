using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Board : MonoBehaviour
{
    public static Action onSwapEvent;
    public static Action onReconfirmEvent;

    public int amountX; // 가로 수량
    public int amountY; // 세로 수량

    public int cubeCountX = 0;
    public int cubeCountY = 0;

    public int InitPosX;
    public int InitPosY;
    public int IntervalX;
    public int IntervalY;

    [SerializeField]
    private StagesDB stagesDB;       // 엑셀 시트

    [SerializeField]
    private Transform InitSpawnPoint;

    [SerializeField]
    private GameObject cubePf;   // 타일 프리팹

    [SerializeField]
    private GameObject holePf;   // 홀 프리팹

    [SerializeField]
    private GameObject emptyPf;   // 빈구멍 프리팹

    [SerializeField]
    private int emptyCount;   // 빈구멍 개수 (한 행)

    [SerializeField]
    private GameObject backgroundPf;  // 투명한 프리팹 (좌표용)

    [SerializeField]
    private GameObject spawnerPf; // 스포너 프리팹

    [SerializeField]
    private Color[] cubeColors; // 타일의 색상 종류
                                
    public bool[,] marker; // null값으로 만들기 위한 마커보드 생성
    public string[,] cubeInitials;  // 큐브 이니셜 저장  
    public Cube[,] cubes; // 모든 타일을 2차원 배열에 넣기
    public GameObject[] spawners;  // 새로운 스포너 1처원 배열에 넣기
    public Background[,] backgrounds; // 좌표 전용객체 2차원 배열에 넣기

    public bool IsPickCube = false; // 타일을 쥐고있는지 판별
    public GameObject currnetPickCube; // 현재 쥐고있는 타일
    public PuzzleMatcher puzzleMatcher;  // 체크하고 있는 놈

    private void Awake()
    {
        puzzleMatcher = GetComponent<PuzzleMatcher>();
    }

    private int[] nullCounts;
    void Start()
    {
        cubeCountX = amountX - emptyCount;
        cubeCountY = amountY - emptyCount;

        int cellCount = stagesDB.Entities.Count;
        cubes = new Cube[cubeCountX, cubeCountY];
        spawners = new GameObject[cubeCountX];
        backgrounds = new Background[cubeCountX, cubeCountY];
        cubeInitials = new string[amountX, amountY];
        marker = new bool[cubeCountX, cubeCountY];
        nullCounts = new int[cubeCountX];

        for (int y = 0; y < cellCount; y++)
        {
            cubeInitials[0, y] = stagesDB.Entities[y].C0;
            cubeInitials[1, y] = stagesDB.Entities[y].C1;
            cubeInitials[2, y] = stagesDB.Entities[y].C2;
            cubeInitials[3, y] = stagesDB.Entities[y].C3;
            cubeInitials[4, y] = stagesDB.Entities[y].C4;
            cubeInitials[5, y] = stagesDB.Entities[y].C5;
            cubeInitials[6, y] = stagesDB.Entities[y].C6;
            cubeInitials[7, y] = stagesDB.Entities[y].C7;
            cubeInitials[8, y] = stagesDB.Entities[y].C8;
        }

        Init();

    }

    // 퍼즐 생성

    #region MakePuzzle
    private void Init()
    {
        for (int x = 0; x < amountX; x++)
        {
            for (int y = 0; y < amountY; y++)
            {
                Vector2 newPos = new Vector2(InitSpawnPoint.position.x + x * IntervalX, InitSpawnPoint.position.y - y * IntervalY);
                //int ColorIndex = Random.Range(0, cubeColors.Length);   // 색깔을 랜덤으로 뽑기

                if (cubeInitials[x, y] == "r" || cubeInitials[x, y] == "y" || cubeInitials[x, y] == "g" || cubeInitials[x, y] == "p")
                {
                    GameObject cubeObj = Instantiate(cubePf, newPos, Quaternion.identity);
                    GameObject backGroundObj = Instantiate(backgroundPf, newPos, Quaternion.identity);
                    backGroundObj.transform.parent = transform;

                    Cube cube = cubeObj.GetComponent<Cube>();
                    Background background = backGroundObj.GetComponent<Background>();

                    cube.InitCoord(x - 1 , y - 1);
                    cube.gameObject.name = $"({x - 1}, {y - 1})";
                    cubeObj.transform.parent = transform;
                    cubes[x - 1, y - 1] = cube;

                    background.InitCoord(x - 1, y - 1);
                    backgrounds[x - 1, y - 1] = background;

                    switch (cubeInitials[x, y])
                    {
                        case "g":
                            cube.GetComponent<Image>().color = cubeColors[0];
                            cube.Type = 0;
                            break;
                        case "p":
                            cube.GetComponent<Image>().color = cubeColors[1];
                            cube.Type = 1;
                            break;
                        case "r":
                            cube.GetComponent<Image>().color = cubeColors[2];
                            cube.Type = 2;
                            break;
                        case "y":
                            cube.GetComponent<Image>().color = cubeColors[3];
                            cube.Type = 3;
                            break;
                    }
                }
                else if (cubeInitials[x, y] == "h")
                {
                    GameObject hole = Instantiate(holePf, newPos, Quaternion.identity);
                    hole.transform.parent = transform;

                    // 구멍 부분에 스포너 설치
                    if (y == 0 && x != 0 && x != 8)
                    {
                        GameObject spawner = Instantiate(spawnerPf, newPos, Quaternion.identity);
                        spawner.transform.parent = transform;
                        spawners[x - 1] = spawner;
                    }

                }
                else
                {
                    GameObject empty = Instantiate(emptyPf, newPos, Quaternion.identity);
                    empty.transform.parent = transform;
                }
            }
        }

        InitMarker();
    }



    private void InitMarker()
    {
        for (int x = 0; x < cubeCountX; x++)
        {
            for (int y = 0; y < cubeCountY; y++)
            {
                marker[x, y] = false;
            }
        }
    }

    #endregion

    #region Arrangement

    public void ArrangeCubes()
    {
        destructionAndSaaveCubes();
        fillEmpty();
        InitMarker();
    }

    //빈공간으로 인해 움직이는 큐브들을 스택에 저장한다.
    Stack<Cube> saveCubes = new Stack<Cube>(); 

    private void destructionAndSaaveCubes()
    {
        for (int x = 0; x < cubeCountX; x++)
        {
            nullCounts[x] = 0;
            for (int y = 0; y < cubeCountY; y++)
            {
                if(true == marker[x, y])
                {
                    // 마킹된 큐브들을 제거시킨다.
                    Destroy(cubes[x, y].gameObject);

                    // 열마다 널값을 더해준다.
                    ++nullCounts[x];

                    // 제거된 오브젝트 좌표를 null로 만든다.
                    cubes[x, y] = null;  
                }
                else
                {
                    // null을 제외한 좌표상 모든 큐브들을 담는다. (마킹된 열만 체크)
                    saveCubes.Push(cubes[x, y]);
                }
            }
        }
    }

    private void fillEmpty()
    {
        for (int x = cubeCountX - 1; x >= 0; x--)
        {
            for (int y = cubeCountY - 1; y >= cubeCountY - nullCounts.Length; y--)
            {
                // 저장했던 큐브를 꺼낸다.
                Cube cube = saveCubes?.Pop();
                
                // 스택이 비어있으면, 정렬된 좌표를 제외한 곳을 모두 null로 만든다.
                if (saveCubes.Count == 0)
                {
                    createCubes();
                    return;
                }

                // 좌표를 담아둔다.
                int posX = cube.X;
                // 꺼내봤는데 현재 x좌표와 맞지않는다면 다시 넣어준다.
                if (cube.X != x)
                {
                    saveCubes.Push(cube); 
                    break;
                }

                // 꺼낸큐브를 바닥부터 배치한다. (역순으로)
                cube.SetPosition(backgrounds[posX, y].transform.position);

                // 배열을 다시 세팅한다.
                cubes[posX, y] = cube;

                // 큐브의 정보도 업데이트해준다.
                cube.InitCoord(posX, y);
            }
        }
    }

    // 새로운 큐브를 생성하는 코루틴
    private void createCubes()
    {
        for (int x = 0; x < cubeCountX; x++)
        {
            if (nullCounts[x] == 0)
            {
                continue;
            }

            for (int y = nullCounts[x] - 1; y >= 0; y--)
            {
                // 원래있던 좌표를 널로 만들어준다.
                cubes[x, y] = null;

                                                                                                                    // 겹치지 않게 딜레이를 줌.
                int ColorIndex = UnityEngine.Random.Range(0, cubeColors.Length);                                    // 색깔을 랜덤으로 뽑기
                GameObject cubeObj = Instantiate(cubePf, spawners[x].transform.position, Quaternion.identity);      // 새로운 큐브 생성
                cubeObj.transform.parent = transform;                                                               // 오브젝트 부모 지정

                Cube cube = cubeObj.GetComponent<Cube>();
                cube.SetPosition(backgrounds[x, y].transform.position);                                             // 빈공간으로 큐브 배치
                cube.InitCoord(x, y);                                                                               // 백그라운드 필드좌표와 동일한 좌표 설정

                cube.SetColor(cubeColors[ColorIndex], ColorIndex);                                                  // 색깔 바꿔주기

                // 새로운 큐브를 할당한다.
                cubes[x, y] = cube;                                                              
            }
        }

        StartCoroutine(DelayReconfirm());
    }

    private IEnumerator DelayReconfirm()
    {
        yield return new WaitForSeconds(1.5f);
        puzzleMatcher.CheckAllPattern();
    }

    #endregion

    #region Swap
    public void SwapObj(Cube firstCube, Cube SecondCube)
    {
        Cube tempCube = cubes[firstCube.X, firstCube.Y];
        cubes[firstCube.X, firstCube.Y] = cubes[SecondCube.X, SecondCube.Y];
        cubes[SecondCube.X, SecondCube.Y] = tempCube;

        Vector2 TempPos = firstCube.transform.position;
        firstCube.transform.position = SecondCube.transform.position;
        SecondCube.transform.position = TempPos;
    }

    // 메모리 주소를 넘겨줘서 실질적인 값을 바꿔준다.
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
}