using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    public int amountX; // 가로 수량
    public int amountY; // 세로 수량
    public int InitPosX; 
    public int InitPosY;
    public int IntervalX;
    public int IntervalY;

    public GameObject cubePf;   // 타일 프리팹
    public Color[] cubeColors; // 타일의 색상 종류
    public GameObject[,] cubes; // 모든 타일을 2차원 배열에 넣기

    public bool IsPickCube = false; // 타일을 쥐고있는지 판별
    public GameObject currnetPickCube; // 현재 쥐고있는 타일


    void Start()
    {
        cubes = new GameObject[amountX, amountY];
        Init();
    }

    // 맵을 그리는 과정
    private void Init()
    {
        for(int x = 0; x < amountX; x++)
        {
            for (int y = 0; y < amountY; y++)
            {
                Vector2 newPosition = new Vector2(InitPosX + x * IntervalX, InitPosY + y * IntervalY);
                int ColorIndex = Random.Range(0, cubeColors.Length);   // 색깔을 랜덤으로 뽑기

                GameObject cubeObj = Instantiate(cubePf, newPosition, Quaternion.identity);
                Cube cube = cubeObj.GetComponent<Cube>();
                cube.GetComponent<Image>().color = cubeColors[ColorIndex];
                cube.Init(x, y);
                cubeObj.transform.parent = this.transform;
                cubes[x, y] = cubeObj;
            }
        }
    }
}
