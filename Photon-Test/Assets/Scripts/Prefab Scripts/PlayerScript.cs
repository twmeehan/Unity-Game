using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class playerScript - controls player movement and gravity if character is
// controled by the user running the script
public class PlayerScript : MonoBehaviour
{

    // private variables

    // true if player is touching ground
    private bool isGrounded = false;

    // is true if the player is in the middle of a jump
    private bool jumping = false;

    // is true while user is still holding space
    private bool spaceDown = false;

    // if player is able to double jump
    private bool doubleJump = false;

    // used to calculate when the player can no longer continue jumping
    private float timeSinceJump = 0.0f;

    // the value of Input.GetAxisRaw("Horizontal") from the previous frame
    private float pastInput = 0;

    // the speed at which the player should be moving just based on player input
    private float moveSpeed = 0;

    // used to calculate moveSpeed when accelerating
    private float moveStartTime = 0;

    private float timeSinceGrounded;

    private Transform transform;
    private Rigidbody2D rb;
    private PhotonView view;


    // Public variables

    public movement Movement;

    public gravity Gravity;

    public jump Jump;

    public GameObject camera;

    public LayerMask GroundLayer;

    // distance from center of character (or feet) to ground
    public float detectionRadius;

    // margin time for player to jump even after going over a ledge
    public float coyoteTime;

    //public Transform IsGroundedChecker;

    // Start is called before the first frame update
    void Start()
    {

        transform = this.gameObject.GetComponent<Transform>();
        rb = this.gameObject.GetComponent<Rigidbody2D>();
        view = this.gameObject.GetComponent<PhotonView>();
        PhotonNetwork.SerializationRate = 20;

        if (view.IsMine)
        {
            camera.SetActive(true);
        }
    }

    // Update() Method - Called every frame to handle jump mechanics. 
    // For some reason it only works in Update() not FixedUpdate()
    void Update()
    {


        // If touching the ground, the player becomes able to double jump again
        if (view.IsMine && isGrounded && Jump.doubleJump)
        {

            doubleJump = true;

        }

        // doubleJump always available
        if (Jump.infiniteJump)
        {

            doubleJump = true;

        }


        // If space is pressed by the owner of the photon view, the player executes the jump code
        if (view.IsMine && Input.GetKeyDown(KeyCode.Space))
        {

            // Executes if the player is on the ground or has been on the ground in the past few milliseconds
            if (isGrounded || (Time.time - timeSinceGrounded) < coyoteTime)
            {

                rb.velocity = new Vector2(rb.velocity.x, 0.0f);


                // boolean jump determains when the player first presses
                jumping = true;

                // spaceDown lasts till the player lets go of space or goes past the max holding time
                spaceDown = true;

            }

            // If not normal jumping then check if the player can double jump
            else if (doubleJump)
            {

                // Cancel downwards velocity
                rb.velocity = new Vector2(rb.velocity.x, 0.0f);


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

    // Update is called once per frame
    void FixedUpdate()
    {
        if (view.IsMine)
        {

            // calculate whether player is on the ground
            isGrounded = false;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, detectionRadius, GroundLayer);
            if (hit.collider != null)
            {

                timeSinceGrounded = Time.time;
                isGrounded = true;

            }

            jump();
            move(Input.GetAxisRaw("Horizontal"));
            gravity();

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

            rb.velocity = new Vector2(0.0f, Jump.jumpStrength);
            jumping = false;

        }

        // continues to run as long as player is still holding space
        else if (spaceDown)
        {

            rb.velocity += new Vector2(0.0f, Jump.jumpHoldConstant * Time.deltaTime);

            // player can only hold space for maxTimeHoldingJump seconds
            if ((Time.time - timeSinceJump) > Jump.maxTimeHoldingJump)
            {
                spaceDown = false;
            }
        }
    }

    /* 
     * gravity() Method - called by FixedUpdate() every frame, calculates the speed of gravity
     */
    public void gravity()
    {
        if (rb.velocity.y >= 0)
        {
            rb.gravityScale = Gravity.decelGrav;
        }
        else
        {
            rb.gravityScale = Gravity.fallingGrav;
        }
        if (rb.velocity.y < -Gravity.maxDownwardVelocity)
        {
            // if player is already going too fast, stop accelerating
            rb.gravityScale = 0;
        }
    }

    /* 
     * move() Method - called by FixedUpdate() every frame, calculates moveSpeed based
     * on input parameter
     */
    public void move(float input)
    {

        // if user stops moving, slow down
        if (input == 0)
        {

            moveSpeed = Mathf.Lerp(moveSpeed, 0, Movement.moveDecelConstant * Time.deltaTime);

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
            moveSpeed = Mathf.Lerp(0, input, (Time.time - moveStartTime) / Movement.moveAccelTime);

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
        rb.velocity = new Vector2(moveSpeed * Movement.speed, rb.velocity.y);

    }

}
[System.Serializable]
public class gravity
{

    // the gravScale when player is moving upwards
    public float decelGrav;

    // the gravScale when player is falling
    public float fallingGrav;

    // the highest speed rb.velocity.y can reach
    public float maxDownwardVelocity;

}

[System.Serializable]
public class movement
{

    // multiplies player input moveSpeed by scalar speed
    public float speed;

    // the time it takes for the player to accelerate
    public float moveAccelTime;

    // linear interpolates for deceleration based on this constant 
    public float moveDecelConstant;

}

[System.Serializable]
public class jump
{

    // stength of initial force when player first presses space
    public float jumpStrength;

    // the force that continuely is applied as player holds space
    public float jumpHoldConstant;

    public float maxTimeHoldingJump;

    public bool doubleJump;

    public bool infiniteJump;

}