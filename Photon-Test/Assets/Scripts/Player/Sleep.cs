using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sleep : MonoBehaviour
{

    private Controller controller;
    private Timer timer;
    private void Start()
    {
        timer = new Timer();
        controller = this.gameObject.GetComponent<Controller>();
        timer.stopwatch.Start();

    }
    // Update is called once per frame
    void Update()
    {
        timer.stopwatch.Update();
        if (UnityEngine.InputSystem.Keyboard.current.qKey.wasPressedThisFrame && timer.stopwatch.time > 1.0f && !controller.sleeping)
        {
            timer.stopwatch.Reset();
            RaycastHit2D shelter = Physics2D.Raycast(controller.transform.position, Vector2.down, 0.1f, (int)Layers.room);
            if (shelter.collider != null)
            {
                if (shelter.collider.gameObject.GetComponent<Shelter>().AttemptToJoinShelter(this.controller))
                {
                    this.controller.SleepInShelter(shelter.collider.gameObject.GetComponent<Shelter>());
                    this.controller.animations.SetTrigger("Sleep");
                    // set player to sleeping
                }

            }
        }
    }
}
