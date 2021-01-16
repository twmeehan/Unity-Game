using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{

    public Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Hello World");
       
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey("s"))
        {
            rb.AddForce(Mathf.Sin(rb.transform.eulerAngles.y * Mathf.Deg2Rad) * 2000 * Time.deltaTime, 0, Mathf.Cos(rb.transform.eulerAngles.y * Mathf.Deg2Rad) * 2000 * Time.deltaTime, ForceMode.Acceleration);
            if (Input.GetKey("a"))
            {
                rb.transform.Rotate(0, 1, 0);
            }
            if (Input.GetKey("d"))
            {
                rb.transform.Rotate(0, -1, 0);
            }
        }
        if (Input.GetKey("w")) 
        {
            rb.AddForce(Mathf.Sin(rb.transform.eulerAngles.y * Mathf.Deg2Rad) *-2000* Time.deltaTime, 0, Mathf.Cos(rb.transform.eulerAngles.y * Mathf.Deg2Rad) *-2000* Time.deltaTime, ForceMode.Acceleration);
            if (Input.GetKey("a"))
            {
                rb.transform.Rotate(0, -1, 0);
            }
            if (Input.GetKey("d"))
            {
                rb.transform.Rotate(0, 1, 0);
            }
        }
        

    }
}
