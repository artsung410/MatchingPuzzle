using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NormalCandy : Candy, IPointerClickHandler
{
    //public override void OnEndDrag(PointerEventData eventData)
    //{
    //    if (false == board.OnMoveAble)
    //    {
    //        return;
    //    }

    //    base.OnEndDrag(eventData);
    //    swap();
    //}

    //private void swap()
    //{
    //    if (targetCandy == null)
    //    {
    //        return;
    //    }

    //    board.SwapObj(this, targetCandy);
    //    board.SwapPos(ref X, ref Y, ref targetCandy.X, ref targetCandy.Y);
    //    GameManager.Instance.onSwapEvent?.Invoke();

    //}


    #region MouseClick
    public void OnPointerClick(PointerEventData eventData)
    {
        if (false == board.OnMoveAble)
        {
            return;
        }

        // 첫번째 타일을 클릭했을 때
        if (false == board.IsPickCandy)
        {
            board.FirstPickCandy = this;                                 
            board.IsPickCandy = true;
        }

        // 두번째 타일을 클릭했을 때
        else
        {
            Candy firstCandy = board.FirstPickCandy.GetComponent<Candy>();       

            if ((Mathf.Abs(firstCandy.X - X) == 1 && Mathf.Abs(firstCandy.Y - Y) == 0) ||
                 (Mathf.Abs(firstCandy.X - X) == 0 && Mathf.Abs(firstCandy.Y - Y) == 1))
            {
                board.SecondPickCandy = board.candies[X, Y];
                board.SwapObj(firstCandy, this);
                board.SwapPos(ref firstCandy.X, ref firstCandy.Y, ref X, ref Y);
                GameManager.Instance.onSwapEvent?.Invoke();
            }
            // 바뀐상태이므로 타일을 쥐고있는 상태가 아니다.
            board.IsPickCandy = false;       
        }
    }
    #endregion
}
