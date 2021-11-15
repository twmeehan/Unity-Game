using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BasicCharacter : MonoBehaviour
{
    Transform transform;
    Rigidbody2D rb;
    PhotonView view;
    public float speed;
    public float jump_strength;
    public LayerMask GroundLayer;
    public LayerMask RoomLayer;
    public GameObject camera;
    public TextMeshProUGUI name;

    float deltaTime;



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

            bool isGrounded = false;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1f, GroundLayer);
            if (hit.collider != null)
            {

                isGrounded = true;

            }
            RaycastHit2D room = Physics2D.Raycast(transform.position, Vector2.up, 0.1f, RoomLayer);
            if (room.collider != null)
            {
                Debug.Log(room.collider.gameObject.name);
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
                rb.velocity = new Vector2(-1 * speed , rb.velocity.y);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                rb.velocity = new Vector2(speed, rb.velocity.y);
            } else
            {
                rb.velocity = new Vector2(0, rb.velocity.y);

            }

            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            float fps = 1.0f / deltaTime;
            Debug.Log(Mathf.Ceil(fps).ToString());

        }
        
    }
}
