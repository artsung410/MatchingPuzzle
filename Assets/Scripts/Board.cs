using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Board : MonoBehaviour
{
    public static Action onSwapEvent;

    public int amountX; // ���� ����
    public int amountY; // ���� ����
    public int InitPosX;
    public int InitPosY;
    public int IntervalX;
    public int IntervalY;

    public GameObject cubePf;   // Ÿ�� ������
    public GameObject backgroundPf;  // ������ ������ (��ǥ��)
    public GameObject spawnerPf; // ������ ������

    public Color[] cubeColors; // Ÿ���� ���� ����
    public GameObject[,] cubes; // ��� Ÿ���� 2���� �迭�� �ֱ�
    public GameObject[] spawners;  // ���ο� ������ 1ó�� �迭�� �ֱ�
    public Background[,] backgrounds; // ��ǥ ���밴ü 2���� �迭�� �ֱ�

    public bool IsPickCube = false; // Ÿ���� ����ִ��� �Ǻ�
    public GameObject currnetPickCube; // ���� ����ִ� Ÿ��
    public PuzzleMatcher puzzleMatcher;  // üũ�ϰ� �ִ� ��

    private void Awake()
    {
        puzzleMatcher = GetComponent<PuzzleMatcher>();
    }

    void Start()
    {
        cubes = new GameObject[amountX, amountY];
        spawners = new GameObject[amountX];
        backgrounds = new Background[amountX, amountY];
        Init();
    }

    // ���� ����

    #region MakePuzzle
    private void Init()
    {
        for (int x = 0; x < amountX; x++)
        {
            for (int y = 0; y < amountY; y++)
            {
                Vector2 newPos = new Vector2(InitPosX + x * IntervalX, InitPosY + y * IntervalY);
                //int ColorIndex = Random.Range(0, cubeColors.Length);   // ������ �������� �̱�

                GameObject cubeObj = Instantiate(cubePf, newPos, Quaternion.identity);
                GameObject coordObj = Instantiate(backgroundPf, newPos, Quaternion.identity);
                coordObj.transform.parent = transform;

                Cube cube = cubeObj.GetComponent<Cube>();
                Background coord = coordObj.GetComponent<Background>();

                if (y == 2 || y == 5)
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
                cube.gameObject.name = $"({x}, {y})";
                cubeObj.transform.parent = transform;
                cubes[x, y] = cubeObj;

                coord.InitCoord(x, y);
                backgrounds[x, y] = coord;

                // �� ���� ������ ��ġ
                if (y == (amountY - 1))
                {
                    Vector2 spawnerPos = new Vector2(newPos.x, newPos.y + IntervalY);
                    GameObject spawner = Instantiate(spawnerPf, spawnerPos, Quaternion.identity);
                    spawners[x] = spawner;
                }
            }
        }
    }

    // ���� 5��   
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

                // �� ���� ������ ��ġ
                if (y == (amountY - 1))
                {
                    Vector2 spawnerPos = new Vector2(newPos.x, newPos.y + IntervalY);
                    GameObject spawner = Instantiate(spawnerPf, spawnerPos, Quaternion.identity);
                    spawners[x] = spawner;
                }
            }
        }
    }

    // ���� 5��
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

                // �� ���� ������ ��ġ
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
                //int ColorIndex = Random.Range(0, cubeColors.Length);   // ������ �������� �̱�

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

                // �� ���� ������ ��ġ
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

    // ��⿭�� ��ǥ�� �ӽ÷� ��Ƶд�.
    public Queue<Background> ReFillCubeColQueue = new Queue<Background>();        // �������� ���� ������� ��ǥ�� ���� ť
    public Queue<Background> NewFillCubeColQueue = new Queue<Background>();       // ť�긦 ������ �� ���� ������� ��ǥ�� ���� ť

    // ������� �����ϴ� �޼���
    public void ReFillCubeCol(int col, int emptyCount)
    {
        int endNull = 0;
        for (int x = 0; x < amountX; x++)
        {
            for (int y = 0; y < amountY; y++)
            {
                if (x == col)
                {
                    if (cubes[x, y] == null)
                    {
                        ReFillCubeColQueue.Enqueue(backgrounds[x, y]);
                        //cubes[x, y].GetComponent<Cube>().InitCoord(x, y - emptyCount);      // ���� �ٲ���ǥ�� ����
                        //cubes[x, y - emptyCount] = cubes[x, y];
                        //cubes[x, y].GetComponent<Cube>().SetPosition(ReFillCubeColQueue.Dequeue());
                        //cubes[x, y] = null;

                        if (ReFillCubeColQueue.Count == 3)
                        {
                            endNull = y;
                        }
                    }
                }
            }
        }

        for (int x = 0; x < amountX; x++)
        {
            for (int y = endNull; y < amountY; y++)
            {
                if (x == col)
                {
                    if (cubes[x, y] != null)
                    {
                        cubes[x, y].GetComponent<Cube>().InitCoord(x, y - emptyCount);      // ���� �ٲ���ǥ�� ����
                        cubes[x, y - emptyCount] = cubes[x, y];
                        cubes[x, y].GetComponent<Cube>().SetPosition(ReFillCubeColQueue.Dequeue().transform.position);
                        cubes[x, y] = null;
                    }
                }
            }
        }

        NewFillCubeCol(col);
        CheckCount();
        ReFillCubeColQueue.Clear();
    }

    // ���ο� ť�긦 �����ϴ� �޼���
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

    // ���ο� ť�긦 �����ϴ� �ڷ�ƾ
    private IEnumerator DelaySpawn(int col, int nullCount)
    {
        for (int i = 0; i < nullCount; i++)
        {
            yield return new WaitForSeconds(0.1f);
            int ColorIndex = UnityEngine.Random.Range(0, cubeColors.Length);                                    // ������ �������� �̱�
            GameObject cubeObj = Instantiate(cubePf, spawners[col].transform.position, Quaternion.identity);    // ���ο� ť�� ����
            cubeObj.transform.parent = transform;                                                               // ������Ʈ �θ� ����

            Cube cube = cubeObj.GetComponent<Cube>();
            Background background = NewFillCubeColQueue.Dequeue();
            cube.SetPosition(background.transform.position);                                                    // ���Ӱ� ���� ��������� ť�� �ֱ�
            cube.InitCoord(col, background.Y);                                                                  // ��׶��� �ʵ���ǥ�� ������ ��ǥ ����
            cubes[col, background.Y] = cubeObj;                                                                 // ��׶��� �ʵ���ǥ�� ������ ��ǥ ����
            cube.SetColor(cubeColors[ColorIndex], ColorIndex);                                                  // ���� �ٲ��ֱ�
        }
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