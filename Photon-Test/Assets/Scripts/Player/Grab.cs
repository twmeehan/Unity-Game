using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grab : MonoBehaviour
{

    private Controller controller;
    private PhotonView toBeDeleted;
    // Start is called before the first frame update
    void Start()
    {
        controller = this.gameObject.GetComponent<Controller>();
    }

    // Update is called once per frame
    void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current.qKey.wasPressedThisFrame && !controller.ragdoll && !controller.holdingLog)
        {

            RaycastHit2D log = Physics2D.Raycast(controller.transform.position, Vector2.down, 1.5f, (int)Layers.log);
            if (log.collider != null)
            {
                controller.AddLog();
                log.collider.gameObject.GetComponent<PhotonView>().RequestOwnership();
                if (log.collider.gameObject.GetComponent<PhotonView>().Owner == PhotonNetwork.LocalPlayer) {
                    PhotonNetwork.Destroy(log.collider.gameObject.GetComponent<PhotonView>());
                }
                else
                {
                    toBeDeleted = log.collider.gameObject.GetComponent<PhotonView>();
                }

            }
        } else if (UnityEngine.InputSystem.Keyboard.current.qKey.wasPressedThisFrame && !controller.ragdoll && controller.holdingLog)
        {
            controller.RemoveLog();
            GameObject newLog = PhotonNetwork.Instantiate(controller.log.name, transform.position, Quaternion.identity);
            newLog.GetComponent<Rigidbody2D>().velocity = new Vector2(10 * controller.character.transform.localScale.x, 10);
            newLog.GetComponent<Rigidbody2D>().angularVelocity = 150;

        }
        if (toBeDeleted != null) {

            if (toBeDeleted.Owner.Equals(PhotonNetwork.LocalPlayer))
            {
                PhotonNetwork.Destroy(toBeDeleted);
                toBeDeleted = null;
            }
            else
            {
            }
        }
    }
}
