using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Munchkin : Candy, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField]
    private float MoveSpeed;
    private int score;      

    private bool isMoving;
    public bool IsMoving
    {
        get => isMoving;
        set => isMoving = value;
    }

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        score = GameManager.Instance.blockDestructionScore;
        isMoving = false;
    }

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

        isMoving = true;
        dragEndPos = CalculateMousePosition();
        moveDir = CalculateDir();
        StartCoroutine(UpdateMove(moveDir));
    }

    // 마우스 포지션 계산
    private Vector2 CalculateMousePosition()
    {
        MousePos = Input.mousePosition;
        MousePos = Camera.main.ScreenToWorldPoint(MousePos);
        return MousePos;
    }

    // 방향 계산
    private Vector2 CalculateDir()
    {
        float distanceX = dragEndPos.x - dragBeginPos.x;
        float distanceY = dragEndPos.y - dragBeginPos.y;

        // 아크탄젠트를 활용해서 각도를 구한다 (라디안 -> 각도)
        float angle = Mathf.Atan2(distanceY, distanceX) * Mathf.Rad2Deg;

        Debug.Log(angle);

        // 1~4분면을 X자로 4개의 영역을 나눠서 대각선으로 드래그해도 특정 좌표로 이동하게 만든다.
        if (angle >= 45 && angle < 135)
        {
            return Vector2.up;
        }
        else if (angle >= -135 && angle < -45)
        {
            return Vector2.down;
        }
        else if (angle >= -45 && angle < 45)
        {
            return Vector2.right;
        }
        else
        {
            return Vector2.left;
        }
    }

    // 실시간 이동
    private IEnumerator UpdateMove(Vector2 moveDir)
    {
        // 파괴시킬 캔디로 등록.
        board.Marker[X, Y] = true;
        GameManager.Instance.onButtonDisableEvent?.Invoke();
        while (true)
        {
            yield return new WaitForSeconds(0.01f);

            // 정해진 방향으로 홀에 들어갈때까지 움직인다.
            transform.Translate(moveDir * Time.deltaTime * MoveSpeed);
        }
    }

    #endregion

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 일반캔디랑 부딪혔을 때
        if(collision.gameObject.layer == 6 )
        {
            collision.gameObject.SetActive(false);
            Candy candy = collision.gameObject.GetComponent<Candy>();
            board.Marker[candy.X, candy.Y] = true;
            GameManager.Instance.SetScore(score);
        }

        // 먼치킨끼리 부딪혔을 때
        else if (collision.gameObject.layer == 7)
        {
            Munchkin Munchkin = collision.gameObject.GetComponent<Munchkin>();

            // 멈춰있는 먼치킨일때
            if (false == Munchkin.IsMoving)
            {
                collision.gameObject.SetActive(false);
                board.Marker[Munchkin.X, Munchkin.Y] = true;
                GameManager.Instance.SetScore(score);
            }

            // 움직이는 먼치킨일 때
            else
            {
                // 큼지막하게 만들어준다.
                collision.transform.localScale = new Vector3(2, 2, 2);
            }
        }

        // 먼치킨이 홀에 들어갔을 때
        else if (collision.gameObject.layer == 8)
        {
            Destroy(gameObject);
            GameManager.Instance.SetMunchkinCount();
            board.ArrangeCandies();
        }
    }

    private void OnDisable()
    {
        if (true == isMoving)
        {
            GameManager.Instance.onButtonEnableEvent?.Invoke();
        }

        StopAllCoroutines();
    }
}
