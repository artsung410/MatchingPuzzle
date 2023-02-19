using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class Ball : Candy, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private Vector2 dragBeginPos;
    private Vector2 dragEndPos;
    private Vector2 moveDir;

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragBeginPos = calculateMousePostion();
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragEndPos = calculateMousePostion();
        moveDir = calculateDir();
        StartCoroutine(updateMove(moveDir));
    }

    private Vector2 MousePos;
    private Vector2 calculateMousePostion()
    {
        MousePos = Input.mousePosition;
        MousePos = Camera.main.ScreenToWorldPoint(MousePos);
        return MousePos;
    }

    private Vector2 calculateDir()
    {
        float distanceX = dragEndPos.x - dragBeginPos.x;
        float distanceY = dragEndPos.y - dragBeginPos.y;

        float angle = Mathf.Atan2(distanceY, distanceX) * Mathf.Rad2Deg;
       

        if (angle > 45 && angle < 135)
        {
            return Vector2.up;
        }
        else if(angle > -135 && angle < -45)
        {
            return Vector2.down;
        }
        else if (angle > -45 && angle < 45)
        {
            return Vector2.left;
        }
        else
        {
            return Vector2.right;
        }
    }

    private IEnumerator updateMove(Vector2 moveDir)
    {
        while(true)
        {
            yield return new WaitForSeconds(0.01f);
            transform.Translate(moveDir * Time.deltaTime * 100f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == 6)
        {
            Destroy(collision.gameObject);
        }
    }
}
