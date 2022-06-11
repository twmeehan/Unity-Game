using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Use : MonoBehaviour
{

    private Controller controller;
    private Timer timer = new Timer();
    public bool currentlyHealing;


    // Start is called before the first frame update
    void Start()
    {
        controller = this.gameObject.GetComponent<Controller>();
    }

    // Update is called once per frame
    void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current.eKey.wasPressedThisFrame && controller.view.IsMine && !controller.kicking && !controller.ragdoll && !controller.sleeping)
        {

            if (controller.holdingStaff)
            {

                controller.animations.SetTrigger("Heal");
                timer.stopwatch.Reset();
                currentlyHealing = true;
                controller.movement.frozen = true;

            } else
            {
                RaycastHit2D player = Physics2D.Raycast(controller.transform.position + new Vector3(controller.character.transform.localScale.x, 0, 0), new Vector2(controller.character.transform.localScale.x, 0), 0.2f, (int)Layers.collider);
                controller.kicking = true;
                controller.animations.SetTrigger("Kick");
                if (player.collider != null)
                {
                    player.collider.gameObject.GetComponent<Controller>().Hit(controller.character.transform.localScale.x);
                }
            }
            


        }

        if (UnityEngine.InputSystem.Keyboard.current.eKey.isPressed && controller.view.IsMine && !controller.kicking && !controller.ragdoll && !controller.sleeping && currentlyHealing)
        {
            
            timer.stopwatch.Update();
            if (timer.stopwatch.time > 2)
            {
                currentlyHealing = false;
                foreach (Controller player in (Controller[])FindObjectsOfType(typeof(Controller)))
                {
                    if (Mathf.Abs(this.controller.transform.position.x - player.transform.position.x) < 5)
                    {
                        player.Revive();
                    }
                }
            }

        } else
        {
            if (currentlyHealing)
            {
                controller.animations.SetTrigger("Idle");

            }
            currentlyHealing = false;
            controller.movement.frozen = false;
        }
    }
}
