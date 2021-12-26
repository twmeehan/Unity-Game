using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Class playerScript - controls player movement and gravity if character is
// controled by the user running the script
public class PlayerScript : MonoBehaviour
{

    // private variables

    private bool frozen = false;
    private bool infected = false;

    private string room;

    private bool sleeping = false;

    // true if player is touching ground
    private bool isGrounded = false;

    // is true if the player is in the middle of a jump
    private bool isJumping = false;

    // if player has pressed space close to but not on ground
    private bool jumpBuffered = false;

    // is true while user is still holding space
    private bool spaceDown = false;

    // if player is able to double jump
    private bool doubleJump = false;

    // used to calculate when the player can no longer continue jumping
    private float timeSinceJump = 0.0f;

    private float timeSinceGrounded;

    private Transform transform;
    private Rigidbody2D rb;
    private PhotonView view;
    private Animator anim;

    private int buttonType = 0;

    private int DISABLED = 0;
    private int GET_INTO_BED = 1;
    private int LEAVE_BED = 2;

    float deltaTime;
    // Public variables

    public movement Movement;

    public gravity Gravity;

    public jump Jump;

    public layers layers;

    public GameObject camera;

    public TextMeshProUGUI name;

    public Button button;

    public float yeetStrength;

    public Transform feetPos;

    // distance from center of character (or feet) to ground
    public float distanceFromGround;

    // margin time for player to jump even after going over a ledge
    public float coyoteTime;

    //public Transform IsGroundedChecker;

    // Start is called before the first frame update
    void Start()
    {

        //anim = this.GetComponent<Animator>();
        transform = this.GetComponent<Transform>();
        rb = this.GetComponent<Rigidbody2D>();
        view = this.GetComponent<PhotonView>();

        PhotonNetwork.SerializationRate = 20;

        if (view.IsMine)
        {
            camera.SetActive(true);
        }

        name.text = view.Owner.NickName;

    }

    // Update() Method - Called every frame to handle jump mechanics. 
    // For some reason it only works in Update() not FixedUpdate()
    public void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        Debug.Log(Mathf.Ceil(fps).ToString());

        // calculate whether player is on the ground
        isGrounded = Physics2D.OverlapCircle(feetPos.position, distanceFromGround, layers.groundLayer);
        if (isGrounded)
            timeSinceGrounded = Time.time;
        if ((isGrounded || Time.time - timeSinceGrounded < coyoteTime) && Jump.doubleJump)
            doubleJump = true;

