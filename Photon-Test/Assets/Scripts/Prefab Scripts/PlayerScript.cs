using Cinemachine;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Class playerScript - controls player movement and gravity if character is
// controled by the user running the script
public class PlayerScript : MonoBehaviourPunCallbacks, IOnEventCallback, IPunObservable
{

    #region private variables

    // info about player
    private bool frozen = false;
    private bool infected = false;
    private string room;
    private bool sleeping = false;

    private bool transitioningToSleep = false;

    // PhotonNetwork.time when the timer started (given by master client)
    private float timeSinceTimerStart;

    // the starting number of seconds on countdown clock
    private float timerSeconds;

    // true if player is touching ground
    private bool isGrounded = false;

    // is true if the player is in the middle of a jump
    private bool isJumping = false;

    // if player has pressed space close to, but not on ground
    private bool jumpBuffered = false;

    // if player is able to double jump
    private bool doubleJump = false;

    // used to calculate when the player can no longer continue jumping and when they are jumping too fast
    private float timeSinceJump = 0.0f;

    // used for coyote time
    private float timeSinceGrounded;

    // what button is currently displayed to the player in the right hand corner
    private int buttonType = 0;

    #endregion

    #region final variables

    // numerical id for each button that can be displayed
    private int DISABLED = 0;
    private int GET_INTO_BED = 1;
    private int LEAVE_BED = 2;

    // components of player prefab
    private Transform transform;
    private Rigidbody2D rb;
    private PhotonView view;
    private Animator anim;

    #endregion

    #region public variables

    public movement Movement;

    public gravity Gravity;

    public jump Jump;

    public layers layers;

    public objects Objects;

    public floats MiscConstants;

    #endregion

    #region temp variables

    float deltaTime;

    #endregion

    void Start()
    {

        //anim = this.GetComponent<Animator>();
        transform = this.GetComponent<Transform>();
        rb = this.GetComponent<Rigidbody2D>();
        view = this.GetComponent<PhotonView>();

        PhotonNetwork.SerializationRate = 30;

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Sending SetTime Event");
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            object[] content = new object[] { 150, PhotonNetwork.Time};
            PhotonNetwork.RaiseEvent(3, content, raiseEventOptions, SendOptions.SendReliable);
        }
        if (view.IsMine)
        {
            Objects.camera.SetActive(true);
        }

