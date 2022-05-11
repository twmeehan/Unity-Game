using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Class Master - attached to the character owned by the master client. Is used to give updates
// to all other controllers and sync day/night cycles.
public class Master : MonoBehaviour
{

    private Controller controller;
    private System.Random rand = new System.Random();
    private List<int> roles = new List<int>() { 1, 1, 1 };
    // the length of time that all players have been sleeping for
    private float timeAllPlayersSleeping = 0.0f;

    // Method Awake() - gets a reference to the controller
    void Awake()
    {
        controller = this.gameObject.GetComponent<Controller>();
    }

    // Method Update() -  is called once per frame
    void Update()
    {

        if (controller.transitionState == (int) States.startingGame && controller.timer.stopwatch.time > 5.0f)
        {

            controller.transitionState = (int)States.none;

            // TODO: remove display role screen

        } 
        /*else if (controller.transitionState == (int) States.transitioningToNight)
        {
            CheckToEndTransitionNight();
        } else if (controller.transitionState == (int)States.displayingRoles)
        {
            CheckToTransitionToDay();
        } else if (controller.transitionState == (int)States.none && controller.GetDay())
        {
            if (CheckIfAllPlayersAreSleeping())
                timeAllPlayersSleeping += Time.deltaTime;
            else
                timeAllPlayersSleeping = 0;
            if (!controller.timer.IsRunning() || timeAllPlayersSleeping > 0.25f)
            {
                // call EndTransitionToNight() on all Controllers
                PhotonNetwork.RaiseEvent(2, null, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);

            }
        }*/

    }

    public void StartGame(List<Controller> players)
    {

        // start a stopwatch and display roles for 5 seconds
        controller.timer.stopwatch.Reset();

        // set timer
        object[] content = new object[] { 127, PhotonNetwork.Time };
        PhotonNetwork.RaiseEvent(3, content, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);

        // TODO: display roles on screen
        AssignRoles(players);

    }
    public void AssignRoles(List<Controller> players)
    {

        int i = rand.Next(players.Count);
        int j;
        players[i].SetRole(0);
        players.RemoveAt(i);
        foreach (Controller player in players)
        {
            j = rand.Next(roles.Count);
            player.SetRole(roles[j]);
            roles.RemoveAt(j);
        }

    }
    // Method CheckToTransitionToDay() - if the screen has finished fading to black and are players
    // have been assigned to beds then begin waking the player up again
    public void CheckToTransitionToDay()
    {

        if (controller.timer.stopwatch.time > 5f)
        {
            controller.timer.stopwatch.Pause();
            controller.timer.stopwatch.time = 0;

            controller.transitionState = (int) States.none;

            // reset healing pod
            ((HealingMachineScript[])FindObjectsOfType(typeof(HealingMachineScript)))[0].available = true;
            ((HealingMachineScript[])FindObjectsOfType(typeof(HealingMachineScript)))[0].updateOtherClients();

            // call StartTransitionToDay() on all Controllers
            PhotonNetwork.RaiseEvent(5, null, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);

/*            List<Controller> players = ((Controller[])FindObjectsOfType(typeof(Controller))).ToList<Controller>();
            foreach (Controller player in players)
            {
                player.SetSleeping(false);
            }*/
        }
    }

    // Method EndTransitionToDay() - called staticly by Controller, sets a timer for the day
    public static void EndTransitionToDay()
    {

        object[] content = new object[] { 91, PhotonNetwork.Time };
        PhotonNetwork.RaiseEvent(3, content, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
       
    }

    // Method CheckToEndTransitionNight() - if the screen has finished fading to black and are players
    // have been assigned to beds then begin waking the player up again
    public void CheckToEndTransitionNight()
    {

        if (controller.transition.GetCurrentAnimatorStateInfo(0).normalizedTime > 1
            && controller.transition.GetCurrentAnimatorStateInfo(0).IsName("Fade") && CheckIfAllPlayersAreSleeping())
        {

            controller.SetSleeping(true);
            // set timer
            object[] content = new object[] { 31, PhotonNetwork.Time };
            PhotonNetwork.RaiseEvent(3, content, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);

            content = SpreadVirus();
            // call EndTransitionToNight() on all Controllers
            PhotonNetwork.RaiseEvent(6, content, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);

        }
    }

    // Method
    public object[] SpreadVirus()
    {

        List<string> newlyInfectedPlayers = new List<string>();

        foreach (RoomScript room in FindObjectsOfType(typeof(RoomScript)))
        {

            List<Controller> uninfectedPlayers = new List<Controller>();

            foreach (Controller player in room.GetPlayers())
            {
                if (!player.GetInfected())
                    uninfectedPlayers.Add(player);
            }

            if (uninfectedPlayers.Count != room.GetPlayers().Count && uninfectedPlayers.Count != 0)
            {
                newlyInfectedPlayers.Add(uninfectedPlayers[rand.Next(uninfectedPlayers.Count)].view.Owner.UserId);
            }

        }

        return newlyInfectedPlayers.ToArray();

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
