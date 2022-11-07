using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player currentPlayer { get; private set; }

    [Header("Move")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private bool canMove;
    [Header("Jump")]
    [SerializeField] private float jumpScale;
    [SerializeField] private int jumpCount;
    [SerializeField] private bool isGround;
    [Header("Roll")]
    [SerializeField] private float rollCooltime;
    [SerializeField] private float rollMoveTime;
    [SerializeField] private float rollSpeed;
    [SerializeField] private bool canRoll;
    [Header("GroundCast")]
    [SerializeField] private Vector2 groundCastOffset;
    [SerializeField] private Vector2 groundCastSize;
    [Header("Attack")]
    [SerializeField] private float invincibleTime;
    [SerializeField] private bool isInvincible;
    [Header("Stun")]
    [SerializeField] private bool isStun;
    [Header("Attack")]
    [SerializeField] private Vector2 attackCastOffset;
    [SerializeField] private Vector2 attackCastSize;
    [SerializeField] private bool canAttack;
    [SerializeField] private bool isSecondAttack;
    [SerializeField] private float attackCoolTime;
    [SerializeField] private float secondAttackChangeTime;

    private int currentJumpCount;
    private float currentStunTime;
    private float currentInvincibleTime;
    private float currentAttackCoolTime;

    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator ani;

    private void Awake()
    {
        if (currentPlayer == null)
            currentPlayer = this;

        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        ani = GetComponent<Animator>();

        currentJumpCount = jumpCount;
    }

    private void Update()
    {
        //땅 감지
        isGround = Physics2D.BoxCast((Vector2)transform.position + groundCastOffset, groundCastSize, 0f, Vector2.zero, 0, LayerMask.GetMask("Ground"));

        var attackCastDir = attackCastOffset;
        attackCastDir.x *= sr.flipX ? -1 : 1;
        RaycastHit2D[] enemiesInAttackRange = Physics2D.BoxCastAll((Vector2)transform.position + attackCastDir, attackCastSize, 0, Vector2.zero);

        if (currentStunTime > 0) //기절
        {
            currentStunTime -= Time.deltaTime;
            isStun = true;
        }
        else
        {
            isStun = false;
        }

        if (currentInvincibleTime > 0) // 무적
        {
            currentInvincibleTime -= Time.deltaTime;
            isInvincible = true;
        }
        else
        {
            isInvincible = false;
        }


        if (canMove)
        {
            Vector2 velocity = new Vector2(rb.velocity.x, rb.velocity.y); //속력

            if (Input.GetButton("Horizontal")) //좌우이동
            {
                velocity.x = Input.GetAxisRaw("Horizontal") * moveSpeed;

                if (Input.GetAxisRaw("Horizontal") != 0)
                {
                    sr.flipX = Input.GetAxisRaw("Horizontal") > 0 ? false : true;
                    ani.SetBool("isMove", true);
                }
                else
                {
                    ani.SetBool("isMove", false);
                    velocity.x = 0;
                }
            }
            if (Input.GetButtonUp("Horizontal"))
            {
                ani.SetBool("isMove", false);
                velocity.x = 0;
            }

            if (jumpCount > 0) //점프
            {
                if (isGround)
                {
                    currentJumpCount = jumpCount;
                    if (Input.GetKeyDown(KeyCode.Space)) //땅에서 점프
                    {
                        ani.SetTrigger("jump");
                        velocity.y = jumpScale;
                    }
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.Space)) //공중에서 점프
                    {
                        if (currentJumpCount > 1)
                        {
                            currentJumpCount--;
                            ani.SetTrigger("jump");
                            velocity.y = jumpScale;
                        }
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.LeftShift) && canRoll) //구르기
            {
                StartCoroutine(Roll());
            }

            if (Input.GetMouseButtonDown(0) && canAttack)
            {
                StartCoroutine(FirstAttack());
            }

            rb.velocity = velocity;
            ani.SetFloat("yVelocity", rb.velocity.y);
            ani.SetBool("isGround", isGround);
        }
    }

    IEnumerator Roll()
    {
        canRoll = false;
        canMove = false;
        ani.SetTrigger("roll");
        ani.SetBool("isRoll", true);
        ani.SetBool("isMove", false);
        for (float t = 0; t < rollMoveTime; t += Time.deltaTime)
        {
            rb.velocity = new Vector2((sr.flipX ? -1 : 1) * rollSpeed, rb.velocity.y);
            yield return new WaitForEndOfFrame();
        }
        canMove = true;
        ani.SetBool("isRoll", false);
        rb.velocity = new Vector2(0, rb.velocity.y);
        if (rollMoveTime < rollCooltime)
            yield return new WaitForSeconds(rollCooltime - rollMoveTime);
        canRoll = true;
    }

    IEnumerator FirstAttack()
    {
        canMove = false;
        canAttack = false;
        ani.SetBool("isSecondAttack", false);
        ani.SetBool("isMove", false);
        ani.SetTrigger("attack");
        yield return new WaitForEndOfFrame();
        rb.velocity = new Vector2((sr.flipX ? -1 : 1), rb.velocity.y);
        for(float t = 0.35f; t > 0; t-=Time.deltaTime)
        {
            yield return new WaitForEndOfFrame();
            rb.velocity = new Vector2((sr.flipX ? -1 : 1) * (t / 0.35f) * 2, rb.velocity.y);
        }
        rb.velocity = new Vector2(0 , rb.velocity.y);
        canMove = true;

        bool isSecondAttack = false;
        for(float t = 0; t < secondAttackChangeTime; t += Time.deltaTime)
        {
            if(Input.GetMouseButtonDown(0))
            {
                StartCoroutine(SecondAttact());
                isSecondAttack = true;
                break;
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }

        if (!isSecondAttack)
            canAttack = true;
    }


    IEnumerator SecondAttact()
    {
        canMove = false;
        ani.SetBool("isSecondAttack", true);
        ani.SetBool("isMove", false);
        ani.SetTrigger("attack");
        yield return new WaitForEndOfFrame();
        rb.velocity = new Vector2((sr.flipX ? -1 : 1), rb.velocity.y);

        for(float t = 0.417f; t > 0; t-=Time.deltaTime)
        {
            yield return new WaitForEndOfFrame();
            rb.velocity = new Vector2((sr.flipX ? -1 : 1) * (t / 0.417f), rb.velocity.y);
        }
        rb.velocity = new Vector2(0 , rb.velocity.y);

        yield return new WaitForSeconds(attackCoolTime);
        canAttack = true;
        canMove = true;
    }

    public void Attack()
    {
        Debug.Log("attack!");
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

        var attackCastDir = attackCastOffset;
        attackCastDir.x *= GetComponent<SpriteRenderer>().flipX ? -1 : 1;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube((Vector2)transform.position + attackCastDir, attackCastSize);
    }
}
