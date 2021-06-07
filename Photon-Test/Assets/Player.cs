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
            if (Input.GetKey(KeyCode.A))
            {
                Vector3 input = new Vector3(-60,0, 0);
                transform.position += input * speed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.D))
            {
                Vector3 input = new Vector3(60,0, 0);
                transform.position += input * speed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.W))
            {
                Vector3 input = new Vector3(0, 60, 0);
                transform.position += input * speed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.S))
            {
                Vector3 input = new Vector3(0, -60, 0);
                transform.position += input * speed * Time.deltaTime;
            }
        }
        //Vector3 input = new Vector3(Input.mousePosition.x - transform.position.x, Input.mousePosition.y - transform.position.y, 0);
        //transform.position += input * speed * Time.deltaTime;

    }
}
