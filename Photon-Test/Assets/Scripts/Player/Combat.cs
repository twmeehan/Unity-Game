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

    }
    public void EnableRagdoll()
    {
        animator.enabled = false;
        joint.enabled = true;
        collider.enabled = false;
        body_rb.velocity = new Vector2(30,30);
        controller.name.enabled = false;

    }
    public void EnableRagdoll(float direction)
    {
        animator.enabled = false;
        joint.enabled = true;
        collider.enabled = false;
        controller.use.currentlyHealing = false;
        body_rb.velocity = new Vector2(30 * direction, 30);
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
