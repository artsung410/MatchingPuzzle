using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Cube : MonoBehaviour, IPointerClickHandler
{
    public int X;              // 2차 배열의 x축 인덱스
    public int Y;              // 2차 배열의 y축 인덱스
    public int Type;           // 큐브 종류
    private Board board;        // 부모 클래스

    [SerializeField]
    private Image image;

    private void Start()
    {
        board = GetComponentInParent<Board>();
    }

    public void InitCoord(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void SetColor(Color color, int type)
    {
        image.color = color;
        Type = type;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 첫번째 타일을 클릭했을 때
        if (false == board.IsPickCube)
        {
            board.currnetPickCube = gameObject;                                 // 처음 타일을 클릭했을 때
            board.IsPickCube = true;
        }

        // 두번째 타일을 클릭했을 때
        else
        {
            Cube firstCube = board.currnetPickCube.GetComponent<Cube>();        // 타겟 타일 할당

            if ((Mathf.Abs(firstCube.X - X) == 1 && Mathf.Abs(firstCube.Y - Y) == 0) ||
                 (Mathf.Abs(firstCube.X - X) == 0 && Mathf.Abs(firstCube.Y - Y) == 1))
            {
                board.SwapObj(firstCube, this);
                board.SwapPos(ref firstCube.X, ref firstCube.Y, ref X, ref Y);

                Board.onSwapEvent?.Invoke();
            }


            board.IsPickCube = false;       // 바뀐상태이므로 타일을 쥐고있는 상태가 아니다.
        }
    }

    public void SetPosition(Vector2 targetPos)
    {
        StartCoroutine(SetPositioning(targetPos));
    }

    private IEnumerator SetPositioning(Vector2 targetPos)
    {
        float distance = transform.position.y - targetPos.y;

        for (int i = 0; i < 20; i++)
        {
            yield return new WaitForSeconds(0.01f);
            transform.position = new Vector2(transform.position.x, transform.position.y - (distance / 20));
        }

    }
}