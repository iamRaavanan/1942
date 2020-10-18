using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private static EnemyManager mInstance;
    private float mDelay = 1f;
    private float mLevelUpdateTime = 25f;
    private int mRangeUnlocked = 3;
    public static EnemyManager instance
    {
        get { return mInstance; }
    }

    public TargetPositionDetail _targetDetail;

    public void OnEnable()
    {
        GameManager._playerKillEvnt += OnLifeReduced;
        GameManager._gameStartEvnt += ResetGame;
    }

    private void Start()
    {
        mInstance = this;
    }

    private void OnLifeReduced ()
    {
        StopCoroutine("CustomUpdate");
        EnemyBase[] eb = GameObject.FindObjectsOfType<EnemyBase>();
        foreach(EnemyBase eb1 in eb)
        {
            eb1.transform.position = new Vector3(2000, 0, 0);
            eb1.gameObject.SetActive(false);
        }
        Bullet[] bullets = GameObject.FindObjectsOfType<Bullet>();
        foreach(Bullet b in bullets)
        {
            b.transform.position = new Vector3(1800, 0, 0);
            b.gameObject.SetActive(false);
        }
    }

    public void ResetGame ()
    {
        mRangeUnlocked = 3;
        GameManager.instance.SetLevel(1);
        StartCoroutine("CustomUpdate");
    }

    private IEnumerator CustomUpdate()
    {
        float InTime = Time.time, InlevelTime = Time.time;
        float elapsedTime = 0f;
        while(!GameConstants.isGameOver)
        {
            elapsedTime = Time.time - InTime;
            while (elapsedTime > mDelay)
            {
                elapsedTime = 0f;
                int random = UnityEngine.Random.Range(1, 7);
                InTime = Time.time + (random/2.0f);
                StartCoroutine(SpawnEnemy(random));
                yield return null;
            }
            while (mLevelUpdateTime < InlevelTime)
            {
                GameManager.instance.SetLevel(GameManager.instance._level + 1);
                mRangeUnlocked += (mRangeUnlocked < 7) ? 1 : 0;
                mLevelUpdateTime += (mLevelUpdateTime * GameManager.instance._level);
            }
            yield return null;
        }
    }

    private IEnumerator SpawnEnemy (int pCount_ = 1)
    {
        int InSelectedEnemy = UnityEngine.Random.Range(0, mRangeUnlocked) + 4;
        EnemyType InType = (InSelectedEnemy == 10) ? EnemyType.E_Type2 : EnemyType.E_Type1;
        pCount_ = (InSelectedEnemy == 10) ? 1 : pCount_;
        int SpawnPos = UnityEngine.Random.Range(0, 3);
        List <Vector2> targetPositions = SetAndRetrieveupTargetPosition(InType, SpawnPos);
        for (int i = 0;  i < pCount_; i++)
        {
            GameObject InGO = PoolManager.instance.GetPooledObject(InSelectedEnemy);
            InGO.SetActive(true);
            InGO.transform.position = (InType == EnemyType.E_Type1) ? _targetDetail._upPos[SpawnPos] : _targetDetail._bossPos;
            InGO.GetComponent<EnemyType1>().SetEnemyType(InType, targetPositions);
            yield return new WaitForSeconds(0.5f);
        }
    }

    private List<Vector2> SetAndRetrieveupTargetPosition (EnemyType pType_, int pSpawnPos_)
    {
        List<Vector2> InList = new List<Vector2>();
        int InRandom = 0;
        switch (pType_)
        {
            case EnemyType.E_None:
                InList = null;
                return InList;
            case EnemyType.E_Type1:
                if (pSpawnPos_ == 0)
                {
                    InRandom = UnityEngine.Random.Range(0, _targetDetail._topSpawnMovementPos.Count);
                    InList = _targetDetail._topSpawnMovementPos[InRandom]._path;
                }
                else if (pSpawnPos_ == 1)
                {
                    InRandom = UnityEngine.Random.Range(0, _targetDetail._toprightSpawnMovementPos.Count);
                    InList = _targetDetail._toprightSpawnMovementPos[InRandom]._path;
                }
                else if (pSpawnPos_ == 2)
                {
                    InRandom = UnityEngine.Random.Range(0, _targetDetail._topleftSpawnMovementPos.Count);
                    InList = _targetDetail._topleftSpawnMovementPos[InRandom]._path;
                }
                break;
            case EnemyType.E_Type2:
                InList = _targetDetail._bottomSpawnMovementPos[0]._path;
                break;
            case EnemyType.E_Type3:
                break;
            default:
                break;
        }
        return InList;
    }

    public void OnDisable()
    {
        GameManager._playerKillEvnt -= OnLifeReduced;
        GameManager._gameStartEvnt -= ResetGame;
    }
}
