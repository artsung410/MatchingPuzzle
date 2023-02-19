using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Candy : MonoBehaviour, IEndDragHandler, IDragHandler
{
    [HideInInspector]
    public int X;              // 2�� �迭�� x�� �ε���

    [HideInInspector]
    public int Y;              // 2�� �迭�� y�� �ε���

    [HideInInspector]
    public int Type;           // ť�� ����

    [HideInInspector]
    public Board board;        // �θ� Ŭ����

    [SerializeField]
    private Image image;

    [HideInInspector]
    public bool onGround = false;

    public Candy targetCandy;

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
    }
    #endregion

    #region drag
    protected Vector2 dragBeginPos;
    protected Vector2 dragEndPos;
    protected Vector2 moveDir;
    protected Vector2 MousePos;

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragBeginPos = calculateMousePostion();
    }

    public void OnDrag(PointerEventData eventData)
    {
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
        float distanceX = dragEndPos.x - dragBeginPos.x;
        float distanceY = dragEndPos.y - dragBeginPos.y;

        float angle = Mathf.Atan2(distanceY, distanceX) * Mathf.Rad2Deg;

        Debug.Log(angle);
        if (angle > 45 && angle < 135)
        {
            targetCandy = Y - 1 >= 0 ? board.candies[X, Y - 1] : null;
            return Vector2.up;
        }
        else if (angle > -135 && angle < -45)
        {
            targetCandy = Y + 1 < board.candyCountY ? board.candies[X, Y + 1] : null;
            return Vector2.down;
        }
        else if (angle > -45 && angle < 45)
        {
            targetCandy = X + 1 < board.candyCountX ? board.candies[X + 1, Y] : null;
            return Vector2.right;
        }
        else
        {
            targetCandy = X - 1 >=0 ? board.candies[X - 1, Y] : null;
            return Vector2.left;
        }
    }

    #endregion



}