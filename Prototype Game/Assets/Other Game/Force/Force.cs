using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Force : MonoBehaviour
{
    public Vector3 Angular, Direction;
    public Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
 
    }

    // Update is called once per frame
    void FixedUpdate()
    {
	if (Input.GetKey("w"))
	{
		Direction.x = 5 * Time.deltaTime;
        	transform.position += transform.forward * Direction.x;
	}

	else if (Input.GetKey("s"))
	{
		Direction.x = -5 * Time.deltaTime;
		transform.position += transform.forward * Direction.x;
	}

	else
	{
		Direction.x = 0;
	}

	if (Input.GetKey("a"))
	{
		Direction.x = -3 * Time.deltaTime;
		transform.position += transform.right * Direction.x;
	}

	else if (Input.GetKey("d"))
	{
	        Direction.x = 3 * Time.deltaTime;
                transform.position += transform.right * Direction.x;
	}

	else
	{
		Direction.x = 0;
	}

    }
}
