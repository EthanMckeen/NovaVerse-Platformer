using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBoss : Enemy
{

    [SerializeField] private SlimeBlock blockade;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private Vector2 boxSize;
    [SerializeField] private float castDistance;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float jumpForceY;
    [SerializeField] private float jumpCoolDown;
    [SerializeField] private float chargeCoolDown;
    [SerializeField] private float chargeTimer;
    [SerializeField] private float jumpTimer;
    public bool cutscene = false;

    public static SlimeBoss Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        blockade = SlimeBlock.instance;
    }
    protected override void Start()
    {
        if (PlayerController.Instance.pState.slimeKilled)
        {
            //if player has killed dont respawn
            gameObject.SetActive(false);
        }
        base.Start();
        rb.gravityScale = 12f;
        jumpTimer = 0;
        chargeTimer = 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(groundCheckPoint.position - transform.up * castDistance, boxSize);

    }

    // Update is called once per frame
    protected override void Update()
    {
        if (this.health <= 0)
        {
            //if its dead break blockade to progress
            Destroy(SlimeBlock.instance.gameObject);
            //add the playercheck 
            PlayerController.Instance.pState.slimeKilled = true;
            Destroy(gameObject);
        }
        base.Update();
        if (cutscene) { return; }
        Turn();
        //count for the timers
        if (Grounded() && chargeTimer < chargeCoolDown && jumpTimer > jumpCoolDown) 
        {
            chargeTimer += Time.deltaTime;
        }
        else if (Grounded())
        {
            jumpTimer += Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (cutscene) { return; }
        JumpAttack();
    }

    private bool Grounded()
    {
        if (Physics2D.BoxCast(groundCheckPoint.position, boxSize, 0, -transform.up, castDistance, whatIsGround))
        {
            return true;
        }
        else { return false; }
    }

    private void JumpAttack()
    {
        if (Grounded() && chargeTimer > chargeCoolDown) //ready to jump
        {
            anim.ResetTrigger("Charge");
            //jump
            jumpTimer = 0f;
            chargeTimer = 0f;
            rb.velocity = new Vector3(DistanceFromPlayer(), jumpForceY);
        }
        else if (Grounded() && chargeTimer < chargeCoolDown && jumpTimer > jumpCoolDown) //on ground and can't jump
        {
            anim.SetTrigger("Charge"); //charge before the jump
        }

           

    }
    private void Turn()
    {
        if (DistanceFromPlayer() > 0)
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
        }
        else if (DistanceFromPlayer() < 0)
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
        }
    }
    private float DistanceFromPlayer()
    {
        float d = PlayerController.Instance.transform.position.x - transform.position.x;
        return d;
    }

    public override void EnemyHit(float _dmgDone, Vector2 _hitDirection, float _hitForce)
    {
        HitFlashFeedback(); //flash white when hit
        health -= _dmgDone;
        audioManager.PlayBaseMobSFX(audioManager.dmgedSound);
        if (!isRecoiling)
        {
            rb.velocity = _hitForce * recoilFactor * _hitDirection;
        }
    }
}
