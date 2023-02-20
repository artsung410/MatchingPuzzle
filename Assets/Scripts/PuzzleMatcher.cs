using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class PuzzleMatcher : MonoBehaviour
{
    private Board board;                        // 보드 정보
    private bool onPattern = false;             // 매칭 되었는지 확인
    private bool checkOneCycle = false;         // 사이클이 한번이상 돌았는지 확인

    private void Awake()
    {
        board = GetComponent<Board>();
    }

    private void OnEnable()
    {
        GameManager.Instance.onSwapEvent += CheckAllPattern;
    }


    #region AllPatternCheck

    // 전체패턴 체크
    public void CheckAllPattern()
    {
        onPattern = false;
        CheckPatternRow();
        CheckPatternColumn();
        CheckPattern_Square();

        // 매칭이 안되었을 때
        if (false == onPattern)
        {
            checkOneCycle = false;
            GameManager.Instance.onButtonEnableEvent?.Invoke();

            // 스왑한 캔디가 매칭이 안되었을 때
            if (board.PickedCandies.Count != 0)
            {

                // 캔디를 원상태로 돌려준다.
                board.PickedCandies[0].SetPrevPos(board.PickedCandies[0], board.PickedCandies[1]);

                // 스왑한 캔디정보를 제거 해준다.
                board.PickedCandies.Clear();
            }

            // 먼치킨의 개수가 0이면 게임클리어를 실행한다.
            if (GameManager.Instance.MunchkinCount == 0)
            {
                GameManager.Instance.onButtonDisableEvent?.Invoke();
                StartCoroutine(DelayGameClear());
            }
            return;
        }

        // 매칭이 되었을 때

        // 스왑한 캔디정보를 제거 해준다.
        board.PickedCandies.Clear();

        // 퍼즐 정리함수를 실행시킨다.
        board.ArrangeCandies();

        // 사이클이 전부 돌았으면 true로 만든다.
        checkOneCycle = true;
    }

    // 패즐 매칭이 끝나고 일정시간 딜레이를 주고 게임클리어한다.
    private IEnumerator DelayGameClear()
    {
        yield return new WaitForSeconds(1f);
        GameManager.Instance.ActivationClearPanel();
    }

    #endregion


    #region RowPatternCheck

    // 가로패턴 체크
    private void CheckPatternRow()
    {
        int checkCount = 3;

        for (int x = 0; x < board.CandyCountX; x++)
        {
            for (int y = 0; y < board.CandyCountY; y++)
            {
                // 범위를 벗어나면 다음좌표로 건너뛴다.
                if (x > board.CandyCountX - checkCount)
                {
                    continue;
                }

                Candy firstCube = board.Candies[x, y];
                Candy secondCube = board.Candies[x + 1, y];
                Candy thirdCube = board.Candies[x + 2, y];

                if (firstCube.Type == secondCube.Type && firstCube.Type == thirdCube.Type)
                {
                    onPattern = true;
                    board.Marker[x, y] = true;
                    board.Marker[x + 1, y] = true;
                    board.Marker[x + 2, y] = true;
                }
            }
        }
    }
    #endregion


    #region ColumnPatternCheck

    // 세로패턴 체크
    private void CheckPatternColumn()
    {
        int checkCount = 3;

        for (int x = 0; x < board.CandyCountX; x++)
        {
            for (int y = 0; y < board.CandyCountY; y++)
            {
                // 범위를 벗어나면 다음좌표로 건너뛴다.
                if (y > board.CandyCountY - checkCount)
                {
                    continue;
                }

                Candy firstCube = board.Candies[x, y];
                Candy secondCube = board.Candies[x, y + 1];
                Candy thirdCube = board.Candies[x, y + 2];

                if (firstCube.Type == secondCube.Type && firstCube.Type == thirdCube.Type)
                {
                    onPattern = true;
                    board.Marker[x, y] = true;
                    board.Marker[x, y + 1] = true;
                    board.Marker[x, y + 2] = true;
                }
            }
        }
    }

    #endregion


    #region SquarePatternCheck
    // 정사각형패턴 체크
    private void CheckPattern_Square()
    {
        int checkCount = 2;

        for (int x = 0; x < board.CandyCountX; x++)
        {
            for (int y = 0; y < board.CandyCountY; y++)
            {
                // 범위를 벗어나면 다음좌표로 건너뛴다.
                if (x > board.CandyCountX - checkCount || y > board.CandyCountY - checkCount)
                {
                    continue;
                }

                Candy firstCube = board.Candies[x, y];
                Candy secondCube = board.Candies[x + 1, y];
                Candy thirdCube = board.Candies[x, y + 1];
                Candy fourthCube = board.Candies[x + 1, y + 1];

                if (firstCube.Type == secondCube.Type && firstCube.Type == thirdCube.Type && firstCube.Type == fourthCube.Type)
                {
                    onPattern = true;

                    // 처음 스왑했을 경우
                    if (false == checkOneCycle)
                    {
                        // 최종 스왑한 좌표로 먼치킨을 생성시켜준다.
                        for (int i = 0; i < checkCount; i++)
                        {
                            for (int j = 0; j < checkCount; j++)
                            {
                                if (x > board.CandyCountX - checkCount || y > board.CandyCountY - checkCount || board.PickedCandies.Count != 2)
                                {
                                    continue;
                                }

                                if (board.Candies[x + i, y + j] != board.PickedCandies[0] && board.Candies[x + i, y + j] != board.PickedCandies[1])
                                {
                                    board.Marker[x + i, y + j] = true;
                                }
                                else if (board.Candies[x + i, y + j] == board.PickedCandies[0] || board.Candies[x + i, y + j] == board.PickedCandies[1])
                                {
                                    board.Marker[x + i, y + j] = false;
                                    board.CreateBall(x + i, y + j);
                                }
                            }
                        }
                    }

                    // 사이클이 한번이상 돌았을 경우
                    else
                    {
                        board.Marker[x, y] = true;
                        board.Marker[x, y + 1] = true;
                        board.Marker[x + 1, y + 1] = true;
                        board.Marker[x + 1, y] = false;
                        board.CreateBall(x + 1, y);
                    }
                
                }
            }
        }
    }
    #endregion


    private void OnDisable()
    {
        GameManager.Instance.onSwapEvent -= CheckAllPattern;
    }
}