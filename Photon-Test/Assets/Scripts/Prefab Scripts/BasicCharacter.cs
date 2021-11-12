using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCharacter : MonoBehaviour
{
    Transform transform;
    Rigidbody2D rb;
    public float speed;
    public float jump_strength;
    public LayerMask GroundLayer;
    public LayerMask RoomLayer;



    // Start is called before the first frame update
    void Start()
    {
        transform = this.gameObject.GetComponent<Transform>();
        rb = this.gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
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
            rb.velocity = new Vector2(0.0f, jump_strength);
        }
        if (rb.velocity.y < 0)
        {
            rb.gravityScale = 6;
        } else
        {
            rb.gravityScale = 2;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += new Vector3(-1 * speed * Time.deltaTime, 0, 0);
        } 

        if (Input.GetKey(KeyCode.D))
        {
            transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
        }
    }
}
