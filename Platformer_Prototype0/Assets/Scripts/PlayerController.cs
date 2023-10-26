using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform thePlayer;
    [Space(5)]

    [Header("Ui and Camera Stuff")]
    [SerializeField] private UIManager hud;
    private CameraFollowObj _camFocus;
    private float fallSpeedThreshold;
    AudioManager audioManager;
    [Space(5)]

    [Header("Horizontal Movement Settings")]
    [SerializeField] private float walkSpeed = 1;
    [Space(5)]

    [Header("Vertical Movement Settings")]
    [SerializeField] GameObject jumpFX;
    private GameObject currentJumpFX;
    [SerializeField] private float jumpForce = 25;
    private int jumpBufferCounter;
    [SerializeField] private int jumpBufferFrames;
    private float coyoteTimeCounter = 0;
    [SerializeField] private float coyoteTime;
    private int airJumpCounter = 0;
    [SerializeField] private int maxAirJump = 1;
    [Space(5)]

    [Header("Ground Check Settings")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private Vector2 boxSize;
    [SerializeField] private float castDistance;
    [SerializeField] private LayerMask whatIsGround;
    [Space(5)]

    [Header("Dash Settings")]
    [SerializeField] GameObject dashFX;
    private GameObject currentDashFX;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCooldown;
    private bool canDash = true;
    private bool dashed;
    [Space(5)]

    [Header("Attacking")]
    [SerializeField] LayerMask attackableLayer;
    [SerializeField] float timeBetweenAttack = .1f;
    [SerializeField] Transform SideAtkTransform, UpAtkTransform, DownAtkTransform;
    [SerializeField] Vector2 SideAtkArea, UpAtkArea, DownAtkArea;
    [SerializeField] float damage;
    [SerializeField] GameObject slashFX;
    [SerializeField] Vector2 slashSize;
    private bool attack = false;
    float timeSinceAttack;
    bool restoreTime;
    float restoreTimeSpeed;
    [Space(5)]

    [Header("Recoil")]
    [SerializeField] int recoilXSteps = 5;
    [SerializeField] int recoilYSteps = 5;
    [SerializeField] float recoilXSpeed = 100;
    [SerializeField] float recoilYSpeed = 100;
    int stepsXRecoiled, stepsYRecoiled = 0;
    [Space(5)]

    [Header("Health Settings")]
    [SerializeField] GameObject healFX;
    private GameObject currentHealFX;
    [SerializeField] public int health;
    [SerializeField] public int maxHealth;
    [SerializeField] public float hitFlashSpeed;
    [SerializeField] GameObject bloodSpurt;
    [SerializeField] GameObject bloodAbsorb;
    public delegate void OnHealthChangedDelegate();
    [HideInInspector] public OnHealthChangedDelegate onHealthChangedCallback;
    float healTimer;
    [SerializeField] float timeToHeal;
    [Space(5)]

    [Header("Mana Settings")]
    [SerializeField] UnityEngine.UI.Image manaStorage;
    [SerializeField] float mana;
    [SerializeField] float manaCheck;
    [SerializeField] public float manaGain;
    [SerializeField] public float manaDrain;
    [Space(5)]

    [Header(" Spell Settings")]
    [SerializeField] float manaCost = 33f;
    [SerializeField] float timeBetweenCast = 0.5f;
    [SerializeField] float timeSinceCast; 
    [SerializeField] float castOrHealTimer; 
    [SerializeField] float spellDamage;
    [SerializeField] float downSpellForce;

    [SerializeField] GameObject iceSideSpell;
    [SerializeField] GameObject iceProjectile;
    [SerializeField] GameObject poisionUpSpell;
    [SerializeField] GameObject fireDownSpell;

    [SerializeField] public bool unlockIceSpell = false;
    [SerializeField] public bool unlockPoisonSpell = false;
    [SerializeField] public bool unlockFireSpell = false;
    [SerializeField] public bool unlockJumpSpell = false;
    [SerializeField] public bool unlockDashSpell = false;




    [HideInInspector] public PlayerStateList pState;
    Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    //Inputs
    private float xAxis, yAxis;
    private float gravity;
    

    public static PlayerController Instance;

    public void Awake()
    {
        pState = GetComponent<PlayerStateList>();
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        Health = maxHealth;
        pState.alive = true;
        //DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (transform.rotation.y == 0)
        {
            pState.lookingRight = true;
        }
        else if (transform.rotation.y == 180)
        {
            pState.lookingRight = false;
        }
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        gravity = rb.gravityScale;
        castOrHealTimer = 0f;

        audioManager = AudioManager.Instance.GetComponent<AudioManager>();
        fallSpeedThreshold = -9f; //matches cameramanager
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(SideAtkTransform.position, SideAtkArea);
        Gizmos.DrawWireCube(UpAtkTransform.position, UpAtkArea);
        Gizmos.DrawWireCube(DownAtkTransform.position, DownAtkArea);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(groundCheckPoint.position-transform.up*castDistance, boxSize);

    }
    // Update is called once per frame
    void Update()
    {
        if (pState.cutscene)
        {
            xAxis = 0f;
            yAxis = 0f;
            if (Grounded())
            {
                Jump();
                Move();
                rb.velocity = Vector2.zero;
            }
            
            return;
        }
        UpdateJumpVariables();
        RestoreTimeScale();
        if (pState.alive)
        {
            GetInputs();
        }

        if (pState.dashing) return;
        if (pState.alive)
        {
            if (!pState.healing)
            {
                Move();
                Jump();
            }
            Falling();
            StartDash();
            Attack();
            IframeFlash();
            Heal();
            CastSpell();
        }
    }
    private void OnTriggerEnter2D(Collider2D _other) //for up and down cast spell
    {
        if (_other.GetComponent<Enemy>() != null && pState.casting)
        {
            Debug.Log("HIT");
            _other.GetComponent<Enemy>().EnemyHit(spellDamage, (_other.transform.position - transform.position).normalized, -recoilYSpeed);
        }
    }
    private void FixedUpdate()
    {
        if(xAxis > 0 || xAxis < 0)
        {
            TurnCheck();
        }
        if(pState.dashing || pState.casting) return;
        Recoil();
    }

    void GetInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        yAxis = Input.GetAxisRaw("Vertical");
        attack = Input.GetButtonDown("Attack");
        if (Input.GetButton("Cast/Heal"))
        {
            if (Input.GetButton("Cast/Heal"))
            {
                castOrHealTimer += Time.deltaTime;
            }
        }
        else if (!Input.GetButton("Cast/Heal"))
        {
            castOrHealTimer = 0f;
        }
    }

    private void TurnCheck()
    {
        if(xAxis > 0 && !pState.lookingRight)
        {
            Turn();
        }
        else if(xAxis < 0 && pState.lookingRight)
        {
            Turn();
        }
    }
    private void Turn()
    {
        if (pState.lookingRight)
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            pState.lookingRight = !pState.lookingRight;
            //turn cam focus
            CameraFollowObj.Instance.CallCamTurn();
        }

        else
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            pState.lookingRight = !pState.lookingRight;
            //turn cam focus
            CameraFollowObj.Instance.CallCamTurn(); ;
        }
    }



    private void Move()
    {
            rb.velocity = new Vector2(walkSpeed * xAxis, rb.velocity.y);
            anim.SetBool("Walking", rb.velocity.x != 0 && Grounded());
    
    }

    void StartDash()
    {
        if (!unlockDashSpell) { return; }
        if (Input.GetButtonDown("Dash") && canDash && !dashed)
        {
            StartCoroutine(Dash());
            dashed = true;
        }
        if (Grounded())
        {
            dashed = false;
        }
    }

    IEnumerator Dash()
    {
        canDash = false;
        pState.dashing = true;
        audioManager.PlayCharSFX(audioManager.dashSound);
        anim.SetTrigger("Dashing");
        currentDashFX = Instantiate(dashFX, thePlayer);
        rb.gravityScale = 0;
        int _dir = pState.lookingRight ? 1 : -1;
        rb.velocity = new Vector2(_dir * dashSpeed, 0);
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = gravity;
        pState.dashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
        yield return null;
    }

    public IEnumerator WalkIntoNewScene(Vector2 _exitDir, float _delay)
    {
        //If exit direction is upwards
        if (_exitDir.y != 0)
        {
            rb.velocity = jumpForce * _exitDir;
        }

        //If exit direction requires horizontal movement
        if (_exitDir.x != 0)
        {
            xAxis = _exitDir.x > 0 ? 1 : -1;

            Move();
        }

        TurnCheck();
        yield return new WaitForSeconds(_delay);
        pState.cutscene = false;
    }
    void Attack()
    {
        if(timeSinceAttack < timeBetweenAttack * 2f)
        {
            timeSinceAttack += Time.deltaTime;
        }
        if(attack && timeSinceAttack >= timeBetweenAttack)
        {
            timeSinceAttack = 0;
            anim.SetTrigger("Attacking");

            if(yAxis == 0 || yAxis < 0 && Grounded())
            {
                Hit(SideAtkTransform, SideAtkArea, ref pState.recoilingX, Vector2.right, recoilXSpeed);
                audioManager.PlayCharSFX(audioManager.slashSound);
                Instantiate(slashFX, SideAtkTransform);
            }
            else if(yAxis > 0)
            {
                Hit(UpAtkTransform, UpAtkArea, ref pState.recoilingY, Vector2.up, recoilYSpeed);
                SlashEffectAngle(slashFX, 80, UpAtkTransform);
            }
            else if (yAxis < 0)
            {
                Hit(DownAtkTransform, DownAtkArea, ref pState.recoilingY, Vector2.down, recoilYSpeed);
                SlashEffectAngle(slashFX, -90, DownAtkTransform);
            }
        }
    }

    private void Hit(Transform _atkTransform, Vector2 _attackArea, ref bool _recoilBool, Vector2 _recoilDir, float _recoilStrength)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_atkTransform.position, _attackArea, 0, attackableLayer);

        if(objectsToHit.Length > 0) 
        {
            _recoilBool = true;
        }
        for(int i = 0; i < objectsToHit.Length; i++)
        {
            if(objectsToHit[i].CompareTag("Spikes"))
            {
                audioManager.PlayBaseMobSFX(audioManager.spikeBounceSound);
            }
            if (objectsToHit[i].GetComponent<Enemy>() != null)
            {
                objectsToHit[i].GetComponent<Enemy>().EnemyHit(damage, _recoilDir, _recoilStrength);
                if (objectsToHit[i].CompareTag("Enemy"))
                {
                    Mana += manaGain; 
                }
            }
        }
    }

    void SlashEffectAngle(GameObject _slashFX, int _effectAngle, Transform _atkTransform)
    {
        audioManager.PlayCharSFX(audioManager.slashSound);
        _slashFX = Instantiate(_slashFX, _atkTransform);
        _slashFX.transform.eulerAngles = new Vector3(0, 0, _effectAngle);
        _slashFX.transform.localScale = slashSize * transform.localScale.x;

    }

    void Recoil()
    {
        if (pState.recoilingX)
        {
            if (pState.lookingRight)
            {
                rb.velocity = new Vector2(-recoilXSpeed, 0);
            }
            else
            {
                rb.velocity = new Vector2(recoilXSpeed, 0);
            }
        }

        if (pState.recoilingY)
        {
            rb.gravityScale = 0;
            if (yAxis < 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, recoilYSpeed);
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, -recoilYSpeed);
            }
            airJumpCounter = 0;
        }
        else
        {
            rb.gravityScale = gravity;
        }

        if(pState.recoilingX && stepsXRecoiled < recoilXSteps)
        {
            stepsXRecoiled++;
        }
        else
        {
            StopRecoilX();
        }
        if (pState.recoilingY && stepsYRecoiled < recoilYSteps)
        {
            stepsYRecoiled++;
        }
        else
        {
            StopRecoilY();
        }

        if (Grounded())
        {
            StopRecoilY();
        }
    }

    void StopRecoilX()
    {
        stepsXRecoiled = 0;
        pState.recoilingX = false;
    }

    void StopRecoilY()
    {
        stepsYRecoiled = 0;
        pState.recoilingY = false;
    }
    public void TakeDamage(float _dmg)
    {
        if (pState.alive)
        {
            if (pState.dashing)
            {
                StopDash();
            }
            Health -= Mathf.RoundToInt(_dmg);
            audioManager.PlayCharSFX(audioManager.bleedSound);
            if (Health <= 0)
            {
                Health = 0;
                StartCoroutine(Death());
            }
            else
            {
                StartCoroutine(Iframes());
            }
        }
        
    }

    IEnumerator Iframes()
    {
        pState.invincible = true;
        GameObject _bloodSpurtParticles = Instantiate(bloodSpurt, new Vector3(transform.position.x, transform.position.y, transform.position.z -1) , Quaternion.identity);
        Destroy(_bloodSpurtParticles, 1.5f);
        anim.SetTrigger("Hurting");
        yield return new WaitForSeconds(1f);
        pState.invincible = false;
        yield return null;
    }

    void IframeFlash()
    {
        sr.color = pState.invincible ? Color.Lerp(Color.white, Color.black, Mathf.PingPong(Time.time * hitFlashSpeed, 1.0f)) : Color.white;
    }

    void RestoreTimeScale()
    {
        if (restoreTime)
        {
            if (Time.timeScale < 1)
            {
                Time.timeScale += Time.deltaTime * restoreTimeSpeed;
            }
            else
            {
                Time.timeScale = 1;
                restoreTime = false;
            }
        }
    }

    private void StopDash()
    {
        StopCoroutine("Dash"); // Stop the Dash coroutine
        Destroy(currentDashFX);
        rb.velocity = Vector2.zero; // Stop the player's movement
        pState.dashing = false;
    }

    public void HitStopTime(float _newTimeScale, int _restoreSpeed, float _delay)
    {
        restoreTimeSpeed = _restoreSpeed;
        if (_delay > 0)
        {
            StopCoroutine(StartTimeAgain(_delay));
            StartCoroutine(StartTimeAgain(_delay));
        }
        else
        {
            restoreTime = true;
        }
        Time.timeScale = _newTimeScale;
    }

    IEnumerator StartTimeAgain(float _delay)
    {
        restoreTime = true;
        yield return new WaitForSeconds(_delay);
        yield return null;
    }

    IEnumerator Death()
    {
        pState.alive = false;
        rb.velocity = new Vector2 (0, rb.velocity.y);
        Time.timeScale = 1f;
        GameObject _bloodSpurtParticles = Instantiate(bloodSpurt, new Vector3(transform.position.x, transform.position.y, transform.position.z - 1), Quaternion.identity);
        Destroy(_bloodSpurtParticles, 1.5f);
        anim.SetTrigger("Death");

        yield return new WaitForSeconds(0.9f);

        StartCoroutine(UIManager.Instance.ActivateDeathScreen());
    }

    public void Respawned()
    {
        if (!pState.alive)
        {
            pState.alive = true;
            Health = maxHealth;
            anim.Play("Player_Idle");
        }
    }
    public int Health
    {
        get { return health; }
        set
        {
            if (health != value)
            {
                health = Mathf.Clamp(value, 0, maxHealth);
                if (onHealthChangedCallback != null)
                {
                    onHealthChangedCallback.Invoke();
                }
            }
        }
    }

    float Mana
    {
        get { return mana; }
        set
        {
            //if mana stats change
            if (mana != value)
            {
                mana = Mathf.Clamp(value, 0, 1);
                hud.UpdateManaBall(Mana);
            }
        }
    }

    void Heal()
    {
        if (Input.GetButton("Cast/Heal") && castOrHealTimer > 0.25f && Mana > 0 &&
            !pState.jumping && !pState.dashing && Grounded())
        {

            //set animations
            xAxis = 0f;
            yAxis = 0f;
            rb.velocity = new Vector2(0, rb.velocity.y);
            pState.healing = true;
            timeSinceCast = 0;
            anim.SetBool("Healing", true);
            bloodAbsorb.SetActive(true);
            //healing
            Mana -= Time.deltaTime * manaDrain;
            manaCheck += Time.deltaTime * manaDrain;
            if (manaCheck >= 0.33f)
            {
                audioManager.PlayCharSFX(audioManager.healSound);
                Instantiate(healFX, transform);
                Health++;
                manaCheck = 0;
            }
        }
        else if(Input.GetButton("Cast/Heal"))
        {
            pState.healing = false;
            bloodAbsorb.SetActive(false);
            anim.SetBool("Healing", false);
            
            manaCheck = 0;
        }
        else
        {
            pState.healing = false;
            bloodAbsorb.SetActive(false);
            anim.SetBool("Healing", false);
            if (timeSinceCast < timeBetweenCast * 2f)
            {
                timeSinceCast += Time.deltaTime;

            }
            manaCheck = 0;
        }
    }

    void CastSpell()
    {
        if (Input.GetButtonUp("Cast/Heal") && castOrHealTimer <= 0.25f && timeSinceCast >= timeBetweenCast && Mana >= manaCost && !pState.healing)
        {
            pState.casting = true;
            timeSinceCast = 0;
            StartCoroutine(CastCoroutine());
        }
        else if(!pState.healing && timeSinceCast < timeBetweenCast * 2f)
        {
            timeSinceCast += Time.deltaTime;
        }

        if (Grounded())
        {
            if (fireDownSpell.activeInHierarchy)
            {
                pState.casting = false;
            }
            //disable downspell if on the ground
            fireDownSpell.SetActive(false);
        }
        //if down spell is active, force player down until grounded
        if (fireDownSpell.activeInHierarchy)
        {
            rb.velocity += downSpellForce * Vector2.down;
        }
    }
    IEnumerator CastCoroutine()
    {
        anim.SetBool("Casting", true);
        //side cast
        if ((yAxis == 0 || (yAxis < 0 && Grounded())) && unlockIceSpell)
        {
            audioManager.PlayCharSFX(audioManager.sideSpellSound);
            anim.SetTrigger("IceCast");
            Instantiate(iceSideSpell, SideAtkTransform);
            GameObject _iceShard = Instantiate(iceProjectile, SideAtkTransform.position, Quaternion.identity);
            pState.casting = false;
            //flip fireball
            if (!pState.lookingRight)
            {
                _iceShard.transform.eulerAngles = new Vector2(_iceShard.transform.eulerAngles.x, 180);
                //if not facing right, rotate the fireball 180 deg
            }
            pState.recoilingX = true;
            yield return new WaitForSeconds(0.35f);
            anim.SetBool("Casting", false);
            Mana -= manaCost;
        }

        //up cast
        else if (yAxis > 0 && unlockPoisonSpell)
        {
            audioManager.PlayCharSFX(audioManager.upSpellSound);
            anim.SetTrigger("PoisonCast");
            rb.gravityScale = 0f;
            rb.velocity = Vector2.zero;
            Instantiate(poisionUpSpell, transform.position + poisionUpSpell.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(0.35f);
            rb.gravityScale = gravity;
            pState.casting = false;
            anim.SetBool("Casting", false);
            Mana -= manaCost;
        }

        //down cast
        else if (yAxis < 0 && !Grounded() && unlockFireSpell)
        {
            audioManager.PlayCharSFX(audioManager.downSpellSound);
            anim.SetTrigger("FireCast");
            rb.gravityScale = 0f;
            rb.velocity = Vector2.zero;
            fireDownSpell.SetActive(true);
            yield return new WaitForSeconds(0.35f);
            rb.gravityScale = gravity;
            anim.SetBool("Casting", false);
            Mana -= manaCost;
        }
        else
        {
            anim.SetBool("Casting", false);
        }

    }


    public bool Grounded()
    {
        if(Physics2D.BoxCast(groundCheckPoint.position, boxSize, 0, -transform.up, castDistance, whatIsGround))
        {
            return true;
        }
        else { return false;}

        /*if(Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, whatIsGround)
           || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround)
           || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround))
        {
            return true;
        }
        else { return false; }*/
    }



    void Jump()
    {
        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce);
            pState.jumping = true;
        }
        if (!Grounded() && airJumpCounter < maxAirJump && Input.GetButtonDown("Jump") && unlockJumpSpell)
        {
            pState.jumping = true;
            currentJumpFX = Instantiate(jumpFX, thePlayer.position, Quaternion.identity);
            audioManager.PlayCharSFX(audioManager.jumpSound);
            airJumpCounter++;
            rb.velocity = new Vector3(rb.velocity.x, jumpForce);
        }
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 3)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);

            pState.jumping = false;
        }
         

        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("Jumping", !Grounded());

    }

    void Falling()
    {
        if(CameraManager.instance == null)
        {
            return;
        }
        if(rb.velocity.y < fallSpeedThreshold && !CameraManager.instance.IsLerpingYDamping && !CameraManager.instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpYDamping(true);
        }

        if(rb.velocity.y >= 0f && !CameraManager.instance.IsLerpingYDamping && CameraManager.instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpedFromPlayerFalling = false;
            CameraManager.instance.LerpYDamping(false);
        }
    }

    void UpdateJumpVariables()
    {
        if (Grounded())
        {
            pState.jumping = false;
            coyoteTimeCounter = coyoteTime;
            airJumpCounter = 0;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferFrames;
        }
        else
        {
            jumpBufferCounter--;
        }
    }


}
