using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NormalCandy : Candy, IPointerClickHandler
{
    public override void OnEndDrag(PointerEventData eventData)
    {
        if (false == board.OnMoveAble)
        {
            return;
        }

        base.OnEndDrag(eventData);
        swap();
    }

    private void swap()
    {
        if (targetCandy == null)
        {
            return;
        }

        board.SwapObj(this, targetCandy);
        board.SwapPos(ref X, ref Y, ref targetCandy.X, ref targetCandy.Y);
        GameManager.Instance.onSwapEvent?.Invoke();

    }


    #region MouseClick
    public void OnPointerClick(PointerEventData eventData)
    {
        if (false == board.OnMoveAble)
        {
            return;
        }

        // ù��° Ÿ���� Ŭ������ ��
        if (false == board.IsPickCandy)
        {
            board.currnetPickCandy = this;                                 
            board.IsPickCandy = true;
        }

        // �ι�° Ÿ���� Ŭ������ ��
        else
        {
            Candy firstCandy = board.currnetPickCandy.GetComponent<Candy>();       

            if ((Mathf.Abs(firstCandy.X - X) == 1 && Mathf.Abs(firstCandy.Y - Y) == 0) ||
                 (Mathf.Abs(firstCandy.X - X) == 0 && Mathf.Abs(firstCandy.Y - Y) == 1))
            {
                board.SwapObj(firstCandy, this);
                board.SwapPos(ref firstCandy.X, ref firstCandy.Y, ref X, ref Y);
                GameManager.Instance.onSwapEvent?.Invoke();
            }
            // �ٲ�����̹Ƿ� Ÿ���� ����ִ� ���°� �ƴϴ�.
            board.IsPickCandy = false;       
        }
    }
    #endregion
}