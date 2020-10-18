using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyType1 : EnemyBase
{
    private void Update()
    {
        if (!mIsUpdatingIndex && _type != EnemyType.E_Type2)
        {
            transform.position = Vector3.MoveTowards(transform.position, mTargetPosition, _speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, mTargetPosition) < 0.2f)
            {
                if (!mIsEndIndex)
                    SetRotationAngle();
                else
                {
                    //Destroy(this.gameObject);
                    this.gameObject.SetActive(false);
                }
            }
        }
        if (_type == EnemyType.E_Type2)
        {
            if(mIsEndIndex)
            {
                transform.position = new Vector3(Mathf.PingPong(Time.time, 1.3f), transform.position.y, transform.position.z);
            }
            else
            {
                if (Vector3.Distance(transform.position, mTargetPosition) > 0.2f)
                    transform.position = Vector3.MoveTowards(transform.position, mTargetPosition, _speed * Time.deltaTime);
                else
                    mIsEndIndex = true;
            }
        }
        mCanShoot = CheckForShoot();
    }

    private bool CheckForShoot()
    {
        if (transform.localPosition.x > -GameManager.instance._ScreenRect.x - mBounds.size.x/2 && transform.localPosition.x < GameManager.instance._ScreenRect.x + mBounds.size.x/2 && 
            transform.localPosition.y > -GameManager.instance._ScreenRect.y - mBounds.size.y/2 && transform.localPosition.y < GameManager.instance._ScreenRect.y + mBounds.size.y/2)
        {
            return true;
        }
        return false;
    }
}
