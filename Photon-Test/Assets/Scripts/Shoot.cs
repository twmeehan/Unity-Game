using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public GameObject Bullet;
    public PhotonView view;
    public Camera camera;

    // Update is called once per frame
    void Update()
    {
        //Debug.Log((Input.mousePosition - camera.WorldToScreenPoint(view.gameObject.transform.position)).normalized);
        if (Input.GetMouseButtonDown(0) && view.IsMine)
        {
            Bullet = PhotonNetwork.Instantiate("Bullet", transform.position, Quaternion.identity);
            Bullet.SendMessage("setCreator", this.gameObject);
            Bullet.GetComponent<Rigidbody2D>().velocity = Vector2.Scale((Input.mousePosition - camera.WorldToScreenPoint(view.gameObject.transform.position)).normalized, new Vector2(10.0f, 10.0f));
        }
    }
}
