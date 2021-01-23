using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{

    public Rigidbody rb;
    public GameObject Bullet;

    public const float MaxSpeed = 200f, Acceleration = 1f, ForwardAccel = 15f, MaxRotation = 4f, RotationAccel = 1f;
    public Vector3 Angular,Direction;
    private Vector2 lookInput, screenCenter, mouseDistance;

    bool wasDown = false;
    // Start is called before the first frame update

    void Start()
    {
        Debug.Log("Hello World");
        screenCenter.x = Screen.width * 0.5f;
        screenCenter.y = Screen.height * 0.5f;

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        Debug.Log(Time.deltaTime);

        lookInput.x = Input.mousePosition.x;
        lookInput.y = Input.mousePosition.y;

        mouseDistance.x = (lookInput.x - screenCenter.x) / screenCenter.y;
        mouseDistance.y = (lookInput.y - screenCenter.y) / screenCenter.y;

        mouseDistance = Vector2.ClampMagnitude(mouseDistance, 1f);
        
        if (Input.GetKey("w"))
        {
            Direction.x = Mathf.Lerp(Direction.x, MaxSpeed, ForwardAccel * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Direction.x > 30)
            {
                Direction.x -= 20 * Time.deltaTime;

            }
        }
        if (mouseDistance.magnitude > 0.5)
        {
            Angular.x = 0;
        }
        Angular.Set(Angular.x,
                mouseDistance.y * Time.deltaTime * 200,
                mouseDistance.x * Time.deltaTime * 200);
        if (Input.GetKey("a"))
        {
            Angular.Set(Mathf.Lerp(Angular.x, -MaxRotation, RotationAccel * Time.deltaTime), 
                Angular.y, 
                Angular.z);
        }
        if (Input.GetKey("d"))
        {
            Angular.Set(Mathf.Lerp(Angular.x, MaxRotation, RotationAccel * Time.deltaTime),
                Angular.y,
                Angular.z);
        }
        transform.Rotate(Angular, Space.Self);
        transform.position += transform.right * Direction.x * -1 * Time.deltaTime;

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

    }
}
