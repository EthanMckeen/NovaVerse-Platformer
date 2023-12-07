using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBEvents : MonoBehaviour
{
    void SlashDamagePlayer()
    {
        if (PlayerController.Instance.transform.position.y < transform.position.y + 2f &&
             PlayerController.Instance.transform.position.y > transform.position.y - 2f
             &&
             (PlayerController.Instance.transform.position.x > transform.position.x ||
            PlayerController.Instance.transform.position.x < transform.position.x))
        {
            Hit(FinalBoss.Instance.SideAtkTransform, FinalBoss.Instance.SideAtkArea);
        }
        else if (PlayerController.Instance.transform.position.y > transform.position.y)
        {
            Hit(FinalBoss.Instance.UpAtkTransform, FinalBoss.Instance.UpAtkArea);
        }
        else if (PlayerController.Instance.transform.position.y < transform.position.y)
        {
            Hit(FinalBoss.Instance.DownAtkTransform, FinalBoss.Instance.DownAtkArea);
        }
    }


    void SlashFX()
    {
        FinalBoss.Instance.SlashAngle();
    }

    void DashFX()
    {
        FinalBoss.Instance.CreateDashFX();
    }

   void Hit(Transform _attackTransform, Vector2 _attackArea)
    {
        Collider2D[] _objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0);
        for (int i = 0; i < _objectsToHit.Length; i++)
        {
            if(_objectsToHit[i].GetComponent<PlayerController>() != null)
            {
                _objectsToHit[i].GetComponent<PlayerController>().TakeDamage(FinalBoss.Instance.damage);
            }
        }
    }

    void Parrying()
    {
        FinalBoss.Instance.parrying = true;
    }

    void BendDownCheck()
    {
        if (FinalBoss.Instance.barrageAttack)
        {
            StartCoroutine(BarrageAttackTransition());
        }
        if (FinalBoss.Instance.outbreakAttack)
        {
            StartCoroutine(OutBreakAttackTransition());
        }
        if (FinalBoss.Instance.bounceAttack)
        {
            FinalBoss.Instance.anim.SetTrigger("Bounce1");
        }
    }
    
    void BorO() 
    {
        if (FinalBoss.Instance.barrageAttack)
        {
            StartCoroutine(FinalBoss.Instance.Barrage());
        }
        if (FinalBoss.Instance.outbreakAttack)
        {
            StartCoroutine(FinalBoss.Instance.Outbreak());
        }
    }

    IEnumerator BarrageAttackTransition()
    {
        yield return new WaitForSeconds(1f);
        FinalBoss.Instance.anim.SetBool("Cast", true);
    }

    IEnumerator OutBreakAttackTransition()
    {
        yield return new WaitForSeconds(1f);
        FinalBoss.Instance.anim.SetBool("Cast", true);

    }

    void DestroyAfterDeath()
    {
        FinalBoss.Instance.DestroyAfterDeath();
    }

}
