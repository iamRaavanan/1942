using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text mScoreTxt;
    [SerializeField] private Text mYourscoreTxt;
    [SerializeField] private Text mHighscoreTxt;
    [SerializeField] private Text mCountdownTxt;

    [SerializeField] private GameObject[] mLifeArr;
    [SerializeField] private GameObject mGameOverGO;
    [SerializeField] private GameObject mWastedGO;
    [SerializeField] private GameObject mPlayBtnGO;
    [SerializeField] private GameObject mMenuGO;

    private int mSceneIndex = 0;
    private void OnEnable()
    {
        mCountdownTxt.text = string.Empty;
        SceneManager.sceneLoaded += OnSceneLoaded;
        GameManager._playerKillEvnt += OnLifeReduced;
    }

    public void OnPlayClicked ()
    {
        mPlayBtnGO.SetActive(false);
        StartCoroutine("BeginGameWithCountdown");
    }

    private IEnumerator BeginGameWithCountdown()
    {
        float InTime = 1.0f / 3.0f;
        float Inpercent = 0f;
        float cTime = Time.time;
        while (Inpercent < 1)
        {
            Inpercent += InTime * Time.deltaTime;
            mCountdownTxt.text = string.Format("{0}", (3 - (int)(Time.time - cTime)));
            yield return null;
        }
        mMenuGO.SetActive(false);
        GameManager._gameStartEvnt?.Invoke();
        GameConstants.isGameStarted = true;
    }

    private void OnLifeReduced ()
    {
        mWastedGO.SetActive(true);
    }

    public void DisableWasted ()
    {
        mWastedGO.SetActive(false);
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        mSceneIndex = arg0.buildIndex;
        if (GameConstants.isRetryClicked)
        {
            GameConstants.isRetryClicked = false;
            OnPlayClicked();
        }
    }

    public void OnUpdateScroe()
    {
        mScoreTxt.text = string.Format("{0}", GameManager.instance._score);
    }

    public void OnUpdateLife (int pRemainingLife_)
    {
        for (int i = 0; i < mLifeArr.Length; i++)
        {
            mLifeArr[i].SetActive((i < pRemainingLife_) ? true : false);
        }
    }

    public void ShowGameOverScreen()
    {
        int InHighscore = PlayerPrefs.GetInt("highscore", 0);
        InHighscore = (GameManager.instance._score > InHighscore) ? GameManager.instance._score : InHighscore;
        PlayerPrefs.SetInt("highscore", InHighscore);
        mYourscoreTxt.text = string.Format("{0}", GameManager.instance._score);
        mHighscoreTxt.text = string.Format("{0}", InHighscore);
        Time.timeScale = 0f;
        mGameOverGO.SetActive(true);
    }

    public void RetryClicked ()
    {
        Time.timeScale = 1f;
        GameConstants.isRetryClicked = true;
        SceneManager.LoadScene(mSceneIndex);
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        GameManager._playerKillEvnt -= OnLifeReduced;
    }
}
