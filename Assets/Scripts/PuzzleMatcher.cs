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

    private void Awake()
    {
        board = GetComponent<Board>();
    }

    private void OnEnable()
    {
        Board.onSwapEvent += CheckAllPattern;
    }

    public void CheckAllPattern()
    {
        onPattern = false;

        CheckPatternRow();
        CheckPatternColumn();

        if (false == onPattern)
        {
            return;
        }

        board.ArrangeCubes();
    }

    public void CheckPatternRow()
    {
        int checkCount = 3;

        for (int x = 0; x < board.cubeCountX; x++)
        {
            for (int y = 0; y < board.cubeCountY; y++)
            {
                if (x > board.cubeCountX - checkCount || board.cubes[x, y] == null || board.cubes[x + 1, y] == null || board.cubes[x + 2, y] == null)
                {
                    continue;
                }

                Cube firstCube = board.cubes[x, y].GetComponent<Cube>();
                Cube secondCube = board.cubes[x + 1, y].GetComponent<Cube>();
                Cube thirdCube = board.cubes[x + 2, y].GetComponent<Cube>();

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

        for (int x = 0; x < board.cubeCountX; x++)
        {
            for (int y = 0; y < board.cubeCountY; y++)
            {
                if (y > board.cubeCountY - checkCount || board.cubes[x, y] == null || board.cubes[x, y + 1] == null || board.cubes[x, y + 2] == null)
                {
                    continue;
                }

                Cube firstCube = board.cubes[x, y].GetComponent<Cube>();
                Cube secondCube = board.cubes[x, y + 1].GetComponent<Cube>();
                Cube thirdCube = board.cubes[x, y + 2].GetComponent<Cube>();

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

    //public void CheckPattern_Square()
    //{
    //    int checkCount = 2;

    //    for (int x = 0; x < board.amountX; x++)
    //    {
    //        for (int y = 0; y < board.amountY; y++)
    //        {
    //            // y = 0 1 2 3
    //            if (x > board.amountX - checkCount)
    //            {
    //                continue;
    //            }

    //            if (y > board.amountY - checkCount)
    //            {
    //                continue;
    //            }

    //            Cube firstCube = board.cubes[x, y].GetComponent<Cube>();
    //            Cube secondCube = board.cubes[x + 1, y].GetComponent<Cube>();
    //            Cube thirdCube = board.cubes[x, y + 1].GetComponent<Cube>();
    //            Cube fourthCube = board.cubes[x + 1, y + 1].GetComponent<Cube>();

    //            if (firstCube.type == secondCube.type && firstCube.type == thirdCube.type && firstCube.type == fourthCube.type)
    //            {
    //                cubes.Add(firstCube);
    //                cubes.Add(secondCube);
    //                cubes.Add(thirdCube);
    //                cubes.Add(fourthCube);
    //            }
    //        }
    //    }

    //    CheckResult((int)Pattern.Square);
    //}

    //private void CheckResult(int index)
    //{
    //    cubeChecks[index] = true;

    //    int checkCount = 0;

    //    for (int i = 0; i < cubeChecks.Length; i++)
    //    {
    //        if (cubeChecks[i] == true)
    //        {
    //            ++checkCount;
    //        }
    //    }

    //    if (checkCount == cubeChecks.Length)
    //    {
    //        //DeletPuzzle();
    //    }
    //}

    //public void DeletPuzzle()
    //{
    //    for (int i = 0; i < cubes.Count; i++)
    //    {
    //        Destroy(cubes[i].gameObject);
    //    }
    //}

    private void OnDisable()
    {
        Board.onSwapEvent -= CheckAllPattern;
    }
}