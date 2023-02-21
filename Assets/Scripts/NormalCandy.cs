using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NormalCandy : Candy, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    #region drag

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragBeginPos = CalculateMousePosition();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("드래깅 중");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (false == IsDraggable)
        {
            return;
        }

        targetCandy = CalculateTargetDir();

        if (targetCandy == null)
        {
            return;
        }

        board.PickedCandy = this;

        if ((Mathf.Abs(X - targetCandy.X) == 1 && Mathf.Abs(Y - targetCandy.Y) == 0) || (Mathf.Abs(X - targetCandy.X) == 0 && Mathf.Abs(Y - targetCandy.Y) == 1))
        {
            board.SwapObj(this, targetCandy);
            board.SwapPos(ref X, ref Y, ref targetCandy.X, ref targetCandy.Y);

            Vector2 TempPos = transform.position;
            SetPositionForSwap(targetCandy.transform.position);
            targetCandy.SetPositionForSwap(TempPos);
        }
        else
        {
            targetCandy = null;
        }

        StartCoroutine(ReCycle());
    }

    private IEnumerator ReCycle()
    {
        yield return new WaitForSeconds(0.5f);
        puzzleMatcher.CheckAllPattern();
    }

    // 마우스 포지션 계산
    private Vector2 CalculateMousePosition()
    {
        MousePos = Input.mousePosition;
        MousePos = Camera.main.ScreenToWorldPoint(MousePos);
        return MousePos;
    }

    // 방향 계산
    private NormalCandy CalculateTargetDir()
    {
        targetCandy = null;
        dragEndPos = CalculateMousePosition();

        float distanceX = dragEndPos.x - dragBeginPos.x;
        float distanceY = dragEndPos.y - dragBeginPos.y;

        // 아크탄젠트를 활용해서 각도를 구한다 (라디안 -> 각도)
        float angle = Mathf.Atan2(distanceY, distanceX) * Mathf.Rad2Deg;

        Debug.Log(angle);

        Candy tempCandy;
        // 1~4분면을 X자로 4개의 영역을 나눠서 대각선으로 드래그해도 특정 좌표로 이동하게 만든다.
        if (angle >= 45 && angle < 135)
        {
            tempCandy = Y - 1 >= 0 ? board.Candies[X, Y - 1] : null;
            return (NormalCandy)tempCandy;
        }
        else if (angle >= -135 && angle < -45)
        {
            tempCandy = Y + 1 < board.CandyCountY ? board.Candies[X , Y + 1] : null;
            return (NormalCandy)tempCandy;
        }
        else if (angle >= -45 && angle < 45)
        {
            tempCandy = X + 1 < board.CandyCountX ? board.Candies[X + 1, Y] : null;
            return (NormalCandy)tempCandy;
        }
        else
        {
            tempCandy = X - 1 >= 0 ? board.Candies[X - 1, Y] : null;
            return (NormalCandy)tempCandy;
        }
    }

    #endregion
}
