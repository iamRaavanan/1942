using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    [SerializeField] private Transform[] mBaseBulletTransformArr;
    [SerializeField] private Transform[] mMediumBulletTransformArr;
    [SerializeField] private int mBulletIndex;
    private Transform[] mBulletTransform;    
    private float mBulletDelay = 0.35f;

    private bool mTouchInitiated;

    private Vector2 mStartPosition;
    private Vector2 mCurrentPosition;
    private Vector2 mNormalizedInput;
    private Vector2 mBoundSize;

    private Rigidbody2D mRigidbody2d;

    private Camera mCamera;
    private float mXvalue;
    private float mYvalue;

    private void Start()
    {
        GameManager._gameStartEvnt += ResetPlayer;
    }
    private void Update()
    {
        if (GameConstants.isGameStarted)
        {
#if UNITY_EDITOR
            if (Input.GetMouseButton(0))
            {
                if (!mTouchInitiated)
                {
                    mTouchInitiated = true;
                    mStartPosition = Input.mousePosition;
                }
                else
                {
                    mCurrentPosition = Input.mousePosition;
                    mNormalizedInput = (mCurrentPosition - mStartPosition).normalized;
                    transform.position += new Vector3(mNormalizedInput.x * 6f * Time.deltaTime, mNormalizedInput.y * 4f * Time.deltaTime, 0);
                    transform.position = new Vector3(Mathf.Clamp(transform.position.x, -2f, 2f), Mathf.Clamp(transform.position.y, -4f, 4f), transform.position.z);
                }
            }
            else
            {
                mTouchInitiated = false;
            }
#endif
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                Vector2 touchPos = mCamera.ScreenToWorldPoint(touch.position);
                if (touch.phase == TouchPhase.Began)
                {
                    mXvalue = touchPos.x - transform.position.x;
                    mYvalue = touchPos.y - transform.position.y;
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    mRigidbody2d.MovePosition(new Vector2(touchPos.x - mXvalue, touchPos.y - mYvalue));
                }
            }
        }
    }

    private IEnumerator SpawnBullet ()
    {
        float InDelay = 1.0f/ mBulletDelay;
        float InComplete = 0f;
        while (InComplete < 1)
        {
            InComplete += InDelay * Time.deltaTime;
            yield return null;
        }
        ShootTarget();
        StartCoroutine("SpawnBullet");
    }

    private void ShootTarget()
    {
        foreach (Transform t in mBulletTransform)
        {
            GameObject InBullet = PoolManager.instance.GetPooledObject(mBulletIndex);
            InBullet.SetActive(true);
            InBullet.transform.position = t.position;
            InBullet.GetComponent<Bullet>().SetTarget();
        }
    }

    public void UpdatePowerupEffect ()
    {
        if (GameManager.instance._powerupLvl < 3)
        {
            mBulletDelay -= 0.12f;
        }
        else if (GameManager.instance._powerupLvl == 3)
        {
            mBulletTransform = mMediumBulletTransformArr;
        }
        else
        {
            mBulletTransform = new Transform[3];
            mBulletTransform[0] = mBaseBulletTransformArr[0];
            mBulletTransform[1] = mMediumBulletTransformArr[0];
            mBulletTransform[2] = mMediumBulletTransformArr[1];
        }
    }

    public void ResetPlayer ()
    {
        transform.position = new Vector3(0, -4, 0);
        mBulletDelay = 0.35f;
        mRigidbody2d = GetComponent<Rigidbody2D>();
        mCamera = Camera.main;
        mBoundSize = GetComponent<SpriteRenderer>().size;
        GameManager.instance.SetLevel(1);
        mBulletTransform = mBaseBulletTransformArr;
        StartCoroutine("SpawnBullet");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Player triggered with " + collision.gameObject.name);
        string InTag = collision.gameObject.tag;
        collision.gameObject.SetActive(false);
        if (InTag == "E_Bullet" || InTag == "Enemy")
        {
            StopCoroutine("SpawnBullet");
            GameManager._playerKillEvnt?.Invoke();
            this.gameObject.SetActive(false);
        }
        else if (InTag == "Powerup")
        {
            GameManager._powerupCaptureEvnt?.Invoke();
        }
    }

    private void OnDisable()
    {
        GameManager._gameStartEvnt -= ResetPlayer;
    }
}
