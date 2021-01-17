using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    float life;
    //public Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        transform.Rotate(0, -90, 0);
        transform.Rotate(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));
        life = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        //rb.AddForce(Mathf.Sin(rb.transform.eulerAngles.y * Mathf.Deg2Rad)*speed, 0, Mathf.Cos(rb.transform.eulerAngles.y * Mathf.Deg2Rad)*speed);
        
        transform.position += transform.forward * 1000 * Time.deltaTime;
        life += Time.deltaTime;
        Debug.Log(life);
        if (life > 3f)
        {
            Destroy(gameObject);
        }
    }
}
