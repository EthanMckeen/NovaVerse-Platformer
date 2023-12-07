using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBoss : Enemy
{
    public static FinalBoss Instance;

    public bool cutscene = true;
    [Header("Attacking")]
    [SerializeField] GameObject slashFX;
    [SerializeField] public Transform SideAtkTransform, UpAtkTransform, DownAtkTransform;
    [SerializeField] public Vector2 SideAtkArea, UpAtkArea, DownAtkArea;
    [SerializeField] private Vector2 slashSize;
    public float attackRange;
    public float attackTimer;
    [Space(5)]

    [Header("Ground Check Settings")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private Vector2 boxSize;
    [SerializeField] private float castDistance;
    [SerializeField] private LayerMask whatIsGround;
    [Space(5)]

    int hitCounter;
    bool stunned, canStun;
    bool alive;

    [HideInInspector] public float runSpeed;
    [HideInInspector] public bool facingRight;

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
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(SideAtkTransform.position, SideAtkArea);
        Gizmos.DrawWireCube(UpAtkTransform.position, UpAtkArea);
        Gizmos.DrawWireCube(DownAtkTransform.position, DownAtkArea);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(groundCheckPoint.position - transform.up * castDistance, boxSize);

    }
    

    // Start is called before the first frame update
    protected override void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
        matDefault = sr.material;
        player = PlayerController.Instance;
        audioManager = AudioManager.Instance.GetComponent<AudioManager>();
        ChangeState(EnemyStates.FB_Stage1);
        alive = true;
        attackCountdown = attackTimer;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (!cutscene)
        {
            base.Update();
                    if(health <= 0 && alive)
                    {
                        Death(0);
                    }
                        if (!attacking)
                        {
                            attackCountdown -= Time.deltaTime;
                        }
                        if (stunned)
                        {
                            rb.velocity = Vector2.zero;
                        }
        }
        else
        {
            anim.Play("Boss_idle");
        }
        
        
    }

    public void Flip()
    {
        if (PlayerController.Instance.transform.position.x < transform.position.x && transform.localScale.x > 0)
        {
            transform.eulerAngles = new Vector2(transform.eulerAngles.x, 180);
            facingRight = false;
        }
        else
        {
            transform.eulerAngles = new Vector2(transform.eulerAngles.x, 0);
            facingRight = true;
        }
    }

    protected override void UpdateEnemyStates()
    {
        if (PlayerController.Instance != null)
        {
            switch (GetCurrentEnemyState)
            {
                case EnemyStates.FB_Stage1:
                    canStun = true;
                    attackTimer = 6;
                    runSpeed = speed;
                    break;
                case EnemyStates.FB_Stage2:
                    canStun = true;
                    attackTimer = 5;
                    break;
                case EnemyStates.FB_Stage3:
                    attackTimer = 8;
                    canStun = false;
                    break;
                case EnemyStates.FB_Stage4:
                    attackTimer = 10;
                    runSpeed = speed / 2;
                    canStun = false;
                    break;
            }
        }
    }

    public bool Grounded()
    {
        if (Physics2D.BoxCast(groundCheckPoint.position, boxSize, 0, -transform.up, castDistance, whatIsGround))
        {
            return true;
        }
        else { return false; }
    }

    protected override void OnCollisionStay2D(Collision2D _other)
    {

    }
    #region attacking

    #region varibles
    [HideInInspector] public bool attacking;
    [HideInInspector] public float attackCountdown;
    [HideInInspector] public bool parrying = false;
   
    [HideInInspector] public Vector2 moveToPosition;
    [HideInInspector] public bool diveAttack;
    public GameObject divingCollider;
    public GameObject bounceCollider;
    public GameObject pillar;
    public GameObject dashFX;

    [HideInInspector] public bool barrageAttack;
    public GameObject barrageIcicle;
    [HideInInspector] public bool outbreakAttack;

    [HideInInspector] public bool bounceAttack;
    [HideInInspector] public float rotationDirectionToTarget;
    [HideInInspector] public int bounceCount;



    #endregion

    #region Control
    public void AttackHandler()
    {
        //Debug.Log(currentEnemyState);
        if(currentEnemyState == EnemyStates.FB_Stage1)
        {
            if(Vector2.Distance(PlayerController.Instance.transform.position, rb.position) <= attackRange)
            {
                StartCoroutine(TripleSlash());
            }
            else
            {              
                    StartCoroutine(Lunge());          
            }
        }
        if (currentEnemyState == EnemyStates.FB_Stage2)
        {
            if (Vector2.Distance(PlayerController.Instance.transform.position, rb.position) <= attackRange)
            {
                StartCoroutine(TripleSlash());
            }
            else
            {
                int _attackChosen = Random.Range(1, 3);
                if (_attackChosen == 1)
                {
                    StartCoroutine(Lunge());
                }
                if (_attackChosen == 2)
                {
                    DiveAttackJump();
                }
                if (_attackChosen == 3)
                {
                    BarrageBendDown();
                }
            }
        }
        if (currentEnemyState == EnemyStates.FB_Stage3)
        {
            if (Vector2.Distance(PlayerController.Instance.transform.position, rb.position) <= attackRange)
            {
                StartCoroutine(TripleSlash());
            }
            else
            {
                int _attackChosen = Random.Range(1, 4);
                if (_attackChosen == 1)
                {
                    OutBreakBendDown();
                }
                if (_attackChosen == 2)
                {
                    DiveAttackJump();
                }
                if (_attackChosen == 3)
                {
                    BarrageBendDown();
                }
                if (_attackChosen == 4)
                {
                    BounceAttack();
                }
            }
        }
        if (currentEnemyState == EnemyStates.FB_Stage4)
        {
            if (Vector2.Distance(PlayerController.Instance.transform.position, rb.position) <= attackRange)
            {
                StartCoroutine(Slash());
            }
            else
            {
                BounceAttack();
            }
        }
    }
    public void ResetAllAttacks()
    {
        attacking = false;

        StopCoroutine(TripleSlash());
        StopCoroutine(Lunge()); 
        StopCoroutine(Parry());
        StopCoroutine(Slash());

        
        diveAttack = false;
        barrageAttack = false;
        outbreakAttack = false;
        bounceAttack = false;
       
    }
    #endregion

    #region Stage 1
    IEnumerator TripleSlash()
    {
        attacking = true;
        rb.velocity = Vector2.zero;

        anim.SetTrigger("Slash");
        yield return new WaitForSeconds(.5f);
        anim.ResetTrigger("Slash");
        

        anim.SetTrigger("Slash2");
        yield return new WaitForSeconds(1f);  
        anim.ResetTrigger("Slash2");


        anim.SetTrigger("Slash3");
        yield return new WaitForSeconds(1f);  
        anim.ResetTrigger("Slash3");

        ResetAllAttacks();
        yield return null;
    }

    public void CreateDashFX()
    {
        audioManager.PlayCharSFX(audioManager.dashSound);
        Instantiate(dashFX, SideAtkTransform);
    }

    public void SlashAngle()
    {
        if(PlayerController.Instance.transform.position.y < transform.position.y +  2f &&
            PlayerController.Instance.transform.position.y > transform.position.y - 2f
            &&
            (PlayerController.Instance.transform.position.x > transform.position.x  || 
           PlayerController.Instance.transform.position.x < transform.position.x))
        {
            Instantiate(slashFX, SideAtkTransform);
            audioManager.PlayCharSFX(audioManager.slashSound);

        }
        else if (PlayerController.Instance.transform.position.y > transform.position.y)
        {
            SlashEffectAngle(slashFX, 80, UpAtkTransform);
            audioManager.PlayCharSFX(audioManager.slashSound);
        }
        else if (PlayerController.Instance.transform.position.y < transform.position.y)
        {
            SlashEffectAngle(slashFX, -90, DownAtkTransform);
            audioManager.PlayCharSFX(audioManager.slashSound);
        }

    }

    void SlashEffectAngle(GameObject _slashFX, int _effectAngle, Transform _atkTransform)
    {
        _slashFX = Instantiate(_slashFX, _atkTransform);
        _slashFX.transform.eulerAngles = new Vector3(0, 0, _effectAngle);
        _slashFX.transform.localScale = slashSize * transform.localScale.x;

    }



    IEnumerator Lunge()
    {
        Flip();
        attacking = true;

        anim.SetBool("Lunge", true);
        yield return new WaitForSeconds(.2f);
        anim.SetBool("Lunge", false);

   
        ResetAllAttacks();
    }

    IEnumerator Parry()
    {
        attacking = true;
        rb.velocity = Vector2.zero;
        anim.SetBool("Parry", true);
        yield return new WaitForSeconds(.5f);
        anim.SetBool("Parry", false);
        parrying = false;

        ResetAllAttacks();
    }

    IEnumerator Parry2()
    {
        attacking = true;
        rb.velocity = Vector2.zero;
        anim.SetTrigger("Parry2");
        PlayerController.Instance.pState.recoilingX = false;
        PlayerController.Instance.pState.recoilingY = false;
        audioManager.PlayBaseMobSFX(audioManager.spikeBounceSound);
        PlayerController.Instance.timeSinceAttack = -1f;
        yield return new WaitForSeconds(.2f);
        anim.ResetTrigger("Parry2");
        parrying = false;

        ResetAllAttacks();
    }

    IEnumerator Slash()
    {
        attacking = true;
        rb.velocity = Vector2.zero;

        anim.SetTrigger("Slash3");
        yield return new WaitForSeconds(.5f);
        anim.ResetTrigger("Slash3");

        ResetAllAttacks();
        yield return null;
    }
    #endregion
    #region Stage 2

    void DiveAttackJump()
    {
        attacking = true;
        moveToPosition = new Vector2(PlayerController.Instance.transform.position.x, rb.transform.position.y + 10);
        diveAttack = true;
        anim.SetBool("Jump", true);
    }

    public void Dive()
    {
        anim.SetBool("Dive", true);
        anim.SetBool("Jump", false);

    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if(_other.GetComponent<PlayerController>() != null && (diveAttack || bounceAttack))
                {
            _other.GetComponent<PlayerController>().TakeDamage(damage * 2);
            PlayerController.Instance.pState.recoilingX = true;
        }
    }

    public void DivingPillars()
    {
        Vector2 _impactPoint = groundCheckPoint.position;
        float _spawnDistance = 5;

        for (int i = 0; i < 10; i++)
        {
            Vector2 _pillarSpawnPointRight = _impactPoint + new Vector2(_spawnDistance, 0);
            Vector2 _pillarSpawnPointLeft = _impactPoint - new Vector2(_spawnDistance, 0);
            Instantiate(pillar, _pillarSpawnPointRight, Quaternion.Euler(0, 0, 0));
            Instantiate(pillar, _pillarSpawnPointLeft, Quaternion.Euler(0, 0, 0));

            _spawnDistance += 5;
        }
        ResetAllAttacks();
    }

    void BarrageBendDown()
    {
        attacking = true;
        rb.velocity = Vector2.zero;
        barrageAttack = true;
        anim.SetTrigger("BendDown");
    }

    public IEnumerator Barrage()
    {
        canRecoil = false;
        rb.velocity = Vector2.zero;

        float _currentAngle = 30f;
        for(int i = 0; i < 20; i++)
        {
            GameObject _projectile = Instantiate(barrageIcicle,transform.position, Quaternion.Euler(0, 0, _currentAngle));
            audioManager.PlayCharSFX(audioManager.sideSpellSound);
            if (facingRight)
            {
                _projectile.transform.eulerAngles = new Vector3(_projectile.transform.eulerAngles.x, 0, _currentAngle + 45);
                if( i < 10)
                {
                     _currentAngle -= 5f;
                }
                else
                {
                    _currentAngle += 5f;
                }
                
            }
            else
            {
                _projectile.transform.eulerAngles = new Vector3(_projectile.transform.eulerAngles.x, 180, _currentAngle);
                if (i < 10)
                {
                    _currentAngle += 5f;
                }
                else
                {
                    _currentAngle -= 5f;
                }
            }
       
            yield return new WaitForSeconds(0.3f);
        }
        yield return new WaitForSeconds(0.1f);
        anim.SetBool("Cast", false);
        ResetAllAttacks();
        canRecoil = true;
    }

    #endregion
    #region Stage 3

    void OutBreakBendDown()
    {
        attacking = true;
        rb.velocity = Vector2.zero;
        moveToPosition = new Vector2(transform.position.x, rb.position.y + 5);
        outbreakAttack = true;
        anim.SetTrigger("BendDown");
    }

    public IEnumerator Outbreak()
    {
        yield return new WaitForSeconds(.1f);
        anim.SetBool("Cast", true);

        rb.velocity = Vector2.zero;
        for(int i = 0; i <30; i++)
        {

            Instantiate(barrageIcicle, transform.position, Quaternion.Euler(0, 0, Random.Range(85, 145))); //downwards
            Instantiate(barrageIcicle, transform.position, Quaternion.Euler(0, 0, Random.Range(25, 85))); //diagonaly right
            Instantiate(barrageIcicle, transform.position, Quaternion.Euler(0, 0, Random.Range(245, 295))); //diagonaly left
            audioManager.PlayCharSFX(audioManager.sideSpellSound);

            yield return new WaitForSeconds(0.2f);
        }
        yield return new WaitForSeconds(0.1f);
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.velocity = new Vector2(rb.velocity.x, -10);
        yield return new WaitForSeconds(0.1f);
        anim.SetBool("Cast", false);
        ResetAllAttacks();
    }

    void BounceAttack()
    {
        attacking = true;
        bounceCount = Random.Range(2, 5);
        BounceBendDown();
    }

    int _bounces = 0;

    public void CheckBounce()
    {
        if(_bounces < bounceCount - 1)
        {
            _bounces++;
            BounceBendDown();
        }
        else
        {
            _bounces = 0;
            anim.Play("Boss_walk");
        }
    }

    public void BounceBendDown()
    {
        rb.velocity = Vector2.zero;
        moveToPosition = new Vector2(transform.position.x, (rb.position.y + 10));
        bounceAttack = true;
        anim.SetTrigger("BendDown");
    }

    public void CalcTargetAng()
    {
        Vector3 _directionToTarget = (PlayerController.Instance.transform.position - transform.position).normalized;

        float _angleOfTarget = Mathf.Atan2(_directionToTarget.y, _directionToTarget.x) * Mathf.Rad2Deg;
        rotationDirectionToTarget = _angleOfTarget;
    }


    #endregion

    #endregion

    public override void EnemyHit(float _dmgDone, Vector2 _hitDirection, float _hitForce)
    {
        if (!stunned)
        {
            if (!parrying)
            {
                if (canStun)
                {
                    hitCounter++;
                    if(hitCounter >= 13)
                    {
                        ResetAllAttacks();
                        StartCoroutine(Stunned());
                    }
                }
                base.EnemyHit(_dmgDone, _hitDirection, _hitForce);
                if (currentEnemyState != EnemyStates.FB_Stage4 && !barrageAttack)
                {
                    ResetAllAttacks();
                    StartCoroutine(Parry());
                }

            }
            else
            {
                StartCoroutine(Parry2());
                ResetAllAttacks();
                StartCoroutine(Slash());
            }

            if(health > 75)
            {
                ChangeState(EnemyStates.FB_Stage1);
            }else if(health <= 75 && health > 50)
            {
                ChangeState(EnemyStates.FB_Stage2);
            }else if(health <= 50 && health > 25)
            {
                ChangeState(EnemyStates.FB_Stage3);
            }
            else if (health <= 25 )
            {
                ChangeState(EnemyStates.FB_Stage4);
            }
            else if (health <= 0)
            {
                Death(0);
            }
        }
        else
        {
            base.EnemyHit(_dmgDone, _hitDirection, _hitForce);
            StopCoroutine(Stunned());
            anim.SetBool("Stunned", false);
            stunned = false;

        }
    }

    public IEnumerator Stunned()
    {
        stunned = true;
        hitCounter = 0;
        anim.SetBool("Stunned", true);

        yield return new WaitForSeconds(6f);
        anim.SetBool("Stunned", false);
        stunned = false;
        yield return null;
    }

    protected override void Death(float _destroyTime)
    {
        ResetAllAttacks();
        alive = false;
        rb.velocity = new Vector2(rb.velocity.x, -25);
        anim.SetTrigger("Die");
    }

    public void DestroyAfterDeath()
    {
        Destroy(gameObject);
    }
}
