using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    // Start is called before the first frame update
    void Start()
    {
        
    }

    
    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(Objects.feetPos.position, MiscConstants.distanceFromGround, Layers.ground);
        if (isGrounded)
            timeSinceGrounded = Time.time;
        if ((isGrounded || Time.time - timeSinceGrounded < MiscConstants.coyoteTime) && doubleJump)
            doubleJumpAvailable = true;
    }
    [System.Serializable]
    public class jump
    {

        // whether to allow jump buffering
        public bool bufferJump;

        // whether double jump is available
        public bool doubleJump;

        public bool infiniteJump;

        // the force that continuely is applied as player holds space
        public float jumpVelocity;

        // the rate at which the jump force increases while player holds space
        public float jumpAccel;

        public float maxTimeHoldingJump;

        // max distance from the ground that player can buffer their jump
        public float jumpBufferDistance;

    }

}
