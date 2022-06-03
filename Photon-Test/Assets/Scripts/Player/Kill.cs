using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kill : MonoBehaviour
{

    Controller controller;
    // Start is called before the first frame update
    void Start()
    {
        controller = this.gameObject.GetComponent<Controller>();
    }

    // Update is called once per frame
    void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current.rKey.wasPressedThisFrame && controller.view.IsMine && !controller.kicking && !controller.ragdoll && !controller.sleeping && controller.role == "killer")
        {

            RaycastHit2D player = Physics2D.Raycast(controller.transform.position + new Vector3(controller.character.transform.localScale.x, 0, 0), new Vector2(controller.character.transform.localScale.x, 0), 0.2f, (int)Layers.collider);
            controller.kicking = true;
            controller.animations.SetTrigger("Stab");
            if (player.collider != null)
            {
                player.collider.gameObject.GetComponent<Controller>().Kill(controller.character.transform.localScale.x);
            }


        }
    }
}
