using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tether : MonoBehaviour
{
    public Rigidbody rb;
    public Vector3 Angular, Direction;

    void FixedUpdate() {

	Direction.x = -2 * Time.deltaTime;
	transform.position += transform.forward * Direction.x;
    }
}
