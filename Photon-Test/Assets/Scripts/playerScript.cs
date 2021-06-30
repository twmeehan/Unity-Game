using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerScript : MonoBehaviourPunCallbacks, IPunObservable
{


    private float grav = 0;
    public float dragConstant = 0.2f;
    private Vector2 drag = new Vector2(0,0);

    // For Move
    private float pastInput = 0;
    private float moveSpeed = 0;
    public float speed = 2;
    public float moveDecelTime = 5.0f;
    public float moveAccelTime = 0.05f;
    public float moveStartTime = 0;
    

    // For Jump
    private bool jumping = false;
    private bool doubleJump = true;
    public float distance = 0.3f;
    //public Transform IsGroundedChecker;
    public LayerMask GroundLayer;
    public float jumpForce = 3.0f;

    [SerializeField] private Vector2 accel = new Vector2(0,0);
    [SerializeField] private Vector2 vel = new Vector2(0,0);
    [SerializeField] private Vector2 pos = new Vector2(0,0);

    // private Vector2 predictedPos = new Vector2(0, 0);
    
    private Transform transform;
    private Rigidbody2D rb;
    private PhotonView view;

    // Start is called before the first frame update
    void Start()
    {

        transform = this.gameObject.GetComponent<Transform>();
        rb = this.gameObject.GetComponent<Rigidbody2D>();
        view = this.gameObject.GetComponent<PhotonView>();

        PhotonNetwork.SerializationRate = 13;

    }
    void Update()
    {
        if (view.IsMine && Input.GetKeyDown(KeyCode.Space))
        {
            if (IsGrounded())
            {

                doubleJump = true;
                jumping = true;

            } else if (doubleJump)
            {
                rb.velocity = new Vector2(0.0f,0.0f);
                doubleJump = false;
                jumping = true;

            }

        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (view.IsMine)
        {
            vel = rb.velocity;
            jump();
            move(Input.GetAxisRaw("Horizontal"));
            rb.velocity = vel;
            gravity();
            CalcAccel();
            

            pos = transform.position;

        } else
        {
            transform.position = pos;

        }
        

    }
    public void jump()
    {
        if (jumping)
        {
            vel += new Vector2(0.0f, jumpForce);
            jumping = false;
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
            grav = 6.0f;
        } else
        {
            grav = 15.0f;
        }
    }
    public void move(float input)
    {
        if (IsGrounded())
        {
            Debug.Log(input);
            if (input == 0)
            {
                moveSpeed = Mathf.Lerp(moveSpeed, 0, moveDecelTime*Time.deltaTime);
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
            vel = new Vector2(moveSpeed * speed, vel.y);
            doubleJump = true;
            Debug.Log(jumping);
        }
        else if (doubleJump != true){
            vel = new Vector2(moveSpeed * speed, vel.y);
            Debug.Log("Not Grounded: DoubleJump"); 
        }
        else{
            Debug.Log("Not Grounded");
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

            float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.timestamp));
            lagComp(pos, vel, accel, lag);
            
        }

    }
    public void lagComp(Vector2 pos, Vector2 vel, Vector2 accel, float t)
    {
        this.pos += pos + (0.5f * accel * t * t + vel * t);
        this.vel = vel + (accel * t);
    }

    bool IsGrounded()
    {

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, distance, GroundLayer);
        if (hit.collider != null)
        {
            Debug.Log("On Ground");
            return true;
            
        }

        return false;
    }

}
