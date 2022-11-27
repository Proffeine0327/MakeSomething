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
    [SerializeField] private int maxHp;
    [SerializeField] private SlimeType slimeType;
    [Header("Stun")]
    [SerializeField] private bool isStun;
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

    private int currentHp;
    private float currentStunTime = 0;
    private bool stunExitTrigger;
    private RaycastHit2D detectInfo;
    private Coroutine currentJumpCoroutine;
    private Coroutine currentAttackCoroutine;

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

    void Update()
    {
        isGround = Physics2D.BoxCast((Vector2)transform.position + groundCastOffset, groundCastSize, 0f, Vector2.zero, 0, LayerMask.GetMask("Ground"));
        ani.SetBool("isGround", isGround);
        ani.SetFloat("yVelocity", rb.velocity.y);

        if (currentStunTime > 0)
        {
            stunExitTrigger = true;

            currentStunTime -= Time.deltaTime;
            isStun = true;

            if(currentJumpCoroutine != null)
                StopCoroutine(currentJumpCoroutine);
            if(currentAttackCoroutine != null)
                StopCoroutine(currentAttackCoroutine);
            canJump = false;
            canAttack = false;
        }
        else
        {
            isStun = false;

            if(stunExitTrigger)
            {
                stunExitTrigger = false;
                canJump = true;
                canAttack = false;
            }
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
        Debug.Log("Jump!");

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
        Debug.Log("Ground!");

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
        Stun(0.3f);
        currentHp -= amount;
    }

    public override void Attack()
    {
        var dirAttackCastOffset = attackCastOffset;
        dirAttackCastOffset.x *= sr.flipX ? -1 : 1;

        if(Physics2D.BoxCast((Vector2)transform.position + dirAttackCastOffset, attackCastSize, 0f, Vector2.zero, 0, LayerMask.GetMask("Player")))
        {
            Player.currentPlayer.Damaged(attackDamage);
        }
    }

    public void Stun(float time)
    {
        if (currentStunTime < time)
            currentStunTime = time;
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