        Objects.name.text = view.Owner.NickName;

    }

    // Update() Method - Called every frame to handle jump mechanics. 
    // For some reason it only works in Update() not FixedUpdate()
    public void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        //Debug.Log(Mathf.Ceil(fps).ToString());

        if (transitioningToSleep && !Objects.transitioner.GetCurrentAnimatorStateInfo(0).IsName("Fade"))
        {
            Objects.transitioner.SetTrigger("Reappear");
            WakeUp();
            Debug.Log("Reappear");
            transitioningToSleep = false;
            //transitioningToSleep = false;
        }
        // stops glitch where when player enters bed and presses space simultaneously, the player floats -- 1/1/22
        if (sleeping && rb.velocity.y > 0)
            rb.velocity = new Vector2(0,0);

        // calculate whether player is on the ground
        isGrounded = Physics2D.OverlapCircle(Objects.feetPos.position, MiscConstants.distanceFromGround, layers.groundLayer);
        if (isGrounded)
            timeSinceGrounded = Time.time;
        if ((isGrounded || Time.time - timeSinceGrounded < MiscConstants.coyoteTime) && Jump.doubleJump)
            doubleJump = true;

        if (view.IsMine)
        {
            timer();
            if (!frozen)
            {
                if (Jump.bufferJump)
                    jumpBuffer();
                jump();
            }
        }
    }
    public void jumpBuffer()
    {

        if (Input.GetKeyDown(KeyCode.Space) && Physics2D.OverlapCircle(Objects.feetPos.position, Jump.jumpBufferDistance, layers.groundLayer) && rb.velocity.y < 0)
        {
            jumpBuffered = true;
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (view.IsMine)
        {

            bed();

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
        if ((isGrounded || Jump.infiniteJump || doubleJump || Time.time-timeSinceGrounded< MiscConstants.coyoteTime) && (Input.GetKeyDown(KeyCode.Space) || jumpBuffered) && (Time.time - timeSinceJump) > 0.2f)
        {
            jumpBuffered = false;
            isJumping = true;
            if (!(isGrounded && Time.time - timeSinceGrounded < MiscConstants.coyoteTime))
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
        
        if (!sleeping && !transitioningToSleep)
        {

            RaycastHit2D currentBed = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, layers.bedLayer);
            if (currentBed.collider != null)
            {

                Objects.button.interactable = true;
                buttonType = GET_INTO_BED;
                //button.image.sprite=...

            }
            else
            {

                Objects.button.interactable = false;
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

    public void timer()
    {
        if (timerSeconds - Mathf.FloorToInt((float) PhotonNetwork.Time - timeSinceTimerStart) < 0)
            Objects.time.text = "0:00";
        else
        {
            Objects.time.text = Mathf.FloorToInt((timerSeconds - Mathf.FloorToInt((float)PhotonNetwork.Time - timeSinceTimerStart)) / 60) 
                + ":" + ((timerSeconds - Mathf.FloorToInt((float)PhotonNetwork.Time - timeSinceTimerStart)) % 60).ToString("00");
        }


    }
    public void StartTimer(float seconds,float startTime)
    {
        timerSeconds = seconds;
        timeSinceTimerStart = startTime;
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

        Objects.button.enabled = true;
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

        rb.velocity = new Vector2(rb.velocity.x, MiscConstants.yeetStrength);

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

    public bool getSleeping()
    {
        return this.sleeping;
    }
    public void OnEvent(EventData photonEvent)
    {

        switch (photonEvent.Code)
        {
            case 2:
                if (!transitioningToSleep)
                {
                    sleeping = false;
                    transitioningToSleep = true;
                    Objects.transitioner.SetTrigger("Fade");
                }
                //WakeUp();
                break;
            case 3:
                object[] data = (object[])photonEvent.CustomData;
                StartTimer(Convert.ToSingle(data[0]), Convert.ToSingle(data[1]));
                break;
        }

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(sleeping);
        } else
        {
            sleeping = (bool) stream.ReceiveNext();
        }
    }
}

// Class gravity - holds variables used to customize the players gravitational constant
// used by playerScript 
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

// Class movement - holds variables used to customize the players horizontal movement
// used by playerScript 
[System.Serializable]
public class movement
{

    public float maxSpeed;

    // acceleration based on this constant 
    public float accelSpeed;

    // deceleration based on this constant 
    public float decelSpeed;

}

// Class jump - holds variables used to customize the players jump
// used by playerScript 
[System.Serializable]
public class jump
{

    // whether to allow jump buffering
    public bool bufferJump;

    // whether double jump is available
    public bool doubleJump;

    public bool infiniteJump;

    // the force that continuely is applied as player holds space
    public float jumpStrength;

    // the rate at which the jump force increases while player holds space
    public float jumpAccel;

    public float maxTimeHoldingJump;

    // max distance from the ground that player can buffer their jump
    public float jumpBufferDistance;

}

// Class layers - holds references to each layer referenced by playerScript
[System.Serializable]
public class layers
{

    public LayerMask groundLayer;

    public LayerMask roomLayer;

    public LayerMask bedLayer;

}

// Class objects - holds references to all GameObjects in the player prefab that
// are referenced by playerScript 
[System.Serializable]
public class objects
{

    // player specific camera
    public GameObject camera;

    // text that floats above the players head and displays his/her username
    public TextMeshProUGUI name;

    // text that displays countdown timer
    public TextMeshProUGUI time;

    // button that is used for interacting and is in the lower left hand corner
    public Button button;

    public Transform feetPos;

    public Animator transitioner;
}

// Class objects - holds random numerical constants used to customize miscellaneous parts
// of playerScript 
[System.Serializable]
public class floats
{

    // the force at which the player is launched from a bed
    public float yeetStrength;

    // distance from character feet to ground
    public float distanceFromGround;

    // margin time for player to jump even after going over a ledge
    public float coyoteTime;

}