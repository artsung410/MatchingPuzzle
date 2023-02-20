using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class PuzzleMatcher : MonoBehaviour
{
    // match가 성공했을 때 만들어져야 할 퍼즐들
    //private static List<Puzzle> _puzzles;
    private Board board;

    private bool onPattern = false;
    private bool checkOneCycle = false;

    private void Awake()
    {
        board = GetComponent<Board>();
    }

    private void OnEnable()
    {
        GameManager.Instance.onSwapEvent += CheckAllPattern;
    }

    public void CheckAllPattern()
    {
        onPattern = false;
        CheckPatternRow();
        CheckPatternColumn();
        CheckPattern_Square();

        if (false == onPattern)
        {
            board.OnMoveAble = true;
            checkOneCycle = false;

            if (GameManager.Instance.MunchkinCount == 0)
            {
                board.OnMoveAble = false;
                StartCoroutine(DelayGameClear());
            }
            return;
        }

        board.ArrangeCandies();
        checkOneCycle = true;
    }

    private IEnumerator DelayGameClear()
    {
        yield return new WaitForSeconds(1f);
        GameManager.Instance.ActivationClearPanel();
    }    

    public void CheckPatternRow()
    {
        int checkCount = 3;

        for (int x = 0; x < board.candyCountX; x++)
        {
            for (int y = 0; y < board.candyCountY; y++)
            {
                if (x > board.candyCountX - checkCount)
                {
                    continue;
                }

                Candy firstCube = board.candies[x, y];
                Candy secondCube = board.candies[x + 1, y];
                Candy thirdCube = board.candies[x + 2, y];

                if (firstCube.Type == secondCube.Type && firstCube.Type == thirdCube.Type)
                {
                    onPattern = true;
                    board.marker[x, y] = true;
                    board.marker[x + 1, y] = true;
                    board.marker[x + 2, y] = true;
                }
            }
        }
    }


    private void CheckPatternColumn()
    {
        int checkCount = 3;

        for (int x = 0; x < board.candyCountX; x++)
        {
            for (int y = 0; y < board.candyCountY; y++)
            {
                if (y > board.candyCountY - checkCount)
                {
                    continue;
                }

                Candy firstCube = board.candies[x, y];
                Candy secondCube = board.candies[x, y + 1];
                Candy thirdCube = board.candies[x, y + 2];

                if (firstCube.Type == secondCube.Type && firstCube.Type == thirdCube.Type)
                {
                    onPattern = true;
                    board.marker[x, y] = true;
                    board.marker[x, y + 1] = true;
                    board.marker[x, y + 2] = true;
                }
            }
        }
    }

    public void CheckPattern_Square()
    {
        int checkCount = 2;

        for (int x = 0; x < board.candyCountX; x++)
        {
            for (int y = 0; y < board.candyCountY; y++)
            {
                // y = 0 1 2 3
                if (x > board.candyCountX - checkCount)
                {
                    continue;
                }

                if (y > board.candyCountY - checkCount)
                {
                    continue;
                }

                Candy firstCube = board.candies[x, y];
                Candy secondCube = board.candies[x + 1, y];
                Candy thirdCube = board.candies[x, y + 1];
                Candy fourthCube = board.candies[x + 1, y + 1];

                if (firstCube.Type == secondCube.Type && firstCube.Type == thirdCube.Type && firstCube.Type == fourthCube.Type)
                {
                    onPattern = true;

                    if (false == checkOneCycle)
                    {
                        for (int i = 0; i < checkCount; i++)
                        {
                            for (int j = 0; j < checkCount; j++)
                            {
                                if (board.candies[x + i, y + j] != board.FirstPickCandy && board.candies[x + i, y + j] != board.SecondPickCandy)
                                {
                                    board.marker[x + i, y + j] = true;
                                }
                                else if (board.candies[x + i, y + j] == board.FirstPickCandy || board.candies[x + i, y + j] == board.SecondPickCandy)
                                {
                                    board.marker[x + i, y + j] = false;
                                    board.CreateBall(x + i, y + j);
                                }

                            }
                        }
                    }
                    else
                    {
                        board.marker[x, y] = true;
                        board.marker[x, y + 1] = true;
                        board.marker[x + 1, y + 1] = true;
                        board.marker[x + 1, y] = false;
                        board.CreateBall(x + 1, y);
                    }
                
                }
            }
        }
    }

    private void OnDisable()
    {
        GameManager.Instance.onSwapEvent -= CheckAllPattern;
    }
}