using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Player : MonoBehaviour
{
    public static Player currentPlayer { get; private set; }

    // keycode 개수에 맞춰서 작업하자, 또한 keycode는 반드시 묶음으로 쓰자
    [SerializeField] int keycodeCount = 5;
    [SerializeField] private KeyCode leftKey;
    [SerializeField] private KeyCode rightKey;
    [SerializeField] private KeyCode jumpKey;
    [SerializeField] private KeyCode downKey;
    [SerializeField] private KeyCode rollKey;
    [Header("Move")]
    [SerializeField] private float addMoveSpeed;
    [SerializeField] private float maxMoveSpeed;
    [SerializeField] private float moveSpeedInAirRaito;
    [SerializeField] private bool canMove;
    [Header("Jump")]
    [SerializeField] private float jumpScale;
    [SerializeField] private float maxJumpTime;
    [SerializeField] private Vector2 jumpStopCastSize;
    [SerializeField] private Vector2 jumpStopCastOffset;
    [SerializeField] private LayerMask jumpStopLayer;
    [SerializeField] private bool canJump;
    [SerializeField] private bool isJumping;
    [Header("GroundCast")]
    [SerializeField] private Vector2 groundCastSize;
    [SerializeField] private Vector2 groundCastOffset;
    [Header("WallCast")]
    [SerializeField] private Vector2 wallSlideCastSize;
    [SerializeField] private Vector2 wallSlideCastOffset;
    [SerializeField] private Vector2 wallHangCastSize;
    [SerializeField] private Vector2 wallHangCastOffset;
    [SerializeField] private Vector2 wallHangAdjustCastSize;
    [SerializeField] private Vector2 wallHangAdjustCastOffset;
    [SerializeField] private float moveKeyPressTimeToWallExit;
    [SerializeField] private float wallCoolTime;
    [SerializeField] private bool isWallSlide;
    [SerializeField] private bool isWallHang;
    [SerializeField] private bool isWallClimb;
    [Header("Crouch")]
    [SerializeField] private bool isCrouch;
    [SerializeField] private Vector2 crouchColliderOffset;
    [SerializeField] private Vector2 crouchColliderSize;

    private float currentJumpTime;
    private float currentJumpScale;
    private float currentMoveKeyPressTimeToWallClimbExit;
    private float currentWallCoolTime;
    private Coroutine currentWallClimbCoroutine;
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
    }

    private void Update()
    {
        bool isGround = Physics2D.BoxCast((Vector2)transform.position + groundCastOffset, groundCastSize, 0f, Vector2.zero, 0, LayerMask.GetMask("Ground"));
        ani.SetBool("isGround", isGround);

        bool isSomethingOnHead = Physics2D.BoxCast((Vector2)transform.position + jumpStopCastOffset, jumpStopCastSize, 0f, Vector2.zero, 0, jumpStopLayer);

        #region Move
        if (canMove && !isCrouch)
        {
            if (Input.GetKey(leftKey))
            {
                rb.AddForce(new Vector2(-addMoveSpeed * (isGround ? 1 : moveSpeedInAirRaito), 0));
                rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxMoveSpeed, maxMoveSpeed), rb.velocity.y);
                ani.SetBool("isMoveKeyPressed", true);

                sr.flipX = true;
            }
            else if (Input.GetKey(rightKey))
            {
                rb.AddForce(new Vector2(addMoveSpeed * (isGround ? 1 : moveSpeedInAirRaito), 0));
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

        #region Crouch
        if (!isWallSlide && !isWallClimb)
        {
            if ((Input.GetKey(downKey) || isSomethingOnHead) && isGround)
            {
                ani.SetBool("isCrouch", true);
                isCrouch = true;
                bc.offset = crouchColliderOffset;
                bc.size = crouchColliderSize;

                if (Input.GetKey(leftKey))
                    sr.flipX = true;

                if (Input.GetKey(rightKey))
                    sr.flipX = false;
            }
            else
            {
                ani.SetBool("isCrouch", false);
                isCrouch = false;
                bc.offset = normalColliderOffset;
                bc.size = normalColliderSize;
            }
        }
        #endregion

        #region Jump
        if (canJump)
        {
            if (Input.GetKeyDown(jumpKey) && isGround)
            {
                isJumping = true;
                ani.SetTrigger("jump");
                currentJumpScale = jumpScale;
                rb.velocity = new Vector2(rb.velocity.x, currentJumpScale);
            }

            if (Input.GetKey(jumpKey) && isJumping)
            {
                currentJumpScale *= 0.98f;
                rb.velocity = new Vector2(rb.velocity.x, currentJumpScale);

                currentJumpTime += Time.deltaTime;
                if (currentJumpTime > maxJumpTime || isSomethingOnHead)
                {
                    currentJumpTime = 0;
                    isJumping = false;
                }
            }

            if (Input.GetKeyUp(jumpKey))
            {
                currentJumpTime = 0;
                isJumping = false;
            }
        }
        #endregion

        #region Wall
        if (!isWallHang && !isWallSlide && !isWallClimb)
        {
            if (currentWallCoolTime <= 0)
            {
                ani.SetBool("isWallHang", false);
                ani.SetBool("isWallSlide", false);

                var dirWallHangCast = wallHangCastOffset;
                dirWallHangCast.x *= sr.flipX ? -1 : 1;

                var dirWallSlideCast = wallSlideCastOffset;
                dirWallSlideCast.x *= sr.flipX ? -1 : 1;

                bool wallHangCastDetected = Physics2D.BoxCast((Vector2)transform.position + dirWallHangCast, wallHangCastSize, 0f, Vector2.zero, 0f, LayerMask.GetMask("Ground"));
                bool wallSlideCastDetected = Physics2D.BoxCast((Vector2)transform.position + dirWallSlideCast, wallSlideCastSize, 0f, Vector2.zero, 0f, LayerMask.GetMask("Ground"));

                if (Input.GetKey(sr.flipX ? leftKey : rightKey))
                {
                    if (!isGround)
                    {
                        if (wallHangCastDetected && wallSlideCastDetected)
                        {
                            isWallSlide = true;
                            ani.SetTrigger("wallSlide");
                        }

                        if (!wallHangCastDetected && wallSlideCastDetected)
                        {
                            var AdjustedPos = Mathf.CeilToInt(transform.position.y) - 0.5f;
                            transform.position = new Vector3(transform.position.x, AdjustedPos, transform.position.z);

                            var dirWallAdjustHangCast = wallHangAdjustCastOffset;
                            dirWallAdjustHangCast.x *= GetComponent<SpriteRenderer>().flipX ? -1 : 1;
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
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            canMove = false;
            canJump = false;
            isJumping = false;

            if (isWallHang)
            {
                ani.SetBool("isWallHang", true);

                bool isWallHangExit = false;

                if (Input.GetKey(sr.flipX ? rightKey : leftKey))
                {
                    currentMoveKeyPressTimeToWallClimbExit += Time.deltaTime;

                    if (currentMoveKeyPressTimeToWallClimbExit > moveKeyPressTimeToWallExit) isWallHangExit = true;
                }

                if (Input.GetKeyUp(sr.flipX ? rightKey : leftKey))
                {
                    currentMoveKeyPressTimeToWallClimbExit = 0;
                }

                if (Input.GetKeyDown(downKey))
                {
                    isWallHangExit = true;
                }

                if (Input.GetKey(jumpKey))
                {
                    if (!isWallClimb)
                    {
                        isWallClimb = true;
                        isWallHang = false;
                        if (currentWallClimbCoroutine != null)
                            StopCoroutine(currentWallClimbCoroutine);
                        currentWallClimbCoroutine = StartCoroutine(WallClimb());
                        Debug.Log("start");
                    }
                }

                if (isWallHangExit)
                {
                    currentMoveKeyPressTimeToWallClimbExit = 0;
                    canMove = true;
                    canJump = true;
                    isWallHang = false;
                    currentWallCoolTime = wallCoolTime;
                    rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                }
            }

            if (isWallSlide)
            {
                ani.SetBool("isWallSlide", true);

                bool isWallSlideExit = false;

                if (Input.GetKey(sr.flipX ? rightKey : leftKey))
                {
                    currentMoveKeyPressTimeToWallClimbExit += Time.deltaTime;

                    if (currentMoveKeyPressTimeToWallClimbExit > moveKeyPressTimeToWallExit)
                        isWallSlideExit = true;
                }

                if (Input.GetKeyUp(sr.flipX ? rightKey : leftKey))
                {
                    currentMoveKeyPressTimeToWallClimbExit = 0;
                }

                if (Input.GetKeyDown(downKey))
                    isWallSlideExit = true;

                if (Input.GetKeyDown(jumpKey))
                {
                    rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                    ani.SetTrigger("jump");
                    isWallSlide = false;

                    rb.velocity = new Vector2((sr.flipX ? 1 : -1) * maxMoveSpeed, jumpScale * 1.2f);
                    canMove = true;
                    canJump = true;
                    currentWallCoolTime = wallCoolTime;
                    sr.flipX = !sr.flipX;
                }

                if (isWallSlideExit)
                {
                    currentMoveKeyPressTimeToWallClimbExit = 0;
                    canMove = true;
                    canJump = true;
                    isWallSlide = false;
                    currentWallCoolTime = wallCoolTime;
                    rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                }
            }
        }
        #endregion

        ani.SetFloat("xVelocity", rb.velocity.x);
        ani.SetFloat("yVelocity", rb.velocity.y);
    }

    IEnumerator WallClimb()
    {
        canMove = false;
        canJump = false;
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
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((Vector2)transform.position + groundCastOffset, groundCastSize);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube((Vector2)transform.position + jumpStopCastOffset, jumpStopCastSize);

        var dirWallSlideCast = wallSlideCastOffset;
        dirWallSlideCast.x *= GetComponent<SpriteRenderer>().flipX ? -1 : 1;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube((Vector2)transform.position + dirWallSlideCast, wallSlideCastSize);

        var dirWallHangCast = wallHangCastOffset;
        dirWallHangCast.x *= GetComponent<SpriteRenderer>().flipX ? -1 : 1;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube((Vector2)transform.position + dirWallHangCast, wallHangCastSize);

        var dirWallAdjustHangCast = wallHangAdjustCastOffset;
        dirWallAdjustHangCast.x *= GetComponent<SpriteRenderer>().flipX ? -1 : 1;
        Gizmos.color = Color.gray;
        Gizmos.DrawWireCube((Vector2)transform.position + dirWallAdjustHangCast, wallHangAdjustCastSize);
    }
}

#if UNITY_EDITOR
[ExecuteInEditMode, CustomEditor(typeof(Player))]
public class PlayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var iterator = serializedObject.GetIterator();

        iterator.NextVisible(true); //m_script
        EditorGUILayout.PropertyField(iterator);
        EditorGUILayout.Space(10);

        iterator.NextVisible(true); //keycodeCount
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(iterator);
        EditorGUI.EndDisabledGroup();

        var _keycodeCount = iterator.intValue;

        for (int i = 0; i < _keycodeCount; i++)
        {
            iterator.NextVisible(false);
            var currentPropertyName = iterator.name;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(iterator.name);
            if (GUILayout.Button(System.Enum.GetName(typeof(KeyCode), iterator.enumValueFlag), EditorStyles.popup))
            {
                SearchableKeycodeWindow.Open((x) =>
                {
                    serializedObject.FindProperty(currentPropertyName).enumValueFlag = (int)x;
                    serializedObject.ApplyModifiedProperties();
                });
            }
            EditorGUILayout.EndHorizontal();
        }

        while (iterator.NextVisible(false)) EditorGUILayout.PropertyField(iterator);
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
