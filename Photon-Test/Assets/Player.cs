using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Player : MonoBehaviour
{

    public float speed = 100;
    PhotonView view;
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        view = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {

        if (view.IsMine)
        {

        }
        if (Input.GetKey("up"))
        {
            Vector3 input = new Vector3(transform.position.x, 1);
            transform.position += input * speed * Time.deltaTime;
        }

        if (Input.GetKey("down"))
        {
            Vector3 input = new Vector3(transform.position.y, 1);
            transform.position += input * speed * Time.deltaTime;
        }
       // Vector3 input = new Vector3(Input.GetButton.x - transform.position.x, Input.GetButtonUp.y - transform.position.y, 0);
       // transform.position += input * speed * Time.deltaTime;

    }
}
