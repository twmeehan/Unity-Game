using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class Particles - attached to player, creates ambient particles
public class Particles : MonoBehaviour
{
    // trigger burst when player stops moving
    public ParticleSystem burst;

    public ParticleSystem landParticles;
    public TrailRenderer cape;

    // used for the particle burst to mask cape, checks if player has been moving for at least 0.5 seconds
    private float timeSinceMove = 0.0f;
    private Movement movement;
    private Rigidbody2D rb;
    

    // Start is called before the first frame update
    void Start()
    {
        movement = this.gameObject.GetComponent<Movement>();
        rb = this.gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

        // LAND ---------------------------------------------------

        ParticleSystem.VelocityOverLifetimeModule vel = landParticles.velocityOverLifetime;

        // check if play is grounded and set isGrounded and timeSinceGrounded
        bool isGrounded = Physics2D.OverlapCircle(movement.required.feetPosition.position, movement.required.distanceFromGround, (int)Layers.ground);
        if (isGrounded && rb.velocity.y < -10.0f)
        {
            landParticles.emission.SetBurst(0, new ParticleSystem.Burst(0, -Mathf.Ceil(rb.velocity.y)/2 - 5));
            vel.yMultiplier = -Mathf.Ceil(rb.velocity.y) / 14;
            landParticles.Play();
        }

        // TRAIL ------------------------------------------------------

        ParticleSystem.EmissionModule emission = landParticles.emission;
        if (isGrounded && rb.velocity.x > 0.4f)
            emission.rate = 4;
        else
            emission.rate = 0;


        // CAPE ----------------------------------------------------

        // enable cape
        cape.time = 1;

        // if player has been moving for 0.5+ seconds
        if (timeSinceMove > 0.5f && isGrounded && !(UnityEngine.InputSystem.Keyboard.current.aKey.isPressed || UnityEngine.InputSystem.Keyboard.current.dKey.isPressed) && cape.enabled)
        {
            timeSinceMove = 0;
            cape.time = 0;
            burst.Play();
        } else if (!(UnityEngine.InputSystem.Keyboard.current.aKey.isPressed || UnityEngine.InputSystem.Keyboard.current.dKey.isPressed))
        {
            timeSinceMove = 0;
        }
        else 
            timeSinceMove += Time.deltaTime;

    }
}
