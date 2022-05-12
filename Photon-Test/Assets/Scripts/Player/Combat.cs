using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour
{

    public Animator animator;
    public WheelJoint2D joint;
    public CapsuleCollider2D collider;
    public Rigidbody2D body_rb;
    private Controller controller;
    

    // Start is called before the first frame update
    void Start()
    {
        controller = this.gameObject.GetComponent<Controller>();
    }

    // Update is called once per frame
    void Update()
    {

        if (controller.ragdoll && animator.enabled)
        {
            EnableRagdoll();
        } else if (!controller.ragdoll && !animator.enabled)
        {
            DisableRagdoll();
        }

        if (UnityEngine.InputSystem.Keyboard.current.eKey.wasPressedThisFrame && controller.view.IsMine)
        {

            RaycastHit2D player = Physics2D.Raycast(controller.transform.position + new Vector3(controller.character.transform.localScale.x, 0,0), new Vector2(controller.character.transform.localScale.x,0), 0.2f, (int)Layers.collider);

            if (player.collider != null)
            {
                player.collider.gameObject.GetComponent<Controller>().PickUp();
            }
            

        }
    }
    public void EnableRagdoll()
    {
        animator.enabled = false;
        joint.enabled = true;
        collider.enabled = false;
        body_rb.velocity = Vector2.zero;
        controller.name.enabled = false;

    }
    public void DisableRagdoll()
    {
        animator.enabled = true;
        joint.enabled = false;
        collider.enabled = true;
        controller.name.enabled = true;

    }
}
