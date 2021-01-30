using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movementv3 : MonoBehaviour
{   

    public Rigidbody rb;
    public GameObject Bullet;

    public float MaxSpeed = 200f, Acceleration = 1f, MaxRotation = 4f, RotationAccel = 1f;
    public Vector3 Angular,Direction;
    private Vector2 lookInput, screenCenter, mouseDistance; 
    bool wasDown = false;
    bool shiftDown = false;
    bool wDown = false;
    bool aDown = false;
    bool dDown = false;

    // Start is called before the first frame update
    void Start()
    {
        screenCenter.x = Screen.width * 0.5f;
        screenCenter.y = Screen.height * 0.5f;
    
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        //Mouse Input
        lookInput.x = Input.mousePosition.x;
        lookInput.y = Input.mousePosition.y;
        mouseDistance.x = (lookInput.x - screenCenter.x) / screenCenter.y;
        mouseDistance.y = (lookInput.y - screenCenter.y) / screenCenter.y;
        mouseDistance = Vector2.ClampMagnitude(mouseDistance, 1f);
        
        //Brake with shift
        if (Input.GetKey(KeyCode.LeftShift) && !shiftDown)
        {	
        	Direction.x -= 100 * Time.deltaTime;
        }
        else if (!Input.GetKey(KeyCode.LeftShift))
        {
            //Move with Vector3 Direction.x
            //Direction.x means forward movement
            if (Input.GetKey("w") && !wDown)
            {
            	if (Direction.x > 200)
            	{
            		Direction.x = 100;
                	Direction.x += 100 * Time.deltaTime;
                }
                else
                {
                	Direction.x = 200;
                }
            }
            else if (!Input.GetKey("w"))
            {
                if (Direction.x > 0)
                {
            		Direction.x -= 5 * Time.deltaTime;
            	}
            	else
            	{
            		Direction.x = 0;
            	}
            }

        }
        
        //Tilt Left
        if (Input.GetKey("a") && !aDown)
        {
            Angular.Set(Mathf.Lerp(Angular.x, -MaxRotation, RotationAccel * Time.deltaTime), Angular.y, Angular.z);
        
        }
        else if (!Input.GetKey("a"))
        {
            //If "a" not down do nothing
        }
        // Tilt Right
        if (Input.GetKey("d") && !dDown)
        {
            Angular.Set(Mathf.Lerp(Angular.x, MaxRotation, RotationAccel * Time.deltaTime), Angular.y, Angular.z);
            
        }
        else if (!Input.GetKey("d"))
        {
            //If "d" not down do nothing 
        }

        

        //Set Angular.x to 0
        if (mouseDistance.magnitude > 0.5)
        {
            Angular.x = 0;
        }

        //Bullets
        if (Input.GetMouseButton(0) && !wasDown)    
        {
            wasDown = true;
            GameObject BulletObject = Instantiate(Bullet);
            BulletObject.transform.position = rb.transform.position;
            BulletObject.transform.rotation = rb.transform.rotation;
        } else if (!Input.GetMouseButton(0))
        {
            wasDown = false;
        }
    

        //PlayerMovement    
        transform.position += transform.right * Direction.x * -1 * Time.deltaTime;
        //PlayerRotation    
        transform.Rotate(Angular, Space.Self);    
        Angular.Set(Angular.x, mouseDistance.y * Time.deltaTime * 200, mouseDistance.x * Time.deltaTime * 200);
    }
}
