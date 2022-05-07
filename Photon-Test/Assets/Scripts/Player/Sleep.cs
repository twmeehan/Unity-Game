using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sleep : MonoBehaviour
{

    private Controller controller;

    private void Start()
    {
        controller = this.gameObject.GetComponent<Controller>();
    }
    // Update is called once per frame
    void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current.qKey.isPressed)
        {
            Debug.Log("q");

            RaycastHit2D campfire = Physics2D.Raycast(controller.transform.position, Vector2.down, 0.1f, (int)Layers.room);
            if (campfire.collider != null)
            {
                if (campfire.collider.gameObject.GetComponent<Shelter>().AttemptToJoinCampfire(this.controller))
                {
                    Debug.Log("sleep");
                    this.controller.animations.SetTrigger("Sleep");
                    // set player to sleeping
                }

            }
        }
    }
}
