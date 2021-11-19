using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BasicCharacter : MonoBehaviourPun, IPunObservable
{
    Transform transform;
    Rigidbody2D rb;
    PhotonView view;

    // variable constants
    public float speed;
    public float jump_strength;

    // layers
    public LayerMask GroundLayer;
    public LayerMask RoomLayer;
    public LayerMask BedLayer;

    // random Objects
    public GameObject camera;
    public TextMeshProUGUI name;
    public Button button;

    // private variables
    private bool infected = false;
    private string room;
    private bool sleeping = false;

    private int buttonType = 0;

    private int DISABLED = 0;
    private int GET_INTO_BED = 1;
    private int LEAVE_BED = 2;



    // Start is called before the first frame update
    void Start()
    {
        transform = this.gameObject.GetComponent<Transform>();
        rb = this.gameObject.GetComponent<Rigidbody2D>();
        view = this.gameObject.GetComponent<PhotonView>();
        if (view.IsMine)
        {
            camera.SetActive(true);
        }
        name.text = view.Owner.NickName;

        PhotonNetwork.SerializationRate = 20;
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (view.IsMine) {

            RaycastHit2D currentRoom = Physics2D.Raycast(transform.position, Vector2.up, 0.1f, RoomLayer);
            if (currentRoom.collider != null)
            {
                room = currentRoom.collider.gameObject.name;
            }

            if (!sleeping)
            {
                RaycastHit2D currentBed = Physics2D.Raycast(transform.position, Vector2.up, 0.1f, BedLayer);
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

                move();

            } else // runs if sleeping
            {
                RaycastHit2D currentBed = Physics2D.Raycast(transform.position, Vector2.up, 0.1f, BedLayer);
                if (currentBed.collider != null && !currentBed.collider.gameObject.GetComponent<BedScript>().player.Equals(this.gameObject.GetComponent<PhotonView>().Owner.UserId))
                    KickFromBed();
            }
        }
        
    }
    // Called by BedScript when a player successfully gets into a bed
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
        object[] objectArray = { t.position.x,t.position.y };
        view.RPC("JoinBedRPC", RpcTarget.All, objectArray as object);

    }
    public void KickFromBed()
    {
        // only run on controller's client

        rb.velocity = new Vector2(rb.velocity.x, jump_strength/2);

        // TODO: Play animation or add particles or smth
        sleeping = false;
    }

    // Called when the player presses the button to leave bed
    public void WakeUp()
    {
        RaycastHit2D currentBed = Physics2D.Raycast(transform.position, Vector2.up, 0.1f, BedLayer);
        currentBed.collider.gameObject.GetComponent<BedScript>().LeaveBed();
    }

    // Called when the player presses the button to enter bed
    public void Sleep()
    {

        RaycastHit2D currentBed = Physics2D.Raycast(transform.position, Vector2.up, 0.1f, BedLayer);
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
    void move()
    {
        bool isGrounded = false;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1f, GroundLayer);
        if (hit.collider != null)
        {

            isGrounded = true;

        }

        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jump_strength);
        }
        if (rb.velocity.y < 0)
        {
            rb.gravityScale = 6;
        }
        else
        {
            rb.gravityScale = 2;
        }
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            rb.velocity = new Vector2(-1 * speed, rb.velocity.y);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);

        }
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        Debug.Log("OnPhotonSerializeView Running");
        if (stream.IsWriting)
        {
            stream.SendNext(sleeping);
            stream.SendNext(infected);
            stream.SendNext(room);
        }
        else
        {
            sleeping = (bool) stream.ReceiveNext();
            infected = (bool) stream.ReceiveNext();
            room = (string) stream.ReceiveNext();
        }

    }
    
}
