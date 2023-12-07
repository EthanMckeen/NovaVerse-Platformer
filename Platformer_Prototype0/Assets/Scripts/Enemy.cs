using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("HEALTH")]
    [SerializeField] protected float health;
    [SerializeField] protected Material matFlash;
    [Space(5)]
    [Header("Basic Enemy Settings")]
    [SerializeField] protected float recoilLength;
    [SerializeField] protected float recoilFactor;
    [SerializeField] protected bool isRecoiling = true;
    [SerializeField] protected bool canRecoil;
    [SerializeField] protected PlayerController player;
    [SerializeField] public float speed;
    [SerializeField] public float damage;
    [SerializeField] protected GameObject yellowBlood;
    [Space(5)]
    private float hitFlashtime = 0.1f;

    protected float recoilTimer;
    [HideInInspector] public Rigidbody2D rb;
    protected SpriteRenderer sr;
    public Animator anim;
    protected AudioManager audioManager;
    public Material matDefault;
    protected enum EnemyStates
    {
        //Crawler
        Crawler_Idle,
        Crawler_Flip,

        //Flyer
        Flyer_Idle,
        Flyer_Chase,
        Flyer_Stun,
        Flyer_Death,

        //Charger
        Charger_Idle,
        Charger_Suprised,
        Charger_Charge,

        //Final Boss
        FB_Stage1,
        FB_Stage2,
        FB_Stage3,
        FB_Stage4,

    }
    protected EnemyStates currentEnemyState;

    protected virtual EnemyStates GetCurrentEnemyState
    {
        get { return currentEnemyState; }
        set
        {
            if(currentEnemyState != value)
            {
                currentEnemyState = value;
                ChangeCurrentAnimation();
            }
        }
    }
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        matDefault = sr.material;
        player = PlayerController.Instance;
        audioManager = AudioManager.Instance.GetComponent<AudioManager>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {

        if (isRecoiling)
        {
            if(recoilTimer < recoilLength)
            {
                recoilTimer += Time.deltaTime;
            }
            else
            {
                isRecoiling = false;
                recoilTimer = 0;
            }
        }
        else
        {
            UpdateEnemyStates();
        }
    }
    public virtual void HitFlashFeedback()
    {
        sr.material = matFlash; 
        Invoke("ResetMaterial", hitFlashtime);
    }
    public virtual void ResetMaterial()
    {
        sr.material = matDefault;
    }

    public virtual void EnemyHit(float _dmgDone, Vector2 _hitDirection, float _hitForce)
    {
        HitFlashFeedback(); //flash white when hit
        health -= _dmgDone;
        audioManager.PlayBaseMobSFX(audioManager.dmgedSound);
        GameObject _yellowBlood = Instantiate(yellowBlood, transform.position, Quaternion.identity);
        Destroy(_yellowBlood, 4.4f);
        if (!isRecoiling && canRecoil)
        {
            rb.velocity = _hitForce * recoilFactor * _hitDirection;
        }
    }

    protected virtual void OnCollisionStay2D(Collision2D _other)
    {
        if (_other.gameObject.CompareTag("Player") && !PlayerController.Instance.pState.invincible && health > 0)
        {
            Attack();
            if (PlayerController.Instance.pState.alive)
            {
                PlayerController.Instance.HitStopTime(0, 5, 0.5f);
            }
        }
    }

    protected virtual void Death(float _destroyTime)
    {
        Destroy(gameObject, _destroyTime);
    }
    protected virtual void UpdateEnemyStates() { }

    protected virtual void ChangeCurrentAnimation() { }

    protected void ChangeState(EnemyStates _newState) 
    {
        GetCurrentEnemyState = _newState;
    }
    public virtual void Attack()
    {

        PlayerController.Instance.TakeDamage(damage);

    }
}
