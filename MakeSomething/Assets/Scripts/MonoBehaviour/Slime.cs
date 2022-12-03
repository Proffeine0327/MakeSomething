using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum SlimeType
{
    red,
    green,
    blue,
}

public class Slime : BaseEnemy
{
    [SerializeField] private SlimeType slimeType;
    [SerializeField] private GameObject hpBarPrefeb;
    [Header("Jump")]
    [SerializeField] private float moveScale;
    [SerializeField] private float jumpScale;
    [SerializeField] private float jumpWaitTime;
    [SerializeField] private bool canJump;
    [SerializeField] private bool isJumping;
    [Header("groundCast")]
    [SerializeField] private Vector2 groundCastSize;
    [SerializeField] private Vector2 groundCastOffset;
    [SerializeField] private bool isGround;
    [Header("Detect")]
    [SerializeField] private float detectRange;
    [SerializeField] private LayerMask detectLayer;
    [SerializeField] private bool isPlayerDetected;
    [Header("Attack")]
    [SerializeField] private int attackDamage;
    [SerializeField] private float stopRange;
    [SerializeField] private float attackWaitTime;
    [SerializeField] private Vector2 attackCastSize;
    [SerializeField] private Vector2 attackCastOffset;
    [SerializeField] private bool canAttack;
    [SerializeField] private bool isAttacking;
    [Header("Damaged")]
    [SerializeField] private GameObject hitParticle;
    [SerializeField] private GameObject attackParticle;

    private bool stunExitTrigger;
    private RaycastHit2D detectInfo;
    private Coroutine currentJumpCoroutine;
    private Coroutine currentAttackCoroutine;
    private UI_HpBar hpbar;

    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator ani;

    private void Awake()
    {
        currentHp = maxHp;

        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        ani = GetComponent<Animator>();

        ani.SetLayerWeight((int)slimeType, 1);
    }

    private void Start() 
    {
        hpbar = MultipleObjectPool.GetObject("hpbar").GetComponent<UI_HpBar>();
        hpbar.transform.SetParent(GameObject.Find("UI_HpBarShow").transform);
        hpbar.SetEnemy(this);
    }

    void Update()
    {
        if(currentHp <= 0)
        {
            Destroy(gameObject);
            MultipleObjectPool.PoolObject("hpbar", hpbar.gameObject);
            return;
        }

        isGround = Physics2D.BoxCast((Vector2)transform.position + groundCastOffset, groundCastSize, 0f, Vector2.zero, 0, LayerMask.GetMask("Ground"));
        ani.SetBool("isGround", isGround);
        ani.SetFloat("yVelocity", rb.velocity.y);

        if(sr.color.a < 1)
        {
            var color = sr.color;
            color.a += Time.deltaTime;
            sr.color = color;
        }

        if (!isPlayerDetected)
        {
            var relativePos = Player.currentPlayer.transform.position - transform.position;
            detectInfo = Physics2D.Raycast(transform.position, relativePos.normalized, detectRange, detectLayer);
            if (detectInfo.collider != null && detectInfo.collider.tag.Equals("Player"))
                isPlayerDetected = true;
        }

        if (isPlayerDetected)
        {
            if (!Physics2D.OverlapCircle(transform.position, stopRange, LayerMask.GetMask("Player"))) //jump
            {
                if (canJump)
                {
                    isJumping = true;
                    canJump = false;
                    canAttack = false;
                    
                    currentJumpCoroutine = StartCoroutine(Jump());
                }
            }
            else //attack
            {
                if(canAttack)
                {
                    isAttacking = true;
                    canAttack = false;
                    canJump = false;

                    currentJumpCoroutine = StartCoroutine(AttackAnimation());
                }
            }
        }
    }

    IEnumerator Jump()
    {
        var relativePlayerPosX = Player.currentPlayer.transform.position.x - transform.position.x;
        sr.flipX = relativePlayerPosX > 0 ? false : true;

        ani.SetTrigger("jump");
        yield return new WaitForEndOfFrame();

        var currentState = ani.GetCurrentAnimatorStateInfo((int)slimeType);
        yield return new WaitForSeconds(currentState.length);

        rb.velocity = new Vector2(moveScale * (relativePlayerPosX > 0 ? 1 : -1), jumpScale);

        yield return new WaitForSeconds(jumpWaitTime);

        bool isGroundInJumpWaitTime = true;
        while (!isGround)
        {
            yield return new WaitForEndOfFrame();
            isGroundInJumpWaitTime = false;
        }

        if (!isGroundInJumpWaitTime)
        {
            yield return new WaitForEndOfFrame();
            currentState = ani.GetCurrentAnimatorStateInfo((int)slimeType);
            yield return new WaitForSeconds(currentState.length);
        }
        isJumping = false;
        canJump = true;
        canAttack = true;
    }

    IEnumerator AttackAnimation()
    {
        var relativePlayerPosX = Player.currentPlayer.transform.position.x - transform.position.x;
        sr.flipX = relativePlayerPosX > 0 ? false : true;

        ani.SetTrigger("attack");
        yield return new WaitForEndOfFrame();

        var currentState = ani.GetCurrentAnimatorStateInfo((int)slimeType);
        yield return new WaitForSeconds(currentState.length);

        yield return new WaitForSeconds(attackWaitTime);

        isAttacking = false;
        canJump = true;
        canAttack = true;
    }

    public override void Damaged(int amount)
    {
        currentHp -= amount;

        var color = sr.color;
        color.a = 0.5f;
        sr.color = color;
        ParticleScript.SpawnParticle(hitParticle,transform.position, Quaternion.identity, 1);
    }

    public override void Attack()
    {
        var dirAttackCastOffset = attackCastOffset;
        dirAttackCastOffset.x *= sr.flipX ? -1 : 1;

        ParticleScript.SpawnParticle(attackParticle, transform.position + new Vector3(0.943f * (sr.flipX ? -1 : 1),-0.054f,0.00771f), Quaternion.Euler(0,0,-12), 0.07f, sr.flipX);
        if(Physics2D.BoxCast((Vector2)transform.position + dirAttackCastOffset, attackCastSize, 0f, Vector2.zero, 0, LayerMask.GetMask("Player")))
        {
            Player.currentPlayer.Damaged(attackDamage);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((Vector2)transform.position + groundCastOffset, groundCastSize);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, stopRange);

        var dirAttackCastOffset = attackCastOffset;
        dirAttackCastOffset.x *= GetComponent<SpriteRenderer>().flipX ? -1 : 1;
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube((Vector2)transform.position + dirAttackCastOffset, attackCastSize);
    }
}
