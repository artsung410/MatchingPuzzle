using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class Cube : MonoBehaviour, IPointerClickHandler
{
    public int X;              // 2차 배열의 x축 인덱스
    public int Y;              // 2차 배열의 y축 인덱스

    private Board board;                     // 부모 클래스

    public TextMeshProUGUI Text;

    private void Start()
    {
        board = GetComponentInParent<Board>();
    }

    public void Init(int x, int y)
    {
        X = x;
        Y = y;
        Text.text = $"({x}, {y})";
    }

    public void SetText(int x, int y)
    {
        Text.text = $"({x}, {y})";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 첫번째 타일을 클릭했을 때
        if (false == board.IsPickCube)
        {
            board.currnetPickCube = gameObject;                               // 처음 타일을 클릭했을 때
            board.IsPickCube = true;
        }

        // 두번째 타일을 클릭했을 때
        else
        {
            Cube firstCube = board.currnetPickCube.GetComponent<Cube>();        // 타겟 타일 할당

            if ( (Mathf.Abs(firstCube.X - X) == 1 && Mathf.Abs(firstCube.Y - Y) == 0) || 
                 (Mathf.Abs(firstCube.X - X) == 0 && Mathf.Abs(firstCube.Y - Y) == 1))
            {
                SwapObj(firstCube, this);
                SwapPos(ref firstCube.X, ref firstCube.Y, ref X, ref Y);

            }

            board.IsPickCube = false;       // 바뀐상태이므로 타일을 쥐고있는 상태가 아니다.
            Text.text = $"({X}, {Y})";

        }
    }

    public void SwapObj(Cube firstCube, Cube SecondCube)
    {
        firstCube.SetText(SecondCube.X, SecondCube.Y);

        GameObject tempCube = board.cubes[firstCube.X, firstCube.Y];
        board.cubes[firstCube.X, firstCube.Y] = board.cubes[SecondCube.X, SecondCube.Y];
        board.cubes[SecondCube.X, SecondCube.Y] = tempCube;

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
}
