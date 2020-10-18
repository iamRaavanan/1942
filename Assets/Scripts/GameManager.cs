using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int mPowerupIndex;
    private static GameManager mInstance;
    public static GameManager instance { get { return mInstance; } }
    public int _level { get; private set; }
    public int _score { get; private set; }
    public int _life { get; private set; }
    public int _powerupLvl { get; private set; }
    public bool _triggerPowerup { get; private set; }

    public Vector2 _ScreenRect { get; private set; }

    public PlayerHandler _playerHandler;
    public EnemyManager _enemyManager;
    public UIManager _uiManager;

    public static Action _enemyKillEvnt;
    public static Action _playerKillEvnt;
    public static Action _gameStartEvnt;
    public static Action<Vector3> _onPowerupSpawnEvnt;
    public static Action _powerupCaptureEvnt;

    private int mPowerupInitScore = 25;

    private void OnEnable()
    {
        _life = 3;
        _enemyKillEvnt += OnKilledEnemy;
        _onPowerupSpawnEvnt += SpawnPowerup;
        _powerupCaptureEvnt += OnPoweupCaptured;
        _playerKillEvnt += OnLifeReduced;
        //_ScreenRect = Camera.main.ScreenToViewportPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        _ScreenRect = new Vector2(2.3f, 4.45f);
        //Debug.Log("SR=> " + _ScreenRect);
    }

    private void OnLifeReduced()
    {
        --_life;
        _uiManager.OnUpdateLife(_life);
        if (_life > 0)
            StartCoroutine("RestartAfterDelay");
        else
            _uiManager.ShowGameOverScreen();
    }

    private IEnumerator RestartAfterDelay ()
    {
        float InTime = 1 / 2f;
        float InPercentage = 0f;
        while(InPercentage <1)
        {
            InPercentage += InTime * Time.deltaTime;
            yield return null;
        }
        _uiManager.DisableWasted();
        _playerHandler.gameObject.SetActive(true);
        _playerHandler.ResetPlayer();
        _enemyManager.ResetGame();
    }

    private void OnPoweupCaptured()
    {
        ++_powerupLvl;
        mPowerupInitScore = 25;
        _playerHandler.UpdatePowerupEffect();
    }

    public void SetLevel(int pLevel_)
    {
        _level = pLevel_;
    }
    private void OnKilledEnemy()
    {
        ++_score;
        --mPowerupInitScore;
        _uiManager.OnUpdateScroe();
        if (mPowerupInitScore == 0 && _powerupLvl < 4)
            _triggerPowerup = true;
    }

    private void SpawnPowerup(Vector3 pPosition_)
    {
        if (!_triggerPowerup)
            return;
        GameObject InPowerup = PoolManager.instance.GetPooledObject(mPowerupIndex);
        InPowerup.SetActive(true);
        InPowerup.transform.position = new Vector3(pPosition_.x, 0, pPosition_.z);
        _triggerPowerup = false;
    }

    private void Awake()
    {
        mInstance = this;
    }

    private void OnDisable()
    {
        _enemyKillEvnt -= OnKilledEnemy;
        _onPowerupSpawnEvnt -= SpawnPowerup;
        _powerupCaptureEvnt -= OnPoweupCaptured;
        _playerKillEvnt -= OnLifeReduced;
    }
}
