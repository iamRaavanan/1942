using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public float _health;
    public float _hitRate;
    public float _speed;
    public EnemyType _type;
    public SpawnLocation _spawnLocation;
    
    protected Vector2 mTargetPosition;

    protected List<Vector2> mTargetValues;

    protected Quaternion mEulerAngleVelocity;

    protected bool mIsUpdatingIndex = false;
    protected bool mIsEndIndex = false;
    protected bool mIsPositiveTurn;
    protected bool mCanShoot = false;

    protected int mCurrentIndex = 0;
    protected float mRotatingAngle;

    protected Bounds mBounds;

    [SerializeField] private GameObject mBulletPrefab;
    [SerializeField] private int mBulletPoolIndex;
    [SerializeField] private Transform[] mBulletTransform;
    private float mSpeed;
    private float mstartAngle = 0f;
    private float mCurrentAngle = 0f;

    public void SetEnemyType (EnemyType pType_, List<Vector2> pTargetValues)
    {
        try
        {
            _type = pType_;
            mTargetValues = pTargetValues;
            mBounds = GetComponent<SpriteRenderer>().bounds;
            Vector2 value = new Vector2(transform.position.x - EnemyManager.instance._targetDetail._straightPos[0].x, transform.position.y - EnemyManager.instance._targetDetail._straightPos[0].y);
            mTargetPosition = mTargetValues[0];
            mSpeed = _speed;
            mEulerAngleVelocity = transform.rotation;

            if (pType_ != EnemyType.E_Type2)
                this.transform.up = GameConstants.LookAt2D(this.transform.up, this.transform.position, mTargetValues[0]);

            StartCoroutine("ThrowBullet");
        }
        catch (Exception e)
        {
            this.gameObject.SetActive(false);
        }
    }

    private IEnumerator ThrowBullet ()
    {
        float InDelay = 1.0f / _hitRate;
        float waitTime = 0f;
        while (waitTime < 1)
        {
            waitTime += InDelay * Time.deltaTime;
            yield return null;
        }
        ShootTarget();
        StartCoroutine("ThrowBullet");
    }

    private void ShootTarget()
    {
        if (!mCanShoot)
            return;

        foreach (Transform t in mBulletTransform)
        {
            //GameObject InBullet = Instantiate(mBulletPrefab) as GameObject;
            GameObject InBullet = PoolManager.instance.GetPooledObject(mBulletPoolIndex);
            InBullet.SetActive(true);
            InBullet.transform.position = t.position;
            InBullet.GetComponent<Bullet>().SetTarget(GameManager.instance._playerHandler.transform);
        }
    }

    protected void SetRotationAngle ()
    {
        try
        {
            if (mIsUpdatingIndex)
                return;
            mIsUpdatingIndex = true;
            mstartAngle = mCurrentAngle = transform.localEulerAngles.z;
            mCurrentIndex++;
            mIsEndIndex = ((mCurrentIndex + 1) == mTargetValues.Count);
            Vector2 direction = mTargetValues[mCurrentIndex] - new Vector2(this.transform.position.x, this.transform.position.y);
            //Debug.Log("Setting Rotation : " + mTargetValues[mCurrentIndex]);
            direction = GameConstants.GetNormal(direction);
            mRotatingAngle = GameConstants.Angle(this.transform.up, direction) /** 180/Mathf.PI*/;
            //if (GameConstants.Cross(this.transform.up, direction).z < 0)
            //    mRotatingAngle = 2 * Mathf.PI - mRotatingAngle;
            mRotatingAngle = mRotatingAngle * Mathf.Rad2Deg;
            mEulerAngleVelocity = Quaternion.Euler(0, 0, mRotatingAngle);
            mIsPositiveTurn = (GameConstants.Cross(this.transform.up, direction).z > 0) ? false : true;
            //Debug.Log("CROSS => " + mIsPositiveTurn + ":::" + mRotatingAngle);
        }
        catch (Exception e)
        {
            this.gameObject.SetActive(false);
        }
    }

    protected void FixedUpdate ()
    {
        if (mIsUpdatingIndex)
        {
            this.transform.position += this.transform.up * _speed * Time.deltaTime;
            transform.localEulerAngles += Vector3.forward * (mIsPositiveTurn ? 250 : -250) * Time.deltaTime;
            mCurrentAngle += (mIsPositiveTurn ? 250 : -250) * Time.deltaTime;
            //Debug.Log(mstartAngle + "==" + mCurrentAngle + " :: " + mEulerAngleVelocity.eulerAngles.z);
            if (Mathf.Abs(mstartAngle - mCurrentAngle) >= mEulerAngleVelocity.eulerAngles.z)
                ResetToNormalSpeed();
        }
    }

    protected void ResetToNormalSpeed ()
    {
        try
        {
            mTargetPosition = mTargetValues[mCurrentIndex];
            _speed = mSpeed;
            mIsUpdatingIndex = false;
        }
        catch (Exception e)
        {
            this.gameObject.SetActive(false);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("triggered with " + collision.gameObject.name);
        if (collision.gameObject.tag == "Bullet")
        {
            GameManager._enemyKillEvnt?.Invoke();
            if (GameManager.instance._triggerPowerup)
            {
                //Debug.Log("Powerup triggered");
                GameManager._onPowerupSpawnEvnt?.Invoke(this.transform.position);
            }
            collision.gameObject.SetActive(false);
            this.gameObject.SetActive(false);
        }
    }
}

public enum EnemyType
{
    E_None,
    E_Type1,
    E_Type2,
    E_Type3
}

public enum SpawnLocation
{
    E_Up,
    E_Down,
    E_Left,
    E_Right
}

public enum State
{
    E_Idle,
    E_Move,
    E_Turn,
    E_Rotate
}