using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Candy : MonoBehaviour, IEndDragHandler, IDragHandler
{
    public Candy TargetCandy;


    public int X;              // 2차 배열의 x축 인덱스


    public int Y;              // 2차 배열의 y축 인덱스

    [HideInInspector]
    public int Type;           // 큐브 종류

    [HideInInspector]
    public Board board;        // 부모 클래스

    [SerializeField]
    private Image image;

    [HideInInspector]
    public bool onGround = false;

    private void Start()
    {
        board = GetComponentInParent<Board>();
    }

    #region SetInfo
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

    public void SetPosition(Vector2 targetPos)
    {
        onGround = false;
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

        onGround = true;
        board.OnMoveAble = true;
    }

    private Vector2 prevPos;

    public void SetPrevPos(Candy firstCandy, Candy secondCandy)
    {
        board.SwapObj(firstCandy, secondCandy);
        board.SwapPos(ref firstCandy.X, ref firstCandy.Y, ref secondCandy.X, ref secondCandy.Y);
    }

    public void SetPositionForSwap(Vector2 targetPos)
    {
        prevPos = transform.position;
        board.OnMoveAble = false;

        if (targetPos.x - transform.position.x == 0)
        {
            StartCoroutine(SetPositioning(targetPos));
        }
        else if (targetPos.y - transform.position.y == 0)
        {
            StartCoroutine(SetPositioningX(targetPos));
        }
    }

    private IEnumerator SetPositioningX(Vector2 targetPos)
    {
        float distance = transform.position.x - targetPos.x;

        for (int i = 0; i < 20; i++)
        {
            yield return new WaitForSeconds(0.01f);
            transform.position = new Vector2(transform.position.x - (distance / 20), transform.position.y);
        }

        onGround = true;
        board.OnMoveAble = true;
    }
    #endregion

    #region drag
    protected Vector2 dragBeginPos;
    protected Vector2 dragEndPos;
    protected Vector2 dragPos;
    protected Vector2 moveDir;
    protected Vector2 MousePos;

    public virtual void OnDrag(PointerEventData eventData)
    {
        Debug.Log("드래깅 중");
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        dragEndPos = calculateMousePostion();
        moveDir = calculateDir();
    }


    protected Vector2 calculateMousePostion()
    {
        MousePos = Input.mousePosition;
        MousePos = Camera.main.ScreenToWorldPoint(MousePos);
        return MousePos;
    }

    protected Vector2 calculateDir()
    {
        float distanceX = dragEndPos.x;
        float distanceY = dragEndPos.y;

        float angle = Mathf.Atan2(distanceY, distanceX) * Mathf.Rad2Deg;

        Debug.Log(angle);
        if (angle >= 45 && angle < 135)
        {
            return Vector2.up;
        }
        else if (angle >= -135 && angle < -45)
        {
            return Vector2.down;
        }
        else if (angle >= -45 && angle < 45)
        {
            return Vector2.right;
        }
        else
        {
            return Vector2.left;
        }
    }

    #endregion

}