using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{

    public Rigidbody rb;
    public GameObject Bullet;

    public const float BaseSpeed = 100f, Acceleration = 1f, MaxSpeed = 15f;
    public Vector3 Anglular,Direction;
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

        transform.position += transform.right * BaseSpeed * -1 * Time.deltaTime;
        lookInput.x = Input.mousePosition.x;
        lookInput.y = Input.mousePosition.y;

        mouseDistance.x = (lookInput.x - screenCenter.x) / screenCenter.y;
        mouseDistance.y = (lookInput.y - screenCenter.y) / screenCenter.y;

        mouseDistance = Vector2.ClampMagnitude(mouseDistance, 1f);

        transform.Rotate(0f, mouseDistance.y * Time.deltaTime * 200, mouseDistance.x * Time.deltaTime * 200, Space.Self);
        if (Input.GetKey("a"))
        {
            transform.Rotate(-4, 0, 0, Space.Self);
        }
        if (Input.GetKey("d"))
        {
            transform.Rotate(4, 0, 0, Space.Self);
        }
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
