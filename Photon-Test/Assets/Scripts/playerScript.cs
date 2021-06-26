using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerScript : MonoBehaviourPunCallbacks, IPunObservable
{

    private  float speed = 2;
    public float jumpForce;
    public float grav = -3;
    public static float dragConstant = 0.2f;
    public Vector2 drag = new Vector2(0,0);
    public float previousDownVel = 0;

    // For Move
    public float moveSpeed = 0;
    private float moveDecelTime = 5.0f;
    private float moveAccelTime = 0.05f;
    private float moveStartTime = 0;
    private float pastInput = 0;

    // For Jump
    public Transform IsGroundedChecker;
    public static float CheckGroundRadius = 0.18f;
    public LayerMask GroundLayer;

    [SerializeField] private Vector2 accel = new Vector2(0,0);
    [SerializeField] private Vector2 vel = new Vector2(0,0);
    [SerializeField] private Vector2 pos = new Vector2(0,0);

    private Vector2 predictedPos = new Vector2(0, 0);
    
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

    // Update is called once per frame
    void FixedUpdate()
    {
        if (view.IsMine)
        {

            move(Input.GetAxisRaw("Horizontal"));

            drag = dragConstant * rb.velocity * -1;
            /**
            if (CheckIfGrounded() && (previousDownVel - rb.velocity.y) > 1)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
            } else
            {
                accel = new Vector2(0, grav) + drag;
                Debug.Log("Grav");
            }

            rb.velocity += accel * Time.deltaTime;
            vel = rb.velocity;
            pos = transform.position;

            previousDownVel = rb.velocity.y;

            */
        } else
        {
            transform.position = pos;
            rb.velocity = vel;

        }
        

    }
    public void move(float input)
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
        rb.velocity = new Vector2(moveSpeed * speed, vel.y);
        
    }
    public void move()
    {
        
    }
    public void interpolate()
    {

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

    bool CheckIfGrounded()
    {
        
        Collider2D collider = Physics2D.OverlapCircle(IsGroundedChecker.position, CheckGroundRadius, GroundLayer);
        if (collider != null)
        {
            return false;
        }
        else
        {
            return true;
        }

    }

}
