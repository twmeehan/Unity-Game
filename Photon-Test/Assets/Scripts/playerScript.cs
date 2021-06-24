using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerScript : MonoBehaviourPunCallbacks, IPunObservable
{

    public float speed;
    public float jumpForce;
    public float grav = -3;
    public static float dragConstant = 0.2f;
    public Vector2 drag = new Vector2(0,0);

    // For Jump
    public Transform IsGroundedChecker;
    public static float CheckGroundRadius = 0.18f;
    public LayerMask GroundLayer;

    [SerializeField] private Vector2 accel = new Vector2(0,0);
    [SerializeField] private Vector2 vel = new Vector2(0,0);
    [SerializeField] private Vector2 pos = new Vector2(0,0);
    
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
            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0)
            {
                rb.velocity = new Vector3(Input.GetAxisRaw("Horizontal") * speed, vel.y);
                
            }
            drag = dragConstant * vel * -1;
            if (CheckIfGrounded())
            {
                accel = new Vector2(0, grav) * 0.5f + drag;
            } else
            {
                accel = new Vector2(0, grav) + drag;
                Debug.Log("Grav");
            }
            
            rb.velocity += accel * Time.deltaTime;
            vel = rb.velocity;
            pos = transform.position;

        } else
        {
            transform.position = pos;
            //vel += accel * Time.deltaTime;
            rb.velocity = vel;

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
            pos += 0.5f * accel * lag * lag + vel * lag;
        }
    }
    bool CheckIfGrounded()
    {

        Collider2D collider = Physics2D.OverlapCircle(IsGroundedChecker.position, CheckGroundRadius, GroundLayer);
        if (collider != null)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

}
