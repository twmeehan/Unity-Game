using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody2D rb;
    public GameObject creator;
    Vector3 lastPos;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lastPos = transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        /*
        RaycastHit2D[] hits = Physics2D.LinecastAll((Vector2)transform.position + rb.velocity * Time.deltaTime * 2.0f, (Vector2) transform.position+rb.velocity*Time.deltaTime*-2.0f);
        foreach (RaycastHit2D hit in hits) {

            if (Physics2D.OverlapPoint(transform.position).gameObject != creator)
            {
                blast();
                Destroy(this.gameObject);
            }
        }
        */
        
        foreach (RaycastHit2D hit in Physics2D.RaycastAll(transform.position, rb.velocity, 0.1f))
        {

            Debug.Log(hit.collider.gameObject);
            if (hit.collider.gameObject != creator)
            {
                blast();
                Destroy(this.gameObject);
            }
        }


        lastPos = transform.position;
    }
    public void setCreator(GameObject name)
    {
        creator = name;
    }
    public void blast()
    {
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position, 2.0f))
        {

            try
            {
                float f = 20.0f * (1 - (collider.gameObject.transform.position -
                    transform.position).magnitude);
                collider.gameObject.SendMessage("applykb",
                    (Vector2)Vector3.Scale((collider.gameObject.transform.position -
                    transform.position).normalized, new Vector3(f,f,f)));

                    
                /*
                 * collider.gameObject.SendMessage("applykb",
                    (Vector2)Vector3.Scale((collider.gameObject.transform.position -
                    transform.position).normalized, new Vector3(5.0f,5.0f,5.0f)));
                 * new Vector3(5f * (1 - (collider.gameObject.transform.position -
                    transform.position).magnitude), 5f * (1 - (collider.gameObject.transform.position -
                    transform.position).magnitude), 5f * (1 - (collider.gameObject.transform.position -
                    transform.position).magnitude))
                 */

                //collider.gameObject.SendMessage("applykb", new Vector2(0.0f, 1.0f));
            }
            catch
            {
            }
        }

    }
}
