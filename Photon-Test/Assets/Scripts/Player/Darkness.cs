using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Darkness : MonoBehaviour
{

    Transform campfire;
    public Animator darkness;

    // Update is called once per frame
    void Update()
    {
        if (campfire == null)
        {
            Campfire[] campfires = (Campfire[])FindObjectsOfType(typeof(Campfire));

            if (campfires.Length > 0)
                campfire = campfires[0].gameObject.transform;
            
        } else
        {
            if (Mathf.Abs(transform.position.x - campfire.position.x) < 12)
            {
                darkness.SetBool("NearFire", true);
                foreach (Controller controller in FindObjectsOfType(typeof(Controller)))
                {
                    controller.usernameAnim.SetBool("Hide",false);
                }
            } else
            {
                darkness.SetBool("NearFire", false);
                foreach (Controller controller in FindObjectsOfType(typeof(Controller)))
                {
                    controller.usernameAnim.SetBool("Hide", true);
                }
            }
        }
    }
}
