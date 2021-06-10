using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{

    public Transform isGroundedChecker;
    public float checkGroundRadius;
    public LayerMask groundLayer;

    public float speed;
    public float jumpForce;
    PhotonView view;
    Animator anim;
    public TextMeshProUGUI playerName;
    public GameObject character;
    bool facingLeft = false;
    public GameObject camera;
    public Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        view = GetComponent<PhotonView>();
        if (view.IsMine)
        {
            playerName.text = PhotonNetwork.NickName;
            camera.SetActive(true);
        } else
        {
            playerName.text = view.Owner.NickName;

        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -3000)
        {
            transform.position = new Vector3(200, 3000, 0);
        }
        if (view.IsMine)
        {
            float x = Input.GetAxisRaw("Horizontal");
            if (x > 0)
            {
                if (facingLeft)
                {
                    view.RPC("faceRight", RpcTarget.All);
                    facingLeft = false;
                }
            } else if (x < 0)
            {
                if (!facingLeft)
                {
                    view.RPC("faceLeft", RpcTarget.All);
                    facingLeft = true;
                }
            }
            rb.velocity = new Vector2(x * speed, rb.velocity.y);
            if (Input.GetKeyDown(KeyCode.W) && CheckIfGrounded())
            {
                Debug.Log("jump");
                rb.AddForce(new Vector2(0, jumpForce));
            }
            
        }
        //Vector3 input = new Vector3(Input.mousePosition.x - transform.position.x, Input.mousePosition.y - transform.position.y, 0);
        //transform.position += input * speed * Time.deltaTime;

    }
    bool CheckIfGrounded()
    {

        Collider2D collider = Physics2D.OverlapCircle(isGroundedChecker.position, checkGroundRadius, groundLayer);
        if (collider != null)
        {
            
            return true;
        }
        else
        {
            return false;
            
        }
    }

    [PunRPC] public void faceLeft()
    {
        character.transform.localScale = new Vector3(-1f, 1f, 1f);
    }
    [PunRPC]
    public void faceRight()
    {
        character.transform.localScale = new Vector3(1f, 1f, 1f);
    }
}
