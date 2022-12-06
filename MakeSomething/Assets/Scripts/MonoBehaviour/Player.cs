using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player currentPlayer { get; private set; }

    [Header("Move")]
    [SerializeField] private int horizontalKeyRaw;
    [SerializeField] private float addMoveSpeed;
    [SerializeField] private float maxMoveSpeed;
    [SerializeField] private float moveSpeedInAirRaito;
    [SerializeField] private bool canMove;
    [Header("Jump")]
    [SerializeField] private float jumpScale;
    [SerializeField] private float maxJumpTime;
    [SerializeField] private float maxJumpScaleRatio;
    [SerializeField] private bool canJump;
    [SerializeField] private bool isJumping;
    [Header("HeadCast")]
    [SerializeField] private Vector2 headCastSize;
    [SerializeField] private Vector2 headCastOffset;
    [SerializeField] private LayerMask headCastLayer;
    [SerializeField] private bool isSomethingOnHead;
    [Header("GroundCast")]
    [SerializeField] private Vector2 groundCastSize;
    [SerializeField] private Vector2 groundCastOffset;
    [SerializeField] private bool isGround;
    [Header("WallCast")]
    [SerializeField] private Vector2 wallSlideCastSize;
    [SerializeField] private Vector2 wallSlideCastOffset;
    [SerializeField] private Vector2 wallHangCastSize;
    [SerializeField] private Vector2 wallHangCastOffset;
    [SerializeField] private Vector2 wallHangAdjustCastSize;
    [SerializeField] private Vector2 wallHangAdjustCastOffset;
    [SerializeField] private Vector2 wallHangBlockCastSize;
    [SerializeField] private Vector2 wallHangBlockCastOffset;
    [SerializeField] private float moveKeyPressTimeToWallExit;
    [SerializeField] private float wallCoolTime;
    [SerializeField] private bool canWallDetect;
    [SerializeField] private bool isWallSlide;
    [SerializeField] private bool isWallHang;
    [SerializeField] private bool isWallClimb;
    [Header("Crouch")]
    [SerializeField] private float crouchMaxMoveSpeed;
    [SerializeField] private bool canCrouch;
    [SerializeField] private bool isCrouch;
    [SerializeField] private Vector2 crouchColliderOffset;
    [SerializeField] private Vector2 crouchColliderSize;
    [Header("Attack")]
    [SerializeField] private Vector2 attackCastSize;
    [SerializeField] private Vector2 attackCastOffset;
    [SerializeField] private LayerMask attackEnemyLayer;
    [SerializeField] private int attackDamage;
    [SerializeField] private float secondAttackChargeTime;
    [SerializeField] private float attackAddForceScale;
    [SerializeField] private float attackDelay;
    [SerializeField] private bool canAttack;
    [SerializeField] private bool secondAttackCharged;
    [SerializeField] private bool isFirstAttacking;
    [SerializeField] private bool isSecondAttacking;
    [Header("Hp")]
    [SerializeField] private int maxHp;
    [SerializeField] private int currentHp; 

    public int MaxHp { get { return maxHp; } }
    public int CurrentHp { get {return currentHp; } }

    private float currentJumpTime;
    private float currentJumpScale;
    private float currentMoveKeyPressTimeToWallClimbExit;
    private float currentWallCoolTime;
    private float currentSecondAttackChargeTime;
    private bool crouchEnterTrigger;
    private bool crouchExitTrigger;
    private Coroutine wallClimbCoroutine;
    private Coroutine rollCoroutine;
    private Vector2 normalColliderSize;
    private Vector2 normalColliderOffset;

    Rigidbody2D rb;
    SpriteRenderer sr;
    BoxCollider2D bc;
    Animator ani;

    private void Awake()
    {
        currentPlayer = this;

        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        bc = GetComponent<BoxCollider2D>();
        ani = GetComponent<Animator>();

        normalColliderOffset = bc.offset;
        normalColliderSize = bc.size;

        currentHp = maxHp;
    }

    private void Update()
    {
        isGround = Physics2D.BoxCast((Vector2)transform.position + groundCastOffset, groundCastSize, 0f, Vector2.zero, 0, LayerMask.GetMask("Ground"));
        ani.SetBool("isGround", isGround);

        isSomethingOnHead = Physics2D.BoxCast((Vector2)transform.position + headCastOffset, headCastSize, 0f, Vector2.zero, 0, headCastLayer);

        #region Move
        horizontalKeyRaw = 0;

        if (InputManager.GetKey("leftkey"))
            horizontalKeyRaw += -1;

        if (InputManager.GetKey("rightkey"))
            horizontalKeyRaw += 1;

        if (canMove)
        {
            if (horizontalKeyRaw == -1)
            {
                rb.AddForce(new Vector2(-addMoveSpeed * (isGround ? 1 : moveSpeedInAirRaito) * Time.deltaTime, 0));
                rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxMoveSpeed, maxMoveSpeed), rb.velocity.y);
                ani.SetBool("isMoveKeyPressed", true);

                sr.flipX = true;
            }
            else if (horizontalKeyRaw == 1)
            {
                rb.AddForce(new Vector2(addMoveSpeed * (isGround ? 1 : moveSpeedInAirRaito) * Time.deltaTime, 0));
                rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxMoveSpeed, maxMoveSpeed), rb.velocity.y);
                ani.SetBool("isMoveKeyPressed", true);

                sr.flipX = false;
            }
            else
            {
                ani.SetBool("isMoveKeyPressed", false);
            }
        }
        #endregion

        #region Jump
        if (canJump)
        {
            if (InputManager.GetKeyDown("jumpkey") && isGround && !isSomethingOnHead)
            {
                isCrouch = false;
                isJumping = true;
                ani.SetTrigger("jump");
                currentJumpScale = jumpScale;
                rb.velocity = new Vector2(rb.velocity.x, currentJumpScale);
            }

            if (InputManager.GetKey("jumpkey") && isJumping)
            {
                currentJumpScale = jumpScale - (currentJumpTime / maxJumpTime * maxJumpScaleRatio * jumpScale);
                rb.velocity = new Vector2(rb.velocity.x, currentJumpScale);

                currentJumpTime += Time.deltaTime;
                if (currentJumpTime > maxJumpTime || isSomethingOnHead)
                {
                    currentJumpTime = 0;
                    isJumping = false;
                }
            }

            if (InputManager.GetKeyUp("jumpkey"))
            {
                currentJumpTime = 0;
                isJumping = false;
            }
        }
        #endregion

        #region Wall
        if (canWallDetect)
        {
            if (!isWallHang && !isWallSlide && !isWallClimb)
            {
                ani.SetBool("isWallHang", false);
                ani.SetBool("isWallSlide", false);
                if (currentWallCoolTime <= 0)
                {
                    var dirWallHangCast = wallHangCastOffset;
                    dirWallHangCast.x *= sr.flipX ? -1 : 1;

                    var dirWallSlideCast = wallSlideCastOffset;
                    dirWallSlideCast.x *= sr.flipX ? -1 : 1;

                    bool wallHangCastDetected = Physics2D.BoxCast((Vector2)transform.position + dirWallHangCast, wallHangCastSize, 0f, Vector2.zero, 0f, LayerMask.GetMask("Ground"));
                    bool wallSlideCastDetected = Physics2D.BoxCast((Vector2)transform.position + dirWallSlideCast, wallSlideCastSize, 0f, Vector2.zero, 0f, LayerMask.GetMask("Ground"));
                    bool wallHangBlockCastDetected = Physics2D.BoxCast((Vector2)transform.position + wallHangBlockCastOffset, wallHangBlockCastSize, 0, Vector2.zero, 0, LayerMask.GetMask("Ground"));

                    if (horizontalKeyRaw == (sr.flipX ? -1 : 1))
                    {
                        if (!isGround)
                        {
                            if (wallHangCastDetected && wallSlideCastDetected)
                            {
                                if(rollCoroutine != null)
                                    StopCoroutine(rollCoroutine);
                                rb.constraints = RigidbodyConstraints2D.FreezeAll;
                                isWallSlide = true;
                                ani.SetTrigger("wallSlide");
                            }

                            if (!wallHangCastDetected && wallSlideCastDetected && !wallHangBlockCastDetected)
                            {
                                if(rollCoroutine != null)
                                    StopCoroutine(rollCoroutine);

                                rb.constraints = RigidbodyConstraints2D.FreezeAll;

                                var AdjustedPos = Mathf.CeilToInt(transform.position.y) - 0.5f;
                                transform.position = new Vector3(transform.position.x, AdjustedPos, transform.position.z);

                                var dirWallAdjustHangCast = wallHangAdjustCastOffset;
                                dirWallAdjustHangCast.x *= sr.flipX ? -1 : 1;

                                if (!Physics2D.BoxCast((Vector2)transform.position + dirWallAdjustHangCast, wallHangAdjustCastSize, 0f, Vector2.zero, 0f, LayerMask.GetMask("Ground")))
                                    transform.Translate(new Vector3(0, -1, 0));

                                isWallHang = true;
                                ani.SetTrigger("wallHang");
                            }
                        }
                    }
                }
                else
                {
                    currentWallCoolTime -= Time.deltaTime;
                }
            }
            else
            {
                canMove = false;
                canJump = false;
                isJumping = false;
                isCrouch = false;
                canCrouch = false;

                if (isWallHang)
                {
                    ani.SetBool("isWallHang", true);

                    bool isWallHangExit = false;

                    if (horizontalKeyRaw == (sr.flipX ? 1 : -1))
                    {
                        currentMoveKeyPressTimeToWallClimbExit += Time.deltaTime;

                        if (currentMoveKeyPressTimeToWallClimbExit > moveKeyPressTimeToWallExit) isWallHangExit = true;
                    }

                    if (horizontalKeyRaw == 0)
                    {
                        currentMoveKeyPressTimeToWallClimbExit = 0;
                    }

                    if (InputManager.GetKeyDown("downkey"))
                    {
                        isWallHangExit = true;
                    }

                    if (InputManager.GetKey("jumpkey"))
                    {
                        if (!isWallClimb)
                        {
                            isWallClimb = true;
                            isWallHang = false;
                            if (wallClimbCoroutine != null)
                                StopCoroutine(wallClimbCoroutine);
                            wallClimbCoroutine = StartCoroutine(WallClimb());
                        }
                    }

                    if (isWallHangExit)
                    {
                        currentMoveKeyPressTimeToWallClimbExit = 0;
                        canMove = true;
                        canJump = true;
                        canCrouch = true;
                        isWallHang = false;
                        currentWallCoolTime = wallCoolTime;
                        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                    }
                }

                if (isWallSlide)
                {
                    ani.SetBool("isWallSlide", true);

                    bool isWallSlideExit = false;

                    if (horizontalKeyRaw == (sr.flipX ? 1 : -1))
                    {
                        currentMoveKeyPressTimeToWallClimbExit += Time.deltaTime;

                        if (currentMoveKeyPressTimeToWallClimbExit > moveKeyPressTimeToWallExit)
                            isWallSlideExit = true;
                    }

                    if (horizontalKeyRaw == 0)
                    {
                        currentMoveKeyPressTimeToWallClimbExit = 0;
                    }

                    if (InputManager.GetKeyDown("downkey"))
                        isWallSlideExit = true;

                    if (InputManager.GetKeyDown("jumpkey"))
                    {
                        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                        ani.SetTrigger("jump");
                        isWallSlide = false;

                        rb.velocity = new Vector2((sr.flipX ? 1 : -1) * maxMoveSpeed, jumpScale * 1.2f);
                        canMove = true;
                        canJump = true;
                        canCrouch = true;
                        currentWallCoolTime = wallCoolTime;
                        sr.flipX = !sr.flipX;
                    }

                    if (isWallSlideExit)
                    {
                        currentMoveKeyPressTimeToWallClimbExit = 0;
                        canMove = true;
                        canJump = true;
                        canCrouch = true;
                        isWallSlide = false;
                        currentWallCoolTime = wallCoolTime;
                        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                    }
                }
            }
        }
        #endregion

        #region Crouch
        if (canCrouch)
        {
            if (!isWallSlide && !isWallClimb)
            {
                if (isGround)
                {
                    if (isSomethingOnHead)
                    {
                        if (!isCrouch)
                            crouchEnterTrigger = true;
                    }
                    else
                    {
                        if (!InputManager.GetKey("downkey"))
                        {
                            if (isCrouch)
                                crouchExitTrigger = true;
                        }
                    }

                    if (InputManager.GetKey("downkey"))
                    {
                        if (!isCrouch)
                            crouchEnterTrigger = true;
                    }
                    else
                    {
                        if (!isSomethingOnHead)
                        {
                            if (isCrouch)
                                crouchExitTrigger = true;
                        }
                    }
                }
                else
                {
                    if (isCrouch)
                        crouchExitTrigger = true;
                }

                if (crouchEnterTrigger)
                {
                    crouchEnterTrigger = false;

                    isCrouch = true;
                    ani.SetLayerWeight(1, 1);
                    canMove = false;
                    bc.offset = crouchColliderOffset;
                    bc.size = crouchColliderSize;
                    canAttack = false;
                }

                if (crouchExitTrigger)
                {
                    crouchExitTrigger = false;

                    isCrouch = false;
                    ani.SetLayerWeight(1, 0);
                    if (!isWallHang && !isWallSlide)
                        canMove = true;
                    bc.offset = normalColliderOffset;
                    bc.size = normalColliderSize;
                    canAttack = true;
                }

                if (isCrouch)
                {
                    if (horizontalKeyRaw == -1)
                    {
                        rb.AddForce(new Vector2(-addMoveSpeed * (isGround ? 1 : moveSpeedInAirRaito) * Time.deltaTime, 0));
                        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -crouchMaxMoveSpeed, crouchMaxMoveSpeed), rb.velocity.y);
                        ani.SetBool("isMoveKeyPressed", true);

                        sr.flipX = true;
                    }
                    else if (horizontalKeyRaw == 1)
                    {
                        rb.AddForce(new Vector2(addMoveSpeed * (isGround ? 1 : moveSpeedInAirRaito) * Time.deltaTime, 0));
                        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -crouchMaxMoveSpeed, crouchMaxMoveSpeed), rb.velocity.y);
                        ani.SetBool("isMoveKeyPressed", true);

                        sr.flipX = false;
                    }
                    else
                    {
                        ani.SetBool("isMoveKeyPressed", false);
                    }
                }
            }
        }
        #endregion

        #region Attack
        if (currentSecondAttackChargeTime > 0)
        {
            currentSecondAttackChargeTime -= Time.deltaTime;
            secondAttackCharged = true;
        }
        else
        {
            secondAttackCharged = false;
        }

        if (canAttack && !isWallClimb && !isCrouch)
        {
            if (InputManager.GetKeyDown("attackkey"))
            {
                canAttack = false;
                if (!secondAttackCharged)
                {
                    ani.SetBool("isSecondAttack", false);
                    isFirstAttacking = true;
                }
                else
                {
                    ani.SetBool("isSecondAttack", true);
                    isSecondAttacking = true;
                    currentSecondAttackChargeTime = 0;
                }

                StartCoroutine(AttackAnimation());
            }
        }
        #endregion
        ani.SetFloat("xVelocity", rb.velocity.x);
        ani.SetFloat("yVelocity", rb.velocity.y);
    }

    IEnumerator AttackAnimation()
    {
        canMove = false;
        canJump = false;
        canCrouch = false;
        canWallDetect = false;

        isCrouch = false;
        isJumping = false;
        isWallHang = false;
        isWallClimb = false;
        isWallSlide = false;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        rb.velocity *= new Vector2(0.25f, 1);
        ani.SetTrigger("attack");
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        var currentState = ani.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(currentState.length);

        canMove = true;
        canJump = true;
        canCrouch = true;
        canWallDetect = true;

        if (!isSecondAttacking)
            currentSecondAttackChargeTime = secondAttackChargeTime;
        else
            yield return new WaitForSeconds(attackDelay);

        isFirstAttacking = false;
        isSecondAttacking = false;

        canAttack = true;
    }

    public void Damaged(int amount)
    {
        currentHp -= amount;
    }

    public void Attack()
    {
        rb.AddForce(new Vector2((sr.flipX ? -1 : 1) * attackAddForceScale, 0));

        var dirAttackCastOffset = attackCastOffset;
        dirAttackCastOffset.x *= sr.flipX ? -1 : 1;

        RaycastHit2D[] hitEnemy = Physics2D.BoxCastAll((Vector2)transform.position + dirAttackCastOffset, attackCastSize, 0, Vector2.zero, 0, attackEnemyLayer);
        foreach(var enemy in hitEnemy) enemy.collider.GetComponent<BaseEnemy>()?.Damaged(10);    
    }

    IEnumerator WallClimb()
    {
        canMove = false;
        canJump = false;
        canCrouch = false;
        isWallHang = false;

        ani.SetTrigger("wallClimb");
        ani.SetBool("isWallClimb", true);
        var currentState = ani.GetCurrentAnimatorStateInfo(0);

        yield return new WaitForSeconds(currentState.length / currentState.speedMultiplier);
        Vector3 climbPos = transform.position;
        climbPos.x += sr.flipX ? -1 : 1;
        climbPos.y += 1.5f;
        transform.position = climbPos;
        ani.SetBool("isWallClimb", false);

        currentMoveKeyPressTimeToWallClimbExit = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        isWallClimb = false;

        canMove = true;
        canJump = true;
        canCrouch = true;
    }

    private void OnDrawGizmos()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((Vector2)transform.position + groundCastOffset, groundCastSize);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube((Vector2)transform.position + headCastOffset, headCastSize);

        var dirWallSlideCast = wallSlideCastOffset;
        dirWallSlideCast.x *= spriteRenderer.flipX ? -1 : 1;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube((Vector2)transform.position + dirWallSlideCast, wallSlideCastSize);

        var dirWallHangCast = wallHangCastOffset;
        dirWallHangCast.x *= spriteRenderer.flipX ? -1 : 1;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube((Vector2)transform.position + dirWallHangCast, wallHangCastSize);

        var dirWallAdjustHangCast = wallHangAdjustCastOffset;
        dirWallAdjustHangCast.x *= spriteRenderer.flipX ? -1 : 1;
        Gizmos.color = Color.gray;
        Gizmos.DrawWireCube((Vector2)transform.position + dirWallAdjustHangCast, wallHangAdjustCastSize);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube((Vector2)transform.position + wallHangBlockCastOffset, wallHangBlockCastSize);

        var dirAttackCastOffset = attackCastOffset;
        dirAttackCastOffset.x *= spriteRenderer.flipX ? -1 : 1;

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube((Vector2)transform.position + dirAttackCastOffset, attackCastSize);
    }
}
