using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Cube : MonoBehaviour, IPointerClickHandler
{
    public int X;              // 2�� �迭�� x�� �ε���
    public int Y;              // 2�� �迭�� y�� �ε���
    public int Type;           // ť�� ����
    private Board board;        // �θ� Ŭ����

    [SerializeField]
    private Image image;

    private void Start()
    {
        board = GetComponentInParent<Board>();
    }

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

    public void OnPointerClick(PointerEventData eventData)
    {
        // ù��° Ÿ���� Ŭ������ ��
        if (false == board.IsPickCube)
        {
            board.currnetPickCube = gameObject;                                 // ó�� Ÿ���� Ŭ������ ��
            board.IsPickCube = true;
        }

        // �ι�° Ÿ���� Ŭ������ ��
        else
        {
            Cube firstCube = board.currnetPickCube.GetComponent<Cube>();        // Ÿ�� Ÿ�� �Ҵ�

            if ((Mathf.Abs(firstCube.X - X) == 1 && Mathf.Abs(firstCube.Y - Y) == 0) ||
                 (Mathf.Abs(firstCube.X - X) == 0 && Mathf.Abs(firstCube.Y - Y) == 1))
            {
                board.SwapObj(firstCube, this);
                board.SwapPos(ref firstCube.X, ref firstCube.Y, ref X, ref Y);

                Board.onSwapEvent?.Invoke();
            }


            board.IsPickCube = false;       // �ٲ�����̹Ƿ� Ÿ���� ����ִ� ���°� �ƴϴ�.
        }
    }

    public void SetPosition(Vector2 targetPos)
    {
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

    }
}