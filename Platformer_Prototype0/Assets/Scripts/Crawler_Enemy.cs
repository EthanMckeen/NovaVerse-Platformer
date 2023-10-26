using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crawler_Enemy : Enemy
{
    
    [SerializeField] private float flipWaitTime;
    [SerializeField] private Transform wallCheckPoint;
    [SerializeField] private Vector2 wallBoxSize;
    [SerializeField] private Vector2 ledgeBoxSize;
    [SerializeField] private float wallCastDistance;
    [SerializeField] private float ledgeCastDistance;
    [SerializeField] private LayerMask whatIsGround;
    float timer, rot;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(wallCheckPoint.position + (transform.right / 2) + transform.up * wallCastDistance, wallBoxSize);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(wallCheckPoint.position + (transform.right/2) - transform.up * ledgeCastDistance, ledgeBoxSize);

    }


    protected override void Start()
    {
        base.Start();
        rb.gravityScale = 12f;
        ChangeState(EnemyStates.Crawler_Idle);
    }

    protected override void Update()
    {
        base.Update();
        if (!PlayerController.Instance.pState.alive)
        {
            ChangeState(EnemyStates.Crawler_Idle);
        }
    }


    protected override void UpdateEnemyStates()
    {
        if (health <= 0)
        {
            Death(0.05f);
        }

        switch (GetCurrentEnemyState)
        {
            case EnemyStates.Crawler_Idle:
                if (Physics2D.BoxCast(wallCheckPoint.position + (transform.right / 2), wallBoxSize, 0, transform.up, wallCastDistance, whatIsGround) ||
                    !Physics2D.BoxCast(wallCheckPoint.position + (transform.right / 2), ledgeBoxSize, 0, -transform.up, ledgeCastDistance, whatIsGround))
                {
                    ChangeState(EnemyStates.Crawler_Flip);
                }
                if (transform.rotation.y == 0f)
                {
                    anim.SetBool("Walking", true);
                    rb.velocity = new Vector2(speed, rb.velocity.y);
                }
                else
                {
                    anim.SetBool("Walking", true);
                    rb.velocity = new Vector2(-speed, rb.velocity.y);
                }
                break;

            case EnemyStates.Crawler_Flip:
                anim.SetBool("Walking", false);
                timer += Time.deltaTime;

                if(timer > flipWaitTime)
                {
                    timer = 0;
                    if (transform.rotation.y == 0f)
                    {
                        rot = 180f;
                    }
                    else
                    {
                        rot = 0f;
                    }
                    
                    Vector3 rotator = new Vector3(transform.rotation.x, rot, transform.rotation.z);
                    transform.rotation = Quaternion.Euler(rotator);
                    ChangeState(EnemyStates.Crawler_Idle);
                }




                break;
        }
    }
}
