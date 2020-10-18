using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform mTransform;
    public void SetTarget (Transform pTransform_ = null)
    {
        try
        {
            mTransform = pTransform_;
            if (mTransform != null)
            {
                this.transform.up = GameConstants.LookAt2D(this.transform.up, this.transform.position, mTransform.position);
                StartCoroutine("EnemyBulletMovement");
            }
            else
                StartCoroutine("PlayerBulletMovement");
        }
        catch(System.Exception e)
        {
            this.gameObject.SetActive(false);
        }
    }

    private IEnumerator EnemyBulletMovement ()
    {
        Vector2 normal = (mTransform.position - transform.position).normalized;
        while ((transform.position.y > -GameManager.instance._ScreenRect.y))
        {
            transform.position += transform.up * 3 * Time.deltaTime;
            yield return null;
        }
        this.gameObject.SetActive(false);
    }

    private IEnumerator PlayerBulletMovement()
    {
        bool isMove = true;
        while (isMove)
        {
            transform.Translate(this.transform.up * 5 * Time.deltaTime);
            isMove = (transform.position.y > 5) ? false : true;
            yield return null;
        }
        this.gameObject.SetActive(false);
    }
}
