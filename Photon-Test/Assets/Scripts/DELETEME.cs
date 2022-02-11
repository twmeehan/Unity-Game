using Cinemachine;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private RoomScript room;
    private bool sleeping = false;
    public Role role;

    private List<int> roles = new List<int>() { 1, 1, 1 };
    private bool transitioningToSleep = false;
    private bool transitioningToDay = false;
    public bool showingResults = false;

    private bool init = false;
    private bool day = true;
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
    private int Heal_Self = 3;
    private int LEAVE_BED = 2;

    // components of player prefab
    private Transform transform;
    private Rigidbody2D rb;
    private PhotonView view;
    private Animator anim;
    private System.Random rand = new System.Random();

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

    #endregion
    
    float deltaTime;

    void Start()
    {
        
        //anim = this.GetComponent<Animator>();
        transform = this.GetComponent<Transform>();
        rb = this.GetComponent<Rigidbody2D>();
        view = this.GetComponent<PhotonView>();

        PhotonNetwork.SerializationRate = 30;

        if (PhotonNetwork.IsMasterClient)
        {

            Debug.Log("Send Event to start 10 second timer");
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            object[] content = new object[] { 20, PhotonNetwork.Time};
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
        //Debug.Log(this.checkCurrentRoom().getBeds().ToStringFull());
        List<PlayerScript> players = ((PlayerScript[])FindObjectsOfType(typeof(PlayerScript))).ToList<PlayerScript>();

        if (players.Count == PhotonNetwork.CurrentRoom.Players.Count && !init && PhotonNetwork.IsMasterClient)
        {
            init = true;
            Debug.Log(players.Count);
            int i = rand.Next(players.Count);
            Debug.Log(i);
            int j;
            players[i].assignRole(0); // 1 Alien
            players.RemoveAt(i);
            foreach (PlayerScript player in players)
            {
                j = rand.Next(roles.Count);
                player.assignRole(roles[j]);
                roles.RemoveAt(j);
            }
        }
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        //Debug.Log(Mathf.Ceil(fps).ToString());

        
        
    }
    

    // Update is called once per frame
    void FixedUpdate()
    {
        if (view.IsMine)
        {
            if (transitioningToSleep && Objects.transitioner.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && Objects.transitioner.GetCurrentAnimatorStateInfo(0).IsName("Fade"))
            {
                Debug.Log(view.name + " has successfully transitioned to night");
                Objects.transitioner.SetTrigger("Reappear");
                transitioningToSleep = false;
                frozen = false;
                StartTimer(31, (float)PhotonNetwork.Time);
                if (PhotonNetwork.IsMasterClient)
                {
                    object[] content = new object[] { 31, PhotonNetwork.Time };
                    PhotonNetwork.RaiseEvent(3, content, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                }
                role.startNight(this);

            }
            if (transitioningToDay && Objects.transitioner.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && Objects.transitioner.GetCurrentAnimatorStateInfo(0).IsName("Fade"))
            {
                Debug.Log(view.name + " has successfully transitioned to day");
                Objects.transitioner.SetTrigger("Reappear");
                transitioningToDay = false;
                this.gameObject.GetComponent<PhotonView>().enabled = true;

                frozen = false;
                WakeUp();
                foreach (BedScript bed in (BedScript[])FindObjectsOfType(typeof(BedScript)))
                {
                    if (bed.player == this.gameObject.GetComponent<PhotonView>().Owner.UserId)
                    {
                        transform.position = bed.transform.position;
                    }
                }
                StartTimer(31, (float)PhotonNetwork.Time);
                if (PhotonNetwork.IsMasterClient)
                {
                    object[] content = new object[] { 31, PhotonNetwork.Time };
                    PhotonNetwork.RaiseEvent(3, content, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                }
                Objects.results.SetActive(false);
            }

        }


    }
	
    public void timer()
    {
        if (timerSeconds - Mathf.FloorToInt((float)PhotonNetwork.Time - timeSinceTimerStart) < 0) {
            Objects.time.text = "0:00";
            
            if (day && PhotonNetwork.IsMasterClient && timerSeconds != 0)
            {
                Debug.Log("Time is 0:00 - Sending Event to transition to night");
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                PhotonNetwork.RaiseEvent(2, null, raiseEventOptions, SendOptions.SendReliable);
                frozen = true;
            } if (!day && role.name == "Alien" && timerSeconds != 0 && !transitioningToSleep && !showingResults)
            {
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                object[] content = new object[] { role.gameObjects[0].GetComponent<PhotonView>().Owner.UserId };
                PhotonNetwork.RaiseEvent(4, content, raiseEventOptions, SendOptions.SendReliable);
                StartTimer(5,(float) PhotonNetwork.Time);
                showingResults = true;
                object[] content2 = new object[] { 5, PhotonNetwork.Time };
                PhotonNetwork.RaiseEvent(3, content2, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
            } if (!day && PhotonNetwork.IsMasterClient && showingResults && timerSeconds - Mathf.FloorToInt((float)PhotonNetwork.Time - timeSinceTimerStart) < 0)
            {
                Debug.Log(timerSeconds - Mathf.FloorToInt((float)PhotonNetwork.Time - timeSinceTimerStart) + " out of " + timerSeconds);
                showingResults = false;
                StartTimer(31, (float)PhotonNetwork.Time);
                object[] content = new object[] { 31, PhotonNetwork.Time };
                PhotonNetwork.RaiseEvent(3, content, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                PhotonNetwork.RaiseEvent(5, null, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);

            }
        }
        else
        {
            Objects.time.text = Mathf.FloorToInt((timerSeconds - Mathf.FloorToInt((float)PhotonNetwork.Time - timeSinceTimerStart)) / 60)
                + ":" + ((timerSeconds - Mathf.FloorToInt((float)PhotonNetwork.Time - timeSinceTimerStart)) % 60).ToString("00");
        }


    }
    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        Debug.Log("Joined Lobby");
        PhotonNetwork.LoadLevel("Menu");
    }
    
    public void assignRole(int role)
    {
        view.RPC("updateRoleRPC", RpcTarget.All, role);

    }
    [PunRPC]
    public void updateRoleRPC(int role)
    {

        switch (role)
        {

            case 0:
                this.role = new Alien();
                if (view.IsMine)
                {
                    foreach (PlayerScript player in (PlayerScript[]) FindObjectsOfType(typeof(PlayerScript)))
                    {
                        player.Objects.indicator.enabled = true;

                    }
                } else
                {
                    foreach (PlayerScript player in (PlayerScript[])FindObjectsOfType(typeof(PlayerScript)))
                    {
                        player.Objects.indicator.enabled = false;

                    }
                }

                break;
            case 1:
                this.role = new Doctor();
                break;
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

    public LayerMask HealingMachine;

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

    public GameObject roleDisplay;

    public GameObject results;

    public SpriteRenderer indicator;
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