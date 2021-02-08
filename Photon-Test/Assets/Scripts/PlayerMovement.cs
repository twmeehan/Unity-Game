using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerMovement : MonoBehaviourPunCallbacks
{ 
    private PhotonView PV;

    void Start()
    {
	    PV = GetComponent<PhotonView>();
    }
 	void Update()
    {
    if (PV.IsMine)
	Movement();

    if (Input.GetKeyDown(KeyCode.Escape))
        Application.Quit();
    }

    void Movement()
    {
        float yMov = Input.GetAxis("Vertical");
        float xMov = Input.GetAxis("Horizontal");
        transform.position += new Vector3(xMov / 7.5f, 0);
    }
}
