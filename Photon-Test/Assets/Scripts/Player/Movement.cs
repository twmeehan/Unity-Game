﻿using UnityEngine;

// Class Movement - controls player movement and gravity
public class Movement : MonoBehaviour
{

    #region private variables

    private Rigidbody2D rb;
    // prevents player from moving (including gravity)
    private bool frozen = false;
    // true if player is touching ground
    private bool isGrounded = false;
    // is true if the player is in the middle of a jump
    private bool isJumping = false;
    // if player has pressed space close to, but not on ground
    private bool jumpBuffered = false;
    // if player is able to double jump
    private bool doubleJumpAvailable = false;
    // used to calculate when the player can no longer continue jumping and when they are jumping too fast
    private float timeSinceJump = 0.0f;
    // used for coyote time
    private float timeSinceGrounded = 0.0f;

    #endregion

    #region public variables

    [Space(10)]
    [Header("Movement")]
    public float maxSpeed;

    // acceleration based on this constant 
    public float accelerationSpeed;

    // deceleration based on this constant 
    public float decelerationSpeed;

    [Space(10)]
    [Header("Jump")]
    // whether to allow jump buffering
    public bool bufferJump;

    // whether double jump is available
    public bool doubleJump;

    public bool infiniteJump;

    // the force that continuely is applied as player holds space
    public float jumpVelocity;

    // the rate at which the jump force increases while player holds space
    public float jumpAcceleration;

    public float maxTimeHoldingJump;

    // margin time for player to jump even after going over a ledge
    public float coyoteTime;

    [Space(10)]
    [Header("Gravity")]
    // the gravScale when player is moving upwards
    public float jumpingGravity;

    // the gravScale when player is falling
    public float fallingGravity;

    // the highest speed rb.velocity.y can reach
    public float maxDownwardVelocity;

    [Space(10)]
    public MovementRequirements required;

    #endregion

    // Method Start() - runs at the start of the game to obtain reference to Rigidbody2D rb
    private void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody2D>();
    }

    // Method Update() - called once per frame
    void Update()
    {
        
        // check if play is grounded and set isGrounded and timeSinceGrounded
        isGrounded = Physics2D.OverlapCircle(required.feetPosition.position, required.distanceFromGround, (int) Layers.ground);
        if (isGrounded)
            timeSinceGrounded = Time.time;

        // if player is touching ground and doubleJump is enabled, set doubleJumpAvailable = true
        if ((isGrounded || Time.time - timeSinceGrounded < coyoteTime) && doubleJump)
            doubleJumpAvailable = true;

        // enable movement and gravity if player is not frozen
        if (!frozen)
        {

            if (bufferJump)
                JumpBuffer();

            CalculateHorizontalMovement();
            CalculateGravity();
            CalculateJumpMovement();

        } else
        {

            // freeze player
            rb.gravityScale = 0;
            rb.velocity = Vector2.zero;

        }

    }

    // Method JumpBuffer() - called by Update() every frame if bufferJump == true. Checks
    // if player is pressing jump while near ground but not on it. If so, buffer the jump.
    public void JumpBuffer()
    {

        if (Input.GetKeyDown(KeyCode.Space) && Physics2D.OverlapCircle(required.feetPosition.position, required.jumpBufferDistance, (int) Layers.ground) && rb.velocity.y < 0)
        {
            jumpBuffered = true;
        }

    }

    // Method CalculateHorizontalMovement() - called by Update() every frame, calculates moveSpeed based
    // on input parameters (handles horizontal movement)
    public void CalculateHorizontalMovement()
    {

        // input is -1, 1, or 0 based off whether user is pressing 'a', 'd', '<-', or '->'
        float input = Input.GetAxisRaw("Horizontal");

        // if player is moving significantly fast and user is not pressing anything, use decelerationSpeed to slow down
        if (input == 0 && Mathf.Abs(rb.velocity.x) > decelerationSpeed * Time.deltaTime)
            rb.velocity = new Vector2(rb.velocity.x - Mathf.Sign(rb.velocity.x) * decelerationSpeed * Time.deltaTime, rb.velocity.y);

        // if player is not moving or drifting slowly while input == 0 then freeze the player's horizontal movement
        else if (input == 0)
            rb.velocity = new Vector2(0, rb.velocity.y);

        // if the player changes directions suddently, don't decelerate just switch directions so movement looks sharp
        else if (Mathf.Abs(rb.velocity.x / maxSpeed - input) > 1.2f)
            rb.velocity = new Vector2(input * maxSpeed * 0.5f, rb.velocity.y);

        // otherwise if player is pressing 'a', 'd', '<-', or '->' then accelerate using accelerationSpeed
        else
        {

            rb.velocity = new Vector2(rb.velocity.x + input * Time.deltaTime * accelerationSpeed, rb.velocity.y);

            // cap speed at maxSpeed
            if (Mathf.Abs(rb.velocity.x) > maxSpeed)
                rb.velocity = new Vector2(input * maxSpeed, rb.velocity.y);

        }

    }

    // Method CalculateGravity() - called by Update() every frame, calculates gravity based
    // on input parameters (changes gravitational constant based on whether falling/jumping)
    public void CalculateGravity()
    {

        if (rb.velocity.y >= 0)
        {
            rb.gravityScale = jumpingGravity;
        } else
        {
            rb.gravityScale = fallingGravity;
        }

        if (rb.velocity.y < -maxDownwardVelocity)
        {
            // if player is already going too fast, stop accelerating
            rb.gravityScale = 0;
        }

    }

    // Method CalculateJumpMovement() - called by Update() every frame, calculates jump strength based
    // on whether player is pressing space
    public void CalculateJumpMovement()
    {

        /* 
         * if player is pressing space or has pressed space while near the ground then check for
         * one of the following:
         * - player is on the ground
         * - player has a double jump available
         * - has infinite jump enabled
         * - has recently been in contact with the ground (coyoteTime)
         * If any return true then the player will jump
         */
        if ((isGrounded || infiniteJump || doubleJumpAvailable || Time.time - timeSinceGrounded < coyoteTime) && (Input.GetKeyDown(KeyCode.Space) || jumpBuffered) && (Time.time - timeSinceJump) > 0.2f)
        {

            jumpBuffered = false;

            // player is currently pressing space
            isJumping = true;

            // if player is not on the ground (meaning they used a double jump) then remove doubleJumpAvailable
            if (!(isGrounded && Time.time - timeSinceGrounded < coyoteTime))
                doubleJumpAvailable = false;

            rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
            timeSinceJump = Time.time;

        }

        // if player continues to press space after the initial jump and they have been jumping for less than
        // maxTimeHoldingJump, continues to move upwards
        if (Input.GetKey(KeyCode.Space) && isJumping == true && Time.time - timeSinceJump < maxTimeHoldingJump)
            rb.velocity = new Vector2(rb.velocity.x, jumpVelocity + (Time.time - timeSinceJump) * jumpAcceleration);

        // if player has just released space or reached maxTimeHoldingJump
        else if (isJumping)
        {

            // rapidly reduce the vertical velocity
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 1.5f);

            // end of jump
            isJumping = false;

        }

    }

}

// Class MovementRequirements - references and constants required for Movement to run without errors
[System.Serializable]
public class MovementRequirements
{

    // position of players feet (to check if player is grounded)
    public Transform feetPosition;

    // minimum distance from character feet to ground
    public float distanceFromGround;

    // max distance from the ground that player can buffer their jump
    public float jumpBufferDistance;
}
