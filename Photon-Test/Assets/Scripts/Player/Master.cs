using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Master : MonoBehaviour
{

    Controller controller;

    // Start is called before the first frame update
    void Awake()
    {
        controller = this.gameObject.GetComponent<Controller>();
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.transitionState == (int) States.transitioningToNight)
        {
            CheckToEndTransitionNight();
        }
    }
    // Method CheckToEndTransitionNight() - if the screen has finished fading to black and are players
    // have been assigned to beds then begin waking the player up again
    public void CheckToEndTransitionNight()
    {

        if (controller.transition.GetCurrentAnimatorStateInfo(0).normalizedTime > 1
            && controller.transition.GetCurrentAnimatorStateInfo(0).IsName("Fade") && CheckIfAllPlayersAreSleeping())
        {
            // set timer
            object[] content = new object[] { 31, PhotonNetwork.Time };
            PhotonNetwork.RaiseEvent(3, content, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);

            // call EndTransitionToNight() on all Controllers
            PhotonNetwork.RaiseEvent(6, null, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);

        }
    }

    // Method CheckIfAllPlayersAreSleeping() - returns true if all players are in beds
    private bool CheckIfAllPlayersAreSleeping()
    {
        foreach (Controller player in FindObjectsOfType(typeof(Controller)))
        {
            if (!player.GetSleeping())
                return false;
        }
        return true;
    }
}
