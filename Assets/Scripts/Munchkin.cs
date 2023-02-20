using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Munchkin : Candy
{
    [SerializeField]
    private float MoveSpeed;

    private int score = 0;

    private bool isMoving = false;
    public bool IsMoving
    {
        get => isMoving;
        set => isMoving = value;
    }

    private void Awake()
    {
        score = GameManager.Instance.blockDestructionScore;
        isMoving = false;
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (false == board.OnMoveAble)
        {
            return;
        }

        isMoving = true;
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
        // �Ϲ�ĵ��� �ε����� ��
        if(collision.gameObject.layer == 6 )
        {
            collision.gameObject.SetActive(false);
            Candy candy = collision.gameObject.GetComponent<Candy>();
            board.marker[candy.X, candy.Y] = true;
            GameManager.Instance.SetScore(score);
        }

        // ��ġŲ���� �ε����� ��
        else if (collision.gameObject.layer == 7)
        {
            Munchkin Munchkin = collision.gameObject.GetComponent<Munchkin>();

            // �����ִ� ��ġŲ�϶�
            if (false == Munchkin.IsMoving)
            {
                collision.gameObject.SetActive(false);
                board.marker[Munchkin.X, Munchkin.Y] = true;
                GameManager.Instance.SetScore(score);
            }

            // �����̴� ��ġŲ�� ��
            else
            {
                collision.transform.localScale = new Vector3(8, 8, 8);
            }
        }

        // ��ġŲ�� Ȧ�� ���� ��
        else if (collision.gameObject.layer == 8)
        {
            Destroy(gameObject);
            GameManager.Instance.SetMunchkinCount();
            board.ArrangeCandies();
        }
    }
}
