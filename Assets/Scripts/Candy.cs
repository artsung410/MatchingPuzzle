using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Candy : MonoBehaviour
{
    [HideInInspector] public int X;                         // 2차 배열의 x축 인덱스
    [HideInInspector] public int Y;                         // 2차 배열의 y축 인덱스
    [HideInInspector] public int Type;                      // 캔디 종류
    [HideInInspector] public bool OnGround;                 // 정지 
    [HideInInspector] public bool IsDraggable;              // 드래그 
    [HideInInspector] public Board board;                   // 부모 클래스
    [SerializeField] private Image image;                   // 캔디 이미지
    [SerializeField] private Button button;                 // 캔디 버튼(스왑)

    private void OnEnable()
    {
        OnGround = false;
        IsDraggable = true;
        GameManager.Instance.onButtonEnableEvent += EnableButton;
        GameManager.Instance.onButtonDisableEvent += DisableButton;
    }

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
    #endregion


    #region SetPosition_CreateCandy
    public void SetPosition(Vector2 targetPos)
    {
        OnGround = false;
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

        OnGround = true;
    }
    #endregion


    #region SetPosition_Swap
    private Vector2 prevPos;

    public void SetPrevPos(Candy firstCandy, Candy secondCandy)
    {
        board.SwapObj(firstCandy, secondCandy);
        board.SwapPos(ref firstCandy.X, ref firstCandy.Y, ref secondCandy.X, ref secondCandy.Y);
    }

    public void SetPositionForSwap(Vector2 targetPos)
    {
        prevPos = transform.position;

        if (targetPos.x - transform.position.x == 0)
        {
            StartCoroutine(SetPositioningY(targetPos));
        }
        else if (targetPos.y - transform.position.y == 0)
        {
            StartCoroutine(SetPositioningX(targetPos));
        }
    }

    private IEnumerator SetPositioningY(Vector2 targetPos)
    {
        float distance = transform.position.y - targetPos.y;
        GameManager.Instance.onButtonDisableEvent?.Invoke();

        for (int i = 0; i < 20; i++)
        {
            yield return new WaitForSeconds(0.01f);
            transform.position = new Vector2(transform.position.x, transform.position.y - (distance / 20));
        }

        OnGround = true;
        GameManager.Instance.onButtonEnableEvent?.Invoke();
    }

    private IEnumerator SetPositioningX(Vector2 targetPos)
    {
        GameManager.Instance.onButtonDisableEvent?.Invoke();
        float distance = transform.position.x - targetPos.x;

        for (int i = 0; i < 20; i++)
        {
            yield return new WaitForSeconds(0.01f);
            transform.position = new Vector2(transform.position.x - (distance / 20), transform.position.y);
        }

        OnGround = true;
        GameManager.Instance.onButtonEnableEvent?.Invoke();
    }
    #endregion


    #region Button
    // 버튼 비활성화
    public void DisableButton()
    {
        IsDraggable = false;

        if (button == null)
        {
            return;
        }

        button.interactable = false;
    }

    // 버튼 활성화
    public void EnableButton()
    {
        IsDraggable = true;

        if (button == null)
        {
            return;
        }

        button.interactable = true;
    }

    #endregion

    private void OnDisable()
    {
        GameManager.Instance.onButtonEnableEvent -= EnableButton;
        GameManager.Instance.onButtonDisableEvent -= DisableButton;
    }
}