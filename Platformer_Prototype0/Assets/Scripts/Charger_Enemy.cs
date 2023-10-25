using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charger_Enemy : Enemy
{

    [SerializeField] private Transform wallCheckPoint;
    [SerializeField] private Vector2 wallBoxSize;
    [SerializeField] private Vector2 ledgeBoxSize;
    [SerializeField] private Vector2 boxSize;
    [SerializeField] private float wallCastDistance;
    [SerializeField] private float ledgeCastDistance;
    [SerializeField] private float ledgeCastHeight;
    [SerializeField] private float castDistance;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private float jumpForce;
    [SerializeField] private float chargeDuration;
    [SerializeField] private float chargeSpeedMultiplier;
    float timer, rot;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(wallCheckPoint.position + (transform.right * wallCastDistance), wallBoxSize);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(wallCheckPoint.position + (transform.right * ledgeCastDistance) - (transform.up * ledgeCastHeight), ledgeBoxSize); //ledge check
        Gizmos.DrawWireCube(wallCheckPoint.position + (transform.right * ledgeCastDistance), ledgeBoxSize); //wall check
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(wallCheckPoint.position - transform.up * castDistance, boxSize);

    }


    protected override void Start()
    {
        base.Start();
        ChangeState(EnemyStates.Charger_Idle);
        rb.gravityScale = 12f;
    }




    protected override void UpdateEnemyStates()
    {
        if (health <= 0)
        {
            Death(0.05f);
        }

        switch (GetCurrentEnemyState)
        {
            case EnemyStates.Charger_Idle:
                if (Physics2D.BoxCast(wallCheckPoint.position + (transform.right), ledgeBoxSize, 0, transform.up, ledgeCastDistance, whatIsGround))
                {
                    Flip();
                }
                RaycastHit2D _hit = Physics2D.BoxCast(wallCheckPoint.position + (transform.right * wallCastDistance), wallBoxSize, 0, transform.up, wallCastDistance, _playerLayer);
                if (_hit.collider != null && _hit.collider.gameObject.CompareTag("Player"))
                {
                    Flip();
                    ChangeState(EnemyStates.Charger_Suprised);
                }
                break;


            case EnemyStates.Charger_Suprised:
                
                ChangeState(EnemyStates.Charger_Charge);
                break;

            case EnemyStates.Charger_Charge:
                timer += Time.deltaTime;

                if(timer < chargeDuration)
                {
                    Debug.Log(Physics2D.BoxCast(wallCheckPoint.position, boxSize, 0, -transform.up, castDistance, whatIsGround));
                    if (Physics2D.BoxCast(wallCheckPoint.position, boxSize, 0, -transform.up, castDistance, whatIsGround))
                    {
                        if (transform.rotation.y == 0f)
                        {

                            rb.velocity = new Vector2(speed * chargeSpeedMultiplier, rb.velocity.y);
                        }
                        else
                        {

                            rb.velocity = new Vector2(-speed * chargeSpeedMultiplier, rb.velocity.y);
                        }
                    }
                    else
                    {
                        rb.velocity = new Vector2(0, rb.velocity.y);
                    }
                }
                else
                {
                    timer = 0;
                    ChangeState(EnemyStates.Charger_Idle);
                }
                break;
        }
    }


    protected override void ChangeCurrentAnimation()
    {

        anim.SetBool("Idle", GetCurrentEnemyState == EnemyStates.Charger_Idle);
        anim.SetBool("Charging", GetCurrentEnemyState == EnemyStates.Charger_Charge);
        anim.SetBool("Suprised", GetCurrentEnemyState == EnemyStates.Charger_Suprised);

        if (GetCurrentEnemyState == EnemyStates.Flyer_Death)
        {
            anim.SetTrigger("Death");
        }


        if (GetCurrentEnemyState == EnemyStates.Charger_Idle)
        {
            anim.speed = 1;
        }
        if (GetCurrentEnemyState == EnemyStates.Charger_Charge)
        {
            anim.speed = chargeSpeedMultiplier;
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
