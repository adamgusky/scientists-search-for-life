using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float runSpeed = 10f;
    [SerializeField] float jumpPower = 20f;
    [SerializeField] float dubJumpPower = 20f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] Vector2 deathKick = new Vector2(20f,20f);
    [SerializeField] GameObject bullet;
    [SerializeField] Transform gun;
    [SerializeField] AudioClip jumpSFX;
    [SerializeField] AudioSource jumpSource;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;
    [SerializeField] private bool jumpBuffered;
    [SerializeField] private float bufferTime = 0.2f;
    [SerializeField] private float bufferTimer;
    [SerializeField] public int level = 0;
    Vector2 moveInput;
    Rigidbody2D myRigidBody;
    Animator myAnimator;
    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeetCollider;


    const float groundCheckRadius = 1.5f;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool wasGrounded;
    float gravityScaleAtStart;
    bool isAlive = true;
    float horizontalValue;
    private bool multipleJumps;
    private int availableJumps = 1;
    public int totalJumps = 1;
    private bool isFacingRight = true;

    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        gravityScaleAtStart = myRigidBody.gravityScale;
    Application.targetFrameRate = 60;
    }

    void Update()
    {
        // if (Input.GetButtonDown("Restart")) {
        //     FindObjectOfType<GameSession>().ResetGameSession();
        // }
        if (isAlive)  {
            if (!isGrounded && coyoteTimeCounter > 0f) {
                coyoteTimeCounter -= Time.deltaTime;
                if (coyoteTimeCounter <= 0)
                {
                    --availableJumps;
                }
            } 
            if (bufferTimer > 0) {
                bufferTimer -= Time.deltaTime;
            } else if (bufferTimer <= 0) {
                jumpBuffered = false;
            }
            

            if (Input.GetButtonDown("Jump") || (jumpBuffered && bufferTimer > 0))
            {
                if (availableJumps > 0)
                {
                    //if our first jump is outside of coyote time
                    if (availableJumps == totalJumps && !isGrounded && coyoteTimeCounter <= 0f)
                    {
                        return;
                    }
                    
                    Jump();
                    jumpBuffered = false;
                    coyoteTimeCounter = 0;
                }
                
                //only buffer if we're on our way down and we don't have any jumps left
                if(!isGrounded && myRigidBody.velocity.y < 0f && !jumpBuffered && availableJumps <= 0) 
                {
                    jumpBuffered = true;
                    bufferTimer = bufferTime;
                }
            }
            Die();
        }

    }

    private void FixedUpdate() {
        if (isAlive)  {
            GroundCheck();
            Run();
            FlipSprite();
            ClimbLadder();
        }
    }

    private void Jump()
    {
        float currentJumpPower = jumpPower;
        // --availableJumps;
        myRigidBody.velocity = Vector2.up * currentJumpPower;
        jumpSource.PlayOneShot(jumpSFX, 0.25f);
    }

    private void GroundCheck()
    {
        isGrounded = false;
        Collider2D[] colliders =  Physics2D.OverlapCircleAll(groundCheck.position, groundCheckRadius, groundLayer);
        if (colliders.Length > 0) {
            myAnimator.SetBool("IsJumping", false);
            isGrounded = true;
            if (!wasGrounded) {
                wasGrounded = true;
                availableJumps = totalJumps;
                //reset coyote time when we land
                coyoteTimeCounter = coyoteTime;
            }
        } else {
            myAnimator.SetBool("IsJumping", true);
            // myAnimator.SetBool("IsIdle", false);
            wasGrounded = false;
        }   
    }

    void Run() {
        Vector2 playerVelocity = new Vector2(Input.GetAxis("Horizontal") * runSpeed, myRigidBody.velocity.y);
        myRigidBody.velocity = playerVelocity;
        horizontalValue = Input.GetAxis("Horizontal");
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;

        // myAnimator.SetBool("IsWalking", playerHasHorizontalSpeed && isGrounded);
    }

    void FlipSprite() {
        if (myRigidBody.velocity.x < 0 && isFacingRight) {
            transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
            isFacingRight = false;
        }
        if (myRigidBody.velocity.x > 0 && !isFacingRight) {
            transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
            isFacingRight = true;
        }
    }
    
    void ClimbLadder() {
        // if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"))) {
        //     myRigidBody.gravityScale = gravityScaleAtStart;
        //     myAnimator.SetBool("IsClimbing", false);
        //     return;
        // }

        // Vector2 climbVelocity = new Vector2(myRigidBody.velocity.x, Input.GetAxis("Vertical") * climbSpeed);
        // myRigidBody.velocity = climbVelocity;
        // myRigidBody.gravityScale = 0f;
    }

    void Die() {
        // if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Skeleton", "Hazards"))) {
            // isAlive = false;
            // myAnimator.SetBool("IsWalking", false);
            // myRigidBody.velocity = deathKick;
            // FindObjectOfType<GameSession>().ProcessPlayerDeath();
        // }
    }
}