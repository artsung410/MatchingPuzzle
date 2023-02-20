using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Candy : MonoBehaviour, IEndDragHandler, IDragHandler
{
    [HideInInspector]
    public int X;              // 2차 배열의 x축 인덱스

    [HideInInspector]
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
            return Vector2.up;
        }
        else if (angle > -135 && angle < -45)
        {
            return Vector2.down;
        }
        else if (angle > -45 && angle < 45)
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