        if (view.IsMine && !frozen)
        {
            if (Jump.bufferJump)
                jumpBuffer();
            jump();
        }
    }
    public void jumpBuffer()
    {

        if (Input.GetKeyDown(KeyCode.Space) && Physics2D.OverlapCircle(feetPos.position, Jump.jumpBufferDistance, layers.groundLayer) && rb.velocity.y < 0)
        {
            jumpBuffered = true;
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (view.IsMine)
        {

            if (!frozen)
            {
                move();
                gravity();

            }

        }


    }

    /* 
     * Jump() Method - called by FixedUpdate() every frame, calculates jump strength based
     * on input determained in Update()
     */
    public void jump()
    {
        if ((isGrounded || Jump.infiniteJump || doubleJump || Time.time-timeSinceGrounded<coyoteTime) && (Input.GetKeyDown(KeyCode.Space) || jumpBuffered))
        {
            jumpBuffered = false;
            isJumping = true;
            if (!(isGrounded && Time.time - timeSinceGrounded < coyoteTime))
                doubleJump = false;
            rb.velocity = new Vector2(rb.velocity.x, Jump.jumpStrength);
            timeSinceJump = Time.time;
        }
        if (Input.GetKey(KeyCode.Space) && isJumping == true && Time.time - timeSinceJump < Jump.maxTimeHoldingJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, Jump.jumpStrength + (Time.time - timeSinceJump)*Jump.jumpAccel);

        } else if (isJumping)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y/1.5f);
            isJumping = false;
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
    public void move()
    {

        float input = Input.GetAxisRaw("Horizontal");

        if (input == 0 && Mathf.Abs(rb.velocity.x) > Movement.decelSpeed * Time.deltaTime)
            rb.velocity = new Vector2(rb.velocity.x - Mathf.Sign(rb.velocity.x) * Movement.decelSpeed * Time.deltaTime, rb.velocity.y);
        else if (input == 0)
            rb.velocity = new Vector2(0, rb.velocity.y);
        else if (Mathf.Abs(rb.velocity.x/Movement.maxSpeed - input) > 1.2f)
            rb.velocity = new Vector2(input * Movement.maxSpeed * 0.5f, rb.velocity.y);
        else
        {
            rb.velocity = new Vector2(rb.velocity.x + input * Time.deltaTime * Movement.accelSpeed, rb.velocity.y);
            if (Mathf.Abs(rb.velocity.x) > Movement.maxSpeed)
                rb.velocity = new Vector2(input * Movement.maxSpeed, rb.velocity.y);
        }

    }
    public string checkCurrentRoom()
    {
        RaycastHit2D currentRoom = Physics2D.Raycast(transform.position, Vector2.up, 0.1f, layers.roomLayer);
        if (currentRoom.collider != null)
        {

            this.room = currentRoom.collider.gameObject.name;
            return room;

        }
        return "";
    }
    public void bed()
    {
        if (!sleeping)
        {
            RaycastHit2D currentBed = Physics2D.Raycast(transform.position, Vector2.up, 0.1f, layers.bedLayer);
            if (currentBed.collider != null)
            {

                button.interactable = true;
                buttonType = GET_INTO_BED;
                //button.image.sprite=...

            }
            else
            {

                button.interactable = false;
                buttonType = DISABLED;
                //button.image.sprite=...

            }

            frozen = false;

        }
        else // runs if sleeping
        {
            frozen = true;

            RaycastHit2D currentBed = Physics2D.Raycast(transform.position, Vector2.up, 0.1f, layers.bedLayer);
            if (currentBed.collider != null && !currentBed.collider.gameObject.GetComponent<BedScript>().player.Equals(this.gameObject.GetComponent<PhotonView>().Owner.UserId))
                KickFromBed();
        }
    }
    [PunRPC]
    public void JoinBedRPC(object[] objectArray)
    {

        sleeping = true;
        rb.velocity = new Vector2(0, 0);
        rb.gravityScale = 0;

        // Move player to the center of bed
        this.transform.position = new Vector2((float)objectArray[0], (float)objectArray[1]);

        //anim.SetBool("Sleeping");

        button.enabled = true;
        //button.image.sprite=...
        buttonType = LEAVE_BED;
    }
    public void JoinBed(Transform t)
    {
        object[] objectArray = { t.position.x, t.position.y };
        view.RPC("JoinBedRPC", RpcTarget.All, objectArray as object);

    }
    public void KickFromBed()
    {
        // only run on controller's client

        rb.velocity = new Vector2(rb.velocity.x, yeetStrength);

        // TODO: Play animation or add particles or smth
        sleeping = false;
    }

    // Called when the player presses the button to leave bed
    public void WakeUp()
    {
        RaycastHit2D currentBed = Physics2D.Raycast(transform.position, Vector2.up, 0.1f, layers.bedLayer);
        currentBed.collider.gameObject.GetComponent<BedScript>().LeaveBed();
    }

    // Called when the player presses the button to enter bed
    public void Sleep()
    {

        RaycastHit2D currentBed = Physics2D.Raycast(transform.position, Vector2.up, 0.1f, layers.bedLayer);
        currentBed.collider.gameObject.GetComponent<BedScript>().EnterBed(this.gameObject);

    }
    public void ButtonPressed()
    {
        switch (buttonType)
        {
            case 0:
                Debug.Log("Error");
                break;
            case 1:
                Sleep();
                break;
            case 2:
                WakeUp();
                break;
        }
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
    public float maxSpeed;

    // the time it takes for the player to accelerate
    public float accelSpeed;

    // deceleration based on this constant 
    public float decelSpeed;

}

[System.Serializable]
public class jump
{

    public bool bufferJump;

    public bool doubleJump;

    public bool infiniteJump;

    // the force that continuely is applied as player holds space
    public float jumpStrength;

    // the rate at which the jump force increases to counteract gravity
    public float jumpAccel;

    public float maxTimeHoldingJump;

    public float jumpBufferDistance;

}

[System.Serializable]
public class layers
{

    public LayerMask groundLayer;

    public LayerMask roomLayer;

    public LayerMask bedLayer;

}