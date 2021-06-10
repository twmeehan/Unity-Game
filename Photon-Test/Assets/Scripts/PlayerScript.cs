using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Class - attached to CharacterPrefab; contains controls for moving as well as
/// RPCs for player rotation
/// </summary>
public class Player : MonoBehaviour
{

    // For Jump
    public Transform IsGroundedChecker;
    public float CheckGroundRadius;
    public LayerMask GroundLayer;

    public float Speed;
    public float JumpForce;
    
    // Username displayed above character
    public TextMeshProUGUI PlayerName;

    // Slef
    public GameObject Character;
    
    // Player components
    public GameObject Camera;
    public Rigidbody2D rb;
    PhotonView View;
    Animator Anim;

    bool facingLeft = false;

    // Method - sets the nickname of all players and gets components
    void Start()
    {

        Anim = GetComponent<Animator>();
        View = GetComponent<PhotonView>();

        if (View.IsMine)
        {
            PlayerName.text = PhotonNetwork.NickName;
            Camera.SetActive(true);
        } else
        {
            PlayerName.text = View.Owner.NickName;

        }
        
    }

    // Method - updated every frame and handles movement
    void Update()
    {
        // If player falls too far teleport them back up
        if (transform.position.y < -3000)
        {
            transform.position = new Vector3(200, 3000, 0);
        }

        if (View.IsMine)
        {
            // Left-Right
            float x = Input.GetAxisRaw("Horizontal");

            if (x > 0)
            {
                if (facingLeft)
                {
                    View.RPC("faceRight", RpcTarget.All);
                    facingLeft = false;
                }
            } else if (x < 0)
            {
                if (!facingLeft)
                {
                    View.RPC("faceLeft", RpcTarget.All);
                    facingLeft = true;
                }
            }
            rb.velocity = new Vector2(x * Speed, rb.velocity.y);

            // Jump
            if (Input.GetKeyDown(KeyCode.W) && CheckIfGrounded())
            {

                rb.AddForce(new Vector2(0, JumpForce));

            }
            
        }

    }
    // Method - checks if player is in contact with ground layer
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

    /// <summary>
    /// Methods - when players turn they send a RPC to all other clients to tell them
    /// to invert the turned player on their client side
    /// </summary>
    [PunRPC] public void faceLeft()
    {
        Character.transform.localScale = new Vector3(-1f, 1f, 1f);
    }
    [PunRPC]
    public void faceRight()
    {
        Character.transform.localScale = new Vector3(1f, 1f, 1f);
    }

}
