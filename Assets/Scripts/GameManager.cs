using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Score")]
    [SerializeField] private TextMeshProUGUI ScoreTMPro;                 // 점수 텍스트
    public int MunchikinGoalInScore;                                     // 먼치킨 골인 점수
    public int blockDestructionScore;                                    // 블록 제거 점수

    [Header("Clear")]
    [SerializeField] private GameObject ClearPanel;                      // 게임 결과 패널
    [SerializeField] private TextMeshProUGUI ResultScoreTMpro;           // 게임 결과 텍스트

    [Header("Goal")]
    [SerializeField] private TextMeshProUGUI MunchkinCountTMPro;         // 먼치킨 텍스트
    [SerializeField] private int MunchkinGoalCount;                      // 먼치킨 목표점수
    [SerializeField] private GameObject MunchkinCheckImage;              // 먼치킨 이미지

    [Header("System")]
    [SerializeField] private TextMeshProUGUI StatusTMpro;                // 매칭 상황을 알리는 텍스트

    [Header("Events")]
    //public Action onSwapEvent;                                         // 스왑 이벤트
    public Action onButtonEnableEvent;                                   // 캔디버튼 활성화 이벤트
    public Action onButtonDisableEvent;                                  // 캔디버튼 비활성화 이벤트

    public static GameManager Instance;                                  // 전역화
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
        Init();
        Application.targetFrameRate = 60;
    }

    private void OnEnable()
    {
        onButtonEnableEvent += DeActivationStatus;
        onButtonDisableEvent += ActivationStatus;
    }

    private void Init()
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

        if (munchkinCount <= 0)
        {
            MunchkinCheckImage.SetActive(true);
        }
    }

    public void ActivationClearPanel()
    {
        ClearPanel.SetActive(true);
        ResultScoreTMpro.text = scoreCount.ToString();
    }

    private bool isOnMatching;
    private void ActivationStatus()
    {
        isOnMatching = true;
        StartCoroutine(OnMatching());
    }

    private void DeActivationStatus()
    {
        StatusTMpro.text = "";
        isOnMatching = false;
        StopAllCoroutines();
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene(0);
    }

    private void OnDisable()
    {
        onButtonEnableEvent -= DeActivationStatus;
        onButtonDisableEvent -= ActivationStatus;
    }

    private IEnumerator OnMatching()
    {
        string dot = ".";
        int dotCount = 0;
        StatusTMpro.text = "매칭중";

        while (true)
        {
            ++dotCount;
            StatusTMpro.text += dot;

            if (dotCount > 5)
            {
                StatusTMpro.text = "매칭중";
                dotCount = 0;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
}
