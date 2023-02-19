using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Munchkin : Candy
{
    [SerializeField]
    private float MoveSpeed;

    private int score = 0;

    private void OnEnable()
    {
        score = GameManager.Instance.blockDestructionScore;
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (false == board.OnMoveAble)
        {
            return;
        }

        base.OnEndDrag(eventData);
        StartCoroutine(updateMove(moveDir));
    }

    private IEnumerator updateMove(Vector2 moveDir)
    {
        board.marker[X, Y] = true;

        while (true)
        {
            yield return new WaitForSeconds(0.01f);
            transform.Translate(moveDir * Time.deltaTime * MoveSpeed);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == 6)
        {
            collision.gameObject.SetActive(false);
            Candy candy = collision.gameObject.GetComponent<Candy>();
            board.marker[candy.X, candy.Y] = true;
            GameManager.Instance.SetScore(score);
        }

        else if (collision.gameObject.layer == 8)
        {
            Destroy(gameObject);
            GameManager.Instance.SetMunchkinCount();
            board.ArrangeCandies();
        }
    }
}
