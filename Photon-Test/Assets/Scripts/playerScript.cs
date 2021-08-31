using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerScript : MonoBehaviourPunCallbacks
{
    // the rate at which rb.velocity will decelerate
    public float drag = 0.2f;

    // the highest speed rb.velocity.y can reach
    public float maxDownwardVelocity = 15.0f;

    // the highest speed rb.velocity.y can reach when touching a wall
    public float maxClingingVelocity = 5.0f;

    // the value of Input.GetAxisRaw("Horizontal") from the previous frame
    private float pastInput = 0;

    // the speed at which the player should be moving just based on player input
    private float moveSpeed = 0;

    // the actual speed at which the player is moving
    private float xVel = 0;

    // multiplies player input moveSpeed by scalar speed
    public float speed = 2;

    // linear interpolates for deceleration based on this constant 
    public float moveDecelConstant = 5.0f;

    // the time it takes for the player to accelerate
    public float moveAccelTime = 0.05f;

    // used to calculate moveSpeed when accelerating
    private float moveStartTime = 0;

    // the gravScale when player is moving upwards
    public float decelGrav = 9.0f;

    // the gravScale when player is sliding down a wall
    public float clingingGrav = 2.0f;

    // the gravScale when player is falling
    public float fallingGrav = 15.0f;

    // For Jump
    private bool jumping = false;
    private bool wallJumping = false;
    private bool spaceDown = false;
    private bool doubleJump = true;

    private float sideJumpMultiplier = 1.0f;
    private float sideJumpStartTime = 0;
    public float sideJumpSpeed = 0.5f;
    public float sideMultiplierDecel = 5.0f;

    public float detectionRadius = 0.3f;
    public float lockoutTime = 0.1f;
    //public Transform IsGroundedChecker;
    public LayerMask GroundLayer;
    public float jumpStrength = 1.0f;
    public float jumpHoldConstant = 0.7f;
    public float maxTimeHoldingJump = 0.4f;
    private float timeSinceJump = 0.0f;
    public float upwardsVelocityForWalljump = 2.0f;
    public float coyoteTime = 0.2f;
    private float timeSinceGrounded;

    [SerializeField] private Vector2 externalForce = new Vector2(0.0f, 0.0f);



    // private Vector2 predictedPos = new Vector2(0, 0);

    private Transform transform;
    private Rigidbody2D rb;
    private PhotonView view;
    private SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {

        transform = this.gameObject.GetComponent<Transform>();
        rb = this.gameObject.GetComponent<Rigidbody2D>();
        view = this.gameObject.GetComponent<PhotonView>();
        sr = this.gameObject.GetComponent<SpriteRenderer>();

        PhotonNetwork.SerializationRate = 20;
    }

    // Update() Method - Called every frame to handle jump mechanics. 
    // For some reason it only works in Update() not FixedUpdate()
    void Update()
    {

        // If touching the ground, the player becomes able to double jump again
        if (view.IsMine && (IsGrounded() || touchWallLeft() || touchWallRight()))
        {

            doubleJump = true;

        }

        // If space is pressed by the owner of the photon view, the player executes the jump code
        if (view.IsMine && Input.GetKeyDown(KeyCode.Space))
        {

            // Executes if the player is on the ground or has been on the ground in the past few milliseconds
            if (IsGrounded() || (Time.time - timeSinceGrounded) < coyoteTime)
            {

                /*
                // If touching wall or in the middle of a wall jump
                if (touchWallLeft() && wallJumping)
                {

                    moveSpeed = sideJumpSpeed;
                    sideJumpStartTime = Time.time;

                }

                else if (touchWallRight() && wallJumping)
                {

                    moveSpeed = -sideJumpSpeed;
                    sideJumpStartTime = Time.time;


                }
                */
                
                // Else if normal jumping and player is falling
                if (rb.velocity.y < 0)
                {

                    // Cancel downwards velocity
                    rb.velocity = new Vector2(rb.velocity.x, 0.0f);

                }

                // boolean jump determains when the player first presses
                jumping = true;

                // spaceDown lasts till the player lets go of space or goes past the max holding time
                spaceDown = true;

            }

            // If not normal jumping then check if the player can double jump
            else if (doubleJump)
            {

                // Cancel downwards velocity
                if (rb.velocity.y < 0)
                {
                    
                    rb.velocity = new Vector2(rb.velocity.x, 0.0f);

                }

                // remove the ability to doubleJump so that player cannot double jump again until they land
                doubleJump = false;

                // boolean jump determains when the player first presses
                jumping = true;

                // spaceDown lasts till the player lets go of space or goes past the max holding time
                spaceDown = true;

            }

        }

        // check if player is no longer holding space to stop their upward acceleration
        else if (view.IsMine && Input.GetKeyUp(KeyCode.Space))
        {

            spaceDown = false;

        }

    }

    /* 
     * Jump() Method - called by FixedUpdate() every frame, calculates jump strength based
     * on input determained in Update()
     */
    public void jump()
    {

        // runs when the player first presses space
        if (jumping)
        {

            // does smth. I forgot what
            timeSinceJump = Time.time;

            rb.velocity += new Vector2(0.0f, jumpStrength);
            jumping = false;

        }

        // continues to run as long as player is still holding space
        else if (spaceDown)
        {

            rb.velocity += new Vector2(0.0f, jumpHoldConstant * Time.deltaTime);

            // player can only hold space for maxTimeHoldingJump seconds
            if ((Time.time - timeSinceJump) > maxTimeHoldingJump)
            {
                spaceDown = false;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (view.IsMine)
        {

            //Debug.Log(rb.velocity.x);
            jump();

            //calcSideMultiplier();
            //crouch();
            
/*
            if ((touchWallLeft() && rb.velocity.x < 0) || (touchWallRight() && rb.velocity.x > 0))
            {
                pastxVel = 0;
                xVel = 0;
                rb.velocity = new Vector2(0.0f, rb.velocity.y);
            }
            if (rb.velocity.x == 0)
            {
                pastxVel = 0;
                xVel = 0;
            }
            */
            

            gravity();
            //CalcAccel();
            move(Input.GetAxisRaw("Horizontal"));

            /*
            try
            {
                if (Time.time - timeSinceGrounded > lockoutTime && wallJumping)
                {

                    moveSpeed = 0;
                    wallJumping = false;

                }
            }
            catch
            {

            }
            */
            



            //addExternalForce();



        }
        else
        {
            /*
            Debug.Log(Time.time + " - " + lastPacket + "/" + lag + " = "+(Time.time - lastPacket) / lag);
            transform.position = Vector2.Lerp(startPos, pos, (float)(Time.time - lastPacket) / lag);
            rb.velocity = Vector2.Lerp(startVel, vel, (float)(Time.time - lastPacket) / lag);
            */


        }


    }
    public void CalcAccel()
    {
        
        externalForce = Vector2.Lerp(externalForce, Vector2.zero,0.2f);
        if (externalForce.magnitude<0.2f)
        {
            externalForce = Vector2.zero;
        }
        
        //accel = new Vector2(0.0f,0.0f);
        //accel.y = -grav;
        //rb.velocity += accel * Time.deltaTime;
        //rb.gravityScale = grav;

    }
    public void gravity()
    {

        if (rb.velocity.y >= 0)
        {

            rb.gravityScale = decelGrav;

        }

        else if (rb.velocity.y < maxDownwardVelocity)
        {

            if (touchWallLeft())
            {

                rb.gravityScale = clingingGrav;


            }
            else if (touchWallRight())
            {

                rb.gravityScale = clingingGrav;


            }
            else
            {

                rb.gravityScale = fallingGrav;

            }
        }
        else
        {

        }
    }
    public void move(float input)
    {
        
        // briefly locks movement for lockoutTime seconds after wall jumping
        if (!wallJumping || (Time.time - timeSinceJump) > lockoutTime)
        {

            sr.color = Color.white;

            // if user stops moving, slow down
            if (input == 0)
            {

                moveSpeed = Mathf.Lerp(moveSpeed, 0, moveDecelConstant * Time.deltaTime);

            }

            // if the user inputs movement after being still
            else if (Mathf.Abs(input) > 0 && pastInput == 0)
            {

                // get ready to accelerate
                moveSpeed = 0;
                moveStartTime = Time.time;

            }

            // if the user input remains the same
            else if (input == pastInput && Mathf.Abs(moveSpeed) < 1)
            {
                // accelerate over moveAccelTime then clamp the speed at 1
                moveSpeed = Mathf.Lerp(0, input, (Time.time - moveStartTime) / moveAccelTime);

                if (moveSpeed > 1)
                {
                    moveSpeed = 1;
                }
                else if (moveSpeed < -1)
                {
                    moveSpeed = -1;
                }

            }

            // if the user suddenly changes direction, stop and start accelerating the other way
            else if (Mathf.Abs(pastInput - input) > 1.5)
            {
                moveSpeed = 0;
            }

            pastInput = input;

            xVel = moveSpeed * speed * sideJumpMultiplier;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(-0.1f,-1.0f).normalized, detectionRadius, GroundLayer);
            RaycastHit2D hit2 = Physics2D.Raycast(transform.position, new Vector2(0.1f, -1.0f).normalized, detectionRadius, GroundLayer);

            Vector2 direction = Vector2.right;

            if (hit.collider != null && hit2.collider != null)
            {

                direction = (hit2.point - hit.point).normalized;

            }
            
            hit = Physics2D.Raycast(transform.position, direction, xVel * Time.deltaTime, GroundLayer);
            if (hit.collider != null)
            {

                xVel = 0;

            }
            else
            {

                transform.position += (Vector3)direction * xVel * Time.deltaTime;

            }

        }

        // runs while user can't move because of a wall jump
        else
        {

            sr.color = Color.red;

            xVel = moveSpeed * speed * sideJumpMultiplier;

        }

    }

    /*
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

        if (stream.IsWriting)
        {

            stream.SendNext(vel);
            stream.SendNext(pos);
            stream.SendNext(accel);

        } else
        {

            vel = (Vector2) stream.ReceiveNext();
            pos = (Vector2) stream.ReceiveNext();
            accel = (Vector2) stream.ReceiveNext();

            this.startPos = transform.position;
            this.startVel = rb.velocity;


            lastPacket = (float) Time.time;
            lag = Mathf.Abs((float)(PhotonNetwork.Time - info.timestamp));
            lagComp(pos, vel, accel, lag);
            
        }

    }
    */

    public void wallJump()
    {
        if ((Time.time - sideJumpStartTime) / sideMultiplierDecel < 1.0f)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, sign(moveSpeed), (Time.time - sideJumpStartTime) / sideMultiplierDecel);
        }
    }

    public int sign(float f)
    {
        if (f > 0)
        {
            return 1;
        } else if (f < 0)
        {
            return -1;
        }
        return 0;
    }
    public void addExternalForce()
    {
        rb.velocity += new Vector2(externalForce.x,0.0f);
        transform.position += new Vector3(0.0f, externalForce.y * Time.deltaTime,0.0f);
    }

    bool IsGrounded()
    {


        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, detectionRadius, GroundLayer);
        if (hit.collider != null)
        {

            timeSinceGrounded = Time.time;
            return true;

        }

        return false;
    }
    bool touchWallLeft()
    {

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.left, detectionRadius, GroundLayer);
        if (hit.collider != null)
        {
            timeSinceGrounded = Time.time;
            //wallJumping = true;
            return true;

        }

        return false;
    }

    bool touchWallRight()
    {

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, detectionRadius, GroundLayer);
        if (hit.collider != null)
        {
            timeSinceGrounded = Time.time;
            //wallJumping = true;
            return true;

        }

        return false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

        if (stream.IsWriting)
        {

            stream.SendNext(externalForce);

        }
        else
        {

            externalForce = (Vector2)stream.ReceiveNext();

        }

    }
    
    void applykb(Vector2 kb)
    {
        externalForce += kb;
        Debug.Log("BOOM");
        view.RPC("applykbNET", RpcTarget.All, externalForce);
    }
    [PunRPC]
    void applykbNET(Vector2 vel)
    {
        this.externalForce = vel;
    }
}
