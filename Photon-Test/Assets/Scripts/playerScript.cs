using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerScript : MonoBehaviourPunCallbacks, IPunObservable
{

    public float predictionConst = 0.5f; 
    private float grav = 0;
    public float dragConstant = 0.2f;
    private Vector2 drag = new Vector2(0,0);
    public float maxDownwardVelocity = 15.0f;
    public float maxClingingVelocity = 5.0f;

    // For Crouch
    private float crouching = 1.0f;
    public float crouchSpeed = 0.5f;
    // For Move
    private float pastInput = 0;
    private float moveSpeed = 0;
    public float horizontalMoveSpeed = 2;
    public float moveDecelConstant = 5.0f;
    public float moveAccelTime = 0.05f;
    private float moveStartTime = 0;

    public float decelGrav = 9.0f;
    public float clingingGrav = 2.0f;
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

    [SerializeField] private Vector2 accel = new Vector2(0,0);
    [SerializeField] private Vector2 vel = new Vector2(0,0);
    [SerializeField] private Vector2 pos = new Vector2(0,0);

    private Vector2 startPos = new Vector2(0, 0);
    private Vector2 startVel = new Vector2(0, 0);

    private float lastPacket = 0.0f;

    private float lag = 0.1f;


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
    void Update()
    {
        if (view.IsMine && (IsGrounded() || touchWallLeft() || touchWallRight()))
        {
            doubleJump = true;

        }
        if (view.IsMine && Input.GetKeyDown(KeyCode.Space))
        {
            if (IsGrounded() || (Time.time - timeSinceGrounded) < coyoteTime)
            {
                if (touchWallLeft() && wallJumping)
                {
                    moveSpeed = 1f;
                    sideJumpMultiplier = sideJumpSpeed;
                    sideJumpStartTime = Time.time;
                }
                else if (touchWallRight() && wallJumping)
                {
                    moveSpeed = -1f;
                    sideJumpMultiplier = sideJumpSpeed;
                    sideJumpStartTime = Time.time;

                }
                else
                {
                    rb.velocity = new Vector2(0.0f, 0.0f);
                    vel = new Vector2(0.0f, 0.0f);
                }
                // doubleJump = true;
                jumping = true;
                spaceDown = true;
            } else if (doubleJump)
            {
                rb.velocity = new Vector2(0.0f,0.0f);
                vel = new Vector2(0.0f, 0.0f);
                doubleJump = false;
                jumping = true;
                spaceDown = true;   

            }

        } else if (view.IsMine && Input.GetKeyUp(KeyCode.Space))
        {
            spaceDown = false;
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (view.IsMine)
        {
            vel = rb.velocity;
            jump();
            calcSideMultiplier();
            crouch();
            move(Input.GetAxisRaw("Horizontal"));
            if ((touchWallLeft() && vel.x < 0) || (touchWallRight() && vel.x > 0))
            {
                vel.x = 0.0f;
            }
            rb.velocity = vel;
            gravity();
            CalcAccel();
            
            try
            {
                if (Time.time - timeSinceGrounded > lockoutTime && wallJumping)
                {

                    moveSpeed = 0;
                    wallJumping = false;
                    
                }
                    } catch
            {

            }
            


            pos = transform.position;

        } else
        {
            Debug.Log(Time.time + " - " + lastPacket + "/" + lag + " = "+(Time.time - lastPacket) / lag);
            transform.position = Vector2.Lerp(startPos, pos, (float)(Time.time - lastPacket) / lag);
            rb.velocity = Vector2.Lerp(startVel, vel, (float)(Time.time - lastPacket) / lag);


        }
        

    }
    public void jump()
    {
        if (jumping)
        {
            timeSinceJump = Time.time;
            vel += new Vector2(0.0f, jumpStrength);
            jumping = false;
        } else if (spaceDown)
        {
            vel += new Vector2(0.0f, jumpStrength * jumpHoldConstant * Time.deltaTime / (Time.time - timeSinceJump));
            if ((Time.time - timeSinceJump) > maxTimeHoldingJump) {
                spaceDown = false;
            }
        }
    }
    public void CalcAccel()
    {
        //drag = dragConstant * rb.velocity * -1;
        //accel = new Vector2(0.0f,0.0f);
        accel.y = -grav;
        rb.velocity += accel * Time.deltaTime;
        //rb.gravityScale = grav;

    }
    public void gravity()
    {
        if (rb.velocity.y >= 0)
        {
            grav = decelGrav;
        } else if (rb.velocity.y < maxDownwardVelocity)
        {
            if (touchWallLeft())
            {

                grav = clingingGrav;
                if (rb.velocity.y < 0 && crouching < 1.0f)
                {
                    rb.velocity = new Vector2((float) rb.velocity.x,0);

                }

            } else if (touchWallRight())
            {

                grav = clingingGrav;


                if (rb.velocity.y < 0 && crouching < 1.0f)
                {
                    rb.velocity = new Vector2((float) rb.velocity.x, 0);

                }

            } else {

                grav = fallingGrav;

            }
        } else
        {

        }
    }
    public void move(float input)
    {
        //doubleJump || (!doubleJump && (Time.time - timeSinceJump) > 0.2f)
        if (!wallJumping || (Time.time - timeSinceJump) > lockoutTime)
        {
            sr.color = Color.white;
            Debug.Log(input);
            if (input == 0)
            {
                moveSpeed = Mathf.Lerp(moveSpeed, 0, moveDecelConstant*Time.deltaTime);
            } else if (Mathf.Abs(input) > 0 && pastInput == 0)
            {
                moveSpeed = 0;
                moveStartTime = Time.time;
            } else if (input == pastInput && Mathf.Abs(moveSpeed) < 1) {
                moveSpeed = Mathf.Lerp(0,input,(Time.time - moveStartTime) / moveAccelTime);
                if (moveSpeed > 1)
                {
                    moveSpeed = 1;
                } else if (moveSpeed < -1)
                {
                    moveSpeed = -1;
                }
            } else if (Mathf.Abs(pastInput-input) > 1.5)
            {
                moveSpeed = 0;
            }
            pastInput = input;
                vel = new Vector2(moveSpeed * horizontalMoveSpeed * sideJumpMultiplier * crouching, vel.y);
        } else
        {
            sr.color = Color.red;
            vel = new Vector2(moveSpeed * horizontalMoveSpeed * sideJumpMultiplier * crouching, upwardsVelocityForWalljump);
        }
        
    }
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
    public void lagComp(Vector2 pos, Vector2 vel, Vector2 accel, float t)
    {

        RaycastHit2D hit = Physics2D.Raycast(transform.position, (0.5f * accel * t * t + vel * t), 1, GroundLayer);
        if (hit.collider != null)
        {
            //this.pos = hit.point;
            //this.vel = Vector2.zero;
        }
        else
        {
            this.pos += (0.5f * accel * t * t + vel * t) * predictionConst;
            this.vel += (accel * t) * predictionConst;
        }
    }

    public void calcSideMultiplier()
    {
        if ((Time.time - sideJumpStartTime) / sideMultiplierDecel < 1.0f)
        {
            sideJumpMultiplier = Mathf.Lerp(sideJumpSpeed, 1.0f , (Time.time - sideJumpStartTime)/sideMultiplierDecel);
        } else
        {
            sideJumpMultiplier = 1.0f;
        }
    }

    public void crouch()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            crouching = crouchSpeed;
        } else
        {
            crouching = 1;
        }
    }

    bool IsGrounded()
    {


        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, detectionRadius, GroundLayer);
        if (hit.collider != null)
        {

            Debug.Log("On Ground");
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
            Debug.Log("On Ground");
            timeSinceGrounded = Time.time;
            wallJumping = true;
            return true;

        }

        return false;
    }

    bool touchWallRight()
    {

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, detectionRadius, GroundLayer);
        if (hit.collider != null)
        {
            Debug.Log("On Ground");
            timeSinceGrounded = Time.time;
            wallJumping = true;
            return true;

        }

        return false;
    }


}
