using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

enum MatchResult
{
    NotMatch = -1,
    None,
    BigCandy
}

enum Pattern
{
    Row3,
    Col3,
    Square
}

public class PuzzleMatcher : MonoBehaviour
{
    // match가 성공했을 때 만들어져야 할 퍼즐들
    //private static List<Puzzle> _puzzles;
    private Board board;

    private List<Cube> cubes = new List<Cube>(); // 제거 될 큐브들을 리스트에 담기

    private bool[] cubeChecks = { false, false, false };

    private void Awake()
    {
        board = GetComponent<Board>();
    }

    private void OnEnable()
    {
        //Board.onSwapEvent += CheckPattern_3Row;
        Board.onSwapEvent += CheckPattern_3Col;
        //Board.onSwapEvent += CheckPattern_Square;
    }

    //public void ResetMatcher()
    //{
    //    cubes.Clear();

    //    for (int i = 0; i < cubeChecks.Length; i++)
    //    {
    //        cubeChecks[i] = false;
    //    }

    //    board.CheckCount();
    //}


    //public void CheckPattern_3Row()
    //{
    //    int checkCount = 3;

    //    for (int x = 0; x < board.amountX; x++)
    //    {
    //        for (int y = 0; y < board.amountY; y++)
    //        {
    //            // y = 0 1 2 3
    //            if (x > board.amountX - checkCount)
    //            {
    //                continue;
    //            }

    //            Cube firstCube = board.cubes[x, y].GetComponent<Cube>();
    //            Cube secondCube = board.cubes[x + 1, y].GetComponent<Cube>();
    //            Cube thirdCube = board.cubes[x + 2, y].GetComponent<Cube>();

    //            if (firstCube.type == secondCube.type && firstCube.type == thirdCube.type)
    //            {
    //                cubes.Add(firstCube);
    //                cubes.Add(secondCube);
    //                cubes.Add(thirdCube);
    //            }
    //        }
    //    }

    //    CheckResult((int)Pattern.Row3);
    //}


    public void CheckPattern_3Col()
    {
        Debug.Log("스왑 이벤트");
        int checkCount = 3;

        for (int x = 0; x < board.amountX; x++)
        {
            for (int y = 0; y < board.amountY; y++)
            {
                // y = 0 1 2 3
                if (y > board.amountY - checkCount || board.cubes[x, y] == null || board.cubes[x, y + 1] == null || board.cubes[x, y + 2] == null)
                {
                    continue;
                }

                Cube firstCube = board.cubes[x, y].GetComponent<Cube>();
                Cube secondCube = board.cubes[x, y + 1].GetComponent<Cube>();
                Cube thirdCube = board.cubes[x, y + 2].GetComponent<Cube>();

                if (firstCube.Type == secondCube.Type && firstCube.Type == thirdCube.Type)
                {
                    board.cubes[x, y] = null;
                    board.cubes[x, y + 1] = null;
                    board.cubes[x, y + 2] = null;

                    Destroy(firstCube.gameObject);
                    Destroy(secondCube.gameObject);
                    Destroy(thirdCube.gameObject);

                    board.CheckCount();
                    board.ReFillCubeCol(x, checkCount);
                    return;
                }
            }
        }


        //CheckResult((int)Pattern.Col3);
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
        //Board.onSwapEvent -= CheckPattern_3Row;
        Board.onSwapEvent -= CheckPattern_3Col;
        //Board.onSwapEvent -= CheckPattern_Square;
    }
}