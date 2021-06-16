using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerScript : MonoBehaviourPunCallbacks, IPunObservable
{

    public float speed;
    public float jumpForce;

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

        PhotonNetwork.SerializationRate = 5;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (view.IsMine)
        {
            pos = transform.position;
            vel = rb.velocity;
            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0)
            {
                rb.velocity = new Vector3(Input.GetAxisRaw("Horizontal") * speed, rb.velocity.y);
            }
        } else
        {
            transform.position = pos;
            rb.velocity = vel;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(vel);
            stream.SendNext(pos);
        } else
        {
            vel = (Vector2) stream.ReceiveNext();
            pos = (Vector2) stream.ReceiveNext();
        }
    }

}
