using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Face : MonoBehaviour
{

    private Animator anim;
    private float timeSinceLastAnimation;
    System.Random rand = new System.Random();

    // Start is called before the first frame update
    void Start()
    {
        anim = this.gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        timeSinceLastAnimation += Time.deltaTime;

        if (timeSinceLastAnimation >= 0.25 && anim.GetCurrentAnimatorStateInfo(0).IsName("Blink"))
        {
            anim.SetTrigger("Rest");
        }
        if (timeSinceLastAnimation >= 0.4 && anim.GetCurrentAnimatorStateInfo(0).IsName("Wink"))
        {
            anim.SetTrigger("Rest");
        }
        if (timeSinceLastAnimation >= 0.8 && anim.GetCurrentAnimatorStateInfo(0).IsName("Happy"))
        {
            anim.SetTrigger("Rest");
        }
        if (rand.NextDouble() < -100/(100+timeSinceLastAnimation) + 0.97)
        {

            int random = rand.Next(2);
            if (random == 0)
            {
                timeSinceLastAnimation = 0;
                anim.SetTrigger("Blink");
            } else
            {
                timeSinceLastAnimation = 0;
                anim.SetTrigger("Happy");
            }

        }
    }
}
