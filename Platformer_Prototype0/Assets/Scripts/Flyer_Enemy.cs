using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flyer_Enemy : Enemy
{

    [SerializeField] private float chaseDistance;
    [SerializeField] private float stunDuration;
    private float timer;


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        ChangeState(EnemyStates.Flyer_Idle);
    }

    protected override void UpdateEnemyStates()
    {
        float _dist = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);


        switch (GetCurrentEnemyState)
        {
            case EnemyStates.Flyer_Idle:
                if(_dist < chaseDistance)
                {
                    ChangeState(EnemyStates.Flyer_Chase);
                }

                break;

            case EnemyStates.Flyer_Chase:
                rb.MovePosition(Vector2.MoveTowards(transform.position, PlayerController.Instance.transform.position, Time.deltaTime * speed));

                Flip();

                break;

            case EnemyStates.Flyer_Stun:
                timer += Time.deltaTime;

                if(timer > stunDuration)
                {
                    ChangeState(EnemyStates.Flyer_Idle);
                    timer = 0;
                }
                break;

            case EnemyStates.Flyer_Death:
                Death(Random.Range(2, 4));
                break;

        }
    }

    public override void EnemyHit(float _dmgDone, Vector2 _hitDirection, float _hitForce)
    {
        base.EnemyHit(_dmgDone, _hitDirection, _hitForce);

        if(health > 0)
        {
            ChangeState(EnemyStates.Flyer_Stun);

        }
        else
        {
            ChangeState(EnemyStates.Flyer_Death);
            
        }
    }

    protected override void Death(float _destroyTime)
    {
        rb.gravityScale = 12;
        base.Death(_destroyTime);
    }

    protected override void ChangeCurrentAnimation()
    {
        anim.SetBool("Idle", GetCurrentEnemyState == EnemyStates.Flyer_Idle);
        anim.SetBool("Chase", GetCurrentEnemyState == EnemyStates.Flyer_Chase);
        anim.SetBool("Stunned", GetCurrentEnemyState == EnemyStates.Flyer_Stun);

        if(GetCurrentEnemyState == EnemyStates.Flyer_Death)
        {
            anim.SetTrigger("Death");
        }
    }
    void Flip()
    {
        if (PlayerController.Instance.transform.position.x < transform.position.x)
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
        }
        else 
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
        }

    }



}
