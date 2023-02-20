using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    public Action onSwapEvent;
    [SerializeField]
    private GameObject ClearPanel;

    [SerializeField]
    private TextMeshProUGUI ScoreTMPro;

    [SerializeField]
    private TextMeshProUGUI MunchkinCountTMPro;

    [SerializeField]
    private GameObject MunchkinCheckImage;

    [SerializeField]
    private int MunchkinGoalCount;

    [SerializeField]
    private int InitMoveCount;

    [Header("Score")]
    public int MunchikinGoalInScore;
    public int blockDestructionScore;

    private int scoreCount;

    private int munchkinCount;
    public int MunchkinCount
    {
        get => munchkinCount;
        set => munchkinCount = value;
    }

    private void Awake()
    {
        Instance = this;
        init();
    }

    private void init()
    {
        munchkinCount = MunchkinGoalCount;
        scoreCount = 0;

        ScoreTMPro.text = scoreCount.ToString();
        MunchkinCountTMPro.text = munchkinCount.ToString();
    }

    public void SetScore(int score)
    {
        scoreCount += score;
        ScoreTMPro.text = scoreCount.ToString();
    }

    public void SetMunchkinCount()
    {
        munchkinCount -= 1;
        MunchkinCountTMPro.text = munchkinCount.ToString();
        SetScore(MunchikinGoalInScore);

        if (munchkinCount == 0)
        {
            MunchkinCheckImage.SetActive(true);
        }
    }

    public void ActivationClearPanel()
    {
        ClearPanel.SetActive(true);
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene(0);
    }
}
