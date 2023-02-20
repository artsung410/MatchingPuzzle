using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NormalCandy : Candy
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
    //    if (board.SecondPickCandy == null)
    //    {
    //        return;
    //    }

    //    board.SwapObj(this, board.SecondPickCandy);
    //    board.SwapPos(ref X, ref Y, ref board.SecondPickCandy.X, ref board.SecondPickCandy.Y);
    //    GameManager.Instance.onSwapEvent?.Invoke();

    //}


    #region MouseClick

    public void ClickCandy()
    {
        if (false == board.OnMoveAble)
        {
            return;
        }

        if (board.PickedCandies.Count != 0)
        {
            if (board.PickedCandies[board.PickedCandies.Count - 1] == this)
            {
                return;
            }
        }

        board.PickCandies(this);

        if (board.PickedCandies.Count == 2)
        {
            if ((Mathf.Abs(board.PickedCandies[0].X - board.PickedCandies[1].X) == 1 && Mathf.Abs(board.PickedCandies[0].Y - board.PickedCandies[1].Y) == 0) ||
                 (Mathf.Abs(board.PickedCandies[0].X - board.PickedCandies[1].X) == 0 && Mathf.Abs(board.PickedCandies[0].Y - board.PickedCandies[1].Y) == 1))
            {
                board.SwapObj(board.PickedCandies[0], board.PickedCandies[1]);
                board.SwapPos(ref board.PickedCandies[0].X, ref board.PickedCandies[0].Y, ref board.PickedCandies[1].X, ref board.PickedCandies[1].Y);
                StartCoroutine(DelayInvoke());
            }
            else
            {
                board.PickedCandies.Clear();
            }
        }
    }

    private IEnumerator DelayInvoke()
    {
        yield return new WaitForSeconds(0.5f);
        GameManager.Instance.onSwapEvent?.Invoke();
    }
    #endregion
}
