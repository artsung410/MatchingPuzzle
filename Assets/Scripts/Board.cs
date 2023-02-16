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
    private GameObject backgroundPf;  // 투명한 프리팹 (좌표용)

    [SerializeField]
    private GameObject spawnerPf; // 스포너 프리팹

    [SerializeField]
    private Color[] cubeColors; // 타일의 색상 종류

    public string[,] cubeInitials;  // 큐브 이니셜 저장  
    public GameObject[,] cubes; // 모든 타일을 2차원 배열에 넣기
    public GameObject[] spawners;  // 새로운 스포너 1처원 배열에 넣기
    public Background[,] backgrounds; // 좌표 전용객체 2차원 배열에 넣기

    public bool IsPickCube = false; // 타일을 쥐고있는지 판별
    public GameObject currnetPickCube; // 현재 쥐고있는 타일
    public PuzzleMatcher puzzleMatcher;  // 체크하고 있는 놈

    private void Awake()
    {
        puzzleMatcher = GetComponent<PuzzleMatcher>();
    }

    void Start()
    {
        int cellCount = stagesDB.Entities.Count;
        cubes = new GameObject[amountX, amountY];
        spawners = new GameObject[amountX];
        backgrounds = new Background[amountX, amountY];
        cubeInitials = new string[amountX, amountY];

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

        for (int x = 0; x < amountX; x++)
        {
            for (int y = 0; y < amountY; y++)
            {
                Debug.Log(cubeInitials[x, y]);
            }
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
                    GameObject coordObj = Instantiate(backgroundPf, newPos, Quaternion.identity);
                    coordObj.transform.parent = transform;

                    Cube cube = cubeObj.GetComponent<Cube>();
                    Background coord = coordObj.GetComponent<Background>();

                    cube.InitCoord(x, y);
                    cube.gameObject.name = $"({x}, {y})";
                    cubeObj.transform.parent = transform;
                    cubes[x, y] = cubeObj;

                    coord.InitCoord(x, y);
                    backgrounds[x, y] = coord;

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
                    if (y == 0)
                    {
                        GameObject spawner = Instantiate(spawnerPf, newPos, Quaternion.identity);
                        spawner.transform.parent = transform;
                        spawners[x] = spawner;
                    }

                }
                else
                {
                    GameObject empty = Instantiate(emptyPf, newPos, Quaternion.identity);
                    empty.transform.parent = transform;
                }
            }
        }
    }

    // 가로 5줄   
    public void customRendering_For3RowPattern()
    {
        EraseRendering();

        amountX = 5;
        amountY = 1;

        cubes = new GameObject[amountX, amountY];
        backgrounds = new Background[amountX, amountY];
        spawners = new GameObject[amountX];

        for (int x = 0; x < amountX; x++)
        {
            for (int y = 0; y < amountY; y++)
            {
                Vector2 newPos = new Vector2(InitPosX + x * IntervalX, InitPosY + y * IntervalY);

                GameObject cubeObj = Instantiate(cubePf, newPos, Quaternion.identity);
                GameObject coordObj = Instantiate(backgroundPf, newPos, Quaternion.identity);
                coordObj.transform.parent = transform;

                Cube cube = cubeObj.GetComponent<Cube>();
                Background coord = coordObj.GetComponent<Background>();

                if (x == 2)
                {
                    cube.SetColor(cubeColors[1], 1);
                }
                else
                {
                    cube.SetColor(cubeColors[0], 0);
                }

                cube.InitCoord(x, y);
                cubeObj.transform.parent = transform;
                cubes[x, y] = cubeObj;
                backgrounds[x, y] = coord;

                // 맨 위에 스포너 설치
                if (y == (amountY - 1))
                {
                    Vector2 spawnerPos = new Vector2(newPos.x, newPos.y + IntervalY);
                    GameObject spawner = Instantiate(spawnerPf, spawnerPos, Quaternion.identity);
                    spawners[x] = spawner;
                }
            }
        }
    }

    // 세로 5줄
    public void customRendering_For3ColPattern()
    {
        EraseRendering();

        amountX = 1;
        amountY = 5;
        cubes = new GameObject[amountX, amountY];
        backgrounds = new Background[amountX, amountY];
        spawners = new GameObject[amountX];

        for (int x = 0; x < amountX; x++)
        {
            for (int y = 0; y < amountY; y++)
            {
                Vector2 newPos = new Vector2(InitPosX + x * IntervalX, InitPosY + y * IntervalY);

                GameObject cubeObj = Instantiate(cubePf, newPos, Quaternion.identity);
                GameObject coordObj = Instantiate(backgroundPf, newPos, Quaternion.identity);
                coordObj.transform.parent = transform;

                Cube cube = cubeObj.GetComponent<Cube>();
                Background coord = coordObj.GetComponent<Background>();

                if (y == 2)
                {
                    cube.GetComponent<Image>().color = cubeColors[1];
                    cube.Type = 1;
                }
                else
                {
                    cube.GetComponent<Image>().color = cubeColors[0];
                    cube.Type = 0;
                }

                cube.InitCoord(x, y);
                cubeObj.transform.parent = transform;
                cubes[x, y] = cubeObj;
                backgrounds[x, y] = coord;

                // 맨 위에 스포너 설치
                if (y == (amountY - 1))
                {
                    Vector2 spawnerPos = new Vector2(newPos.x, newPos.y + IntervalY);
                    GameObject spawner = Instantiate(spawnerPf, spawnerPos, Quaternion.identity);
                    spawners[x] = spawner;
                }
            }
        }
    }

    // 3x2
    public void customRendering_ForSquarePattern()
    {
        EraseRendering();

        amountX = 3;
        amountY = 2;
        cubes = new GameObject[amountX, amountY];
        backgrounds = new Background[amountX, amountY];
        spawners = new GameObject[amountX];

        for (int x = 0; x < amountX; x++)
        {
            for (int y = 0; y < amountY; y++)
            {
                Vector2 newPos = new Vector2(InitPosX + x * IntervalX, InitPosY + y * IntervalY);
                //int ColorIndex = Random.Range(0, cubeColors.Length);   // 색깔을 랜덤으로 뽑기

                GameObject cubeObj = Instantiate(cubePf, newPos, Quaternion.identity);
                GameObject coordObj = Instantiate(backgroundPf, newPos, Quaternion.identity);
                coordObj.transform.parent = transform;

                Cube cube = cubeObj.GetComponent<Cube>();
                Background coord = coordObj.GetComponent<Background>();

                if ((x == 1 && y == 0) || (x == 2 && y == 1))
                {
                    cube.GetComponent<Image>().color = cubeColors[1];
                    cube.Type = 1;
                }
                else
                {
                    cube.GetComponent<Image>().color = cubeColors[0];
                    cube.Type = 0;
                }

                cube.InitCoord(x, y);
                cubeObj.transform.parent = this.transform;
                cubes[x, y] = cubeObj;
                backgrounds[x, y] = coord;

                // 맨 위에 스포너 설치
                if (y == (amountY - 1))
                {
                    Vector2 spawnerPos = new Vector2(newPos.x, newPos.y + IntervalY);
                    GameObject spawner = Instantiate(spawnerPf, spawnerPos, Quaternion.identity);
                    spawners[x] = spawner;
                }
            }
        }
    }

    public void EraseRendering()
    {
        for (int x = 0; x < amountX; x++)
        {
            for (int y = 0; y < amountY; y++)
            {
                Destroy(cubes[x, y]);
                cubes[x, y] = null;
            }

            Destroy(spawnerPf);
            spawners[x] = null;
        }

        cubes = null;
        backgrounds = null;
        spawnerPf = null;
    }
    #endregion

    #region NullCount
    public void CheckCount()
    {
        for (int x = 0; x < amountX; x++)
        {
            for (int y = 0; y < amountY; y++)
            {
                Debug.Log(cubes[x, y]);
            }
        }
    }
    #endregion

    #region ReFill

    private Vector2 changePos;
    public Queue<Background> NewFillCubeColQueue = new Queue<Background>();       // 큐브를 정리한 후 생긴 빈공간의 좌표를 담을 큐

    // 빈공간을 정렬하는 메서드
    public void ReFillCubeCol(int col, int emptyCount)
    {
        int endNull = 0;

        // 빈공간을 체크하는 logic (첫번째 null 만 체크한다)
        for (int x = 0; x < amountX; x++)
        {
            for (int y = 0; y < amountY; y++)
            {
                if (x == col)
                {
                    if (cubes[x, y] == null)
                    {
                        changePos = backgrounds[x, y].transform.position;
                        endNull = y + 3;
                        break;
                    }
                }
            }
        }

        // 빈공간으로 채워넣는 logic

        for (int x = 0; x < amountX; x++)
        {
            for (int y = endNull; y < amountY; y++)
            {
                if (x == col)
                {
                    if (cubes[x, y] != null)
                    {
                        cubes[x, y].GetComponent<Cube>().InitCoord(x, y - emptyCount);      // 새로 바뀔좌표로 리셋
                        cubes[x, y - emptyCount] = cubes[x, y];
                        cubes[x, y].GetComponent<Cube>().SetPosition(changePos);
                        cubes[x, y] = null;
                    }
                }

                changePos = new Vector3(changePos.x, changePos.y += IntervalY);             // 타겟좌표를 위로 한칸씩 이동시킨다
            }
        }

        NewFillCubeCol(col);
        CheckCount();
    }

    // 새로운 큐브를 생성하는 메서드
    public void NewFillCubeCol(int col)
    {
        int nullCount = 0;

        for (int x = 0; x < amountX; x++)
        {
            for (int y = 0; y < amountY; y++)
            {
                if (cubes[x, y] == null)
                {
                    ++nullCount;
                    NewFillCubeColQueue.Enqueue(backgrounds[x, y]);
                }
            }
        }

        StartCoroutine(DelaySpawn(col, nullCount));
    }

    // 새로운 큐브를 생성하는 코루틴
    private IEnumerator DelaySpawn(int col, int nullCount)
    {
        for (int i = 0; i < nullCount; i++)
        {
            yield return new WaitForSeconds(0.1f);                                                              // 겹치지 않게 딜레이를 줌.
            int ColorIndex = UnityEngine.Random.Range(0, cubeColors.Length);                                    // 색깔을 랜덤으로 뽑기
            GameObject cubeObj = Instantiate(cubePf, spawners[col].transform.position, Quaternion.identity);    // 새로운 큐브 생성
            cubeObj.transform.parent = transform;                                                               // 오브젝트 부모 지정

            Cube cube = cubeObj.GetComponent<Cube>();
            Background background = NewFillCubeColQueue.Dequeue();
            cube.SetPosition(background.transform.position);                                                    // 새롭게 생긴 빈공간으로 큐브 넣기
            cube.InitCoord(col, background.Y);                                                                  // 백그라운드 필드좌표와 동일한 좌표 설정
            cubes[col, background.Y] = cubeObj;                                                                 // 백그라운드 필드좌표와 동일한 좌표 설정
            cube.SetColor(cubeColors[ColorIndex], ColorIndex);                                                  // 색깔 바꿔주기
        }

        StartCoroutine(DelayReconfirm());
    }

    private IEnumerator DelayReconfirm()
    {
        yield return new WaitForSeconds(0.5f);
        onReconfirmEvent?.Invoke();
    }

    #endregion

    #region Swap
    public void SwapObj(Cube firstCube, Cube SecondCube)
    {
        GameObject tempCube = cubes[firstCube.X, firstCube.Y];
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