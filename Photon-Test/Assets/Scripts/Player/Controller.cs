using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    #region private variables

    private bool sleeping = false;
    private bool day = true;
    private bool infected = false;

    private System.Random rand = new System.Random();
    #endregion

    #region public variables
    [Space(10)]
    [Header("To Be Replaced")]
    public GameObject resultsScreen;
    public GameObject roleText;

    [Space(10)]
    [Header("Required")]
    public Timer timer;
    public Movement movement;
    public Interact interact;
    public Animator transition;

    [Space(10)]
    [Header("Not Required")]
    public PhotonView view;
    public Role role;
    public int transitionState = 0;


    #endregion


    // Start is called before the first frame update
    void Start()
    {
        view = this.gameObject.GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transitionState == (int) States.transitioningToNight && view.IsMine)
        {
            TransitionToNight();
        }
    }

    // Method SwitchToNight() - called by OnEvent when master client sends the code to switch
    // all players to night. Puts remaining players into remaining beds and starts night animations
    public void StartTransitionToNight()
    {
        // only runs on the client that owns this Controller

        // transitionState used to ensure this method is only called once (aka. this method has cooldown)
        if (transitionState != (int) States.transitioningToNight && view.IsMine)
        {

            transitionState = (int)States.transitioningToNight;

            day = false;

            movement.frozen = true;

            transition.SetTrigger("Fade");

            // player can't leave bed while transitioning to night
            interact.DisableButton();

        }

    }

    // Method SwitchToNight() - called every frame whiletransitionState == (int) States.transitioningToNight
    // assigns players to beds once the screen is black
    private void TransitionToNight()
    {
        if (transition.GetCurrentAnimatorStateInfo(0).normalizedTime > 1
            && transition.GetCurrentAnimatorStateInfo(0).IsName("Fade") && view.IsMine)
        {

            // if players are not sleeping put them in the remaining beds
            FindBed();

        }
    }

    // Method SwitchToNight() - called by OnEvent when master client detects that all players
    // have been assigned to beds (only after screen has faded to black)
    private void EndTransitionToNight()
    {
        if (view.IsMine)
        {

            transition.SetTrigger("Reappear");
            transitionState = (int)States.none;
            movement.frozen = false;
            role.StartNight(this);

            // only player that owns this client will not be sleeping, for everyone else sleeping = true
            sleeping = false;

            // goes through all players and freezes them
            foreach (Controller player in (Controller[])FindObjectsOfType(typeof(Controller)))
            {

                // for this Controller, disable PhotonView. This will prevent this player from sending position updates
                if (player == this)
                    this.gameObject.GetComponent<PhotonView>().enabled = false;

                // all other clients will also disable PhotonView, but set velocity to 0 to ensure that other players do not
                // continue to slide.
                else
                    player.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

            }

        }

    }

    // Method FindBed() - if this player is not sleeping find a open bed for them based off of which beds
    // are open and what other players are still awake
    private void FindBed()
    {

        // if players are not sleeping put them in the remaining beds
        if (!sleeping)
        {

            // get a list of all available beds and sort them according to position
            List<BedScript> beds = new List<BedScript>();
            foreach (BedScript bed in (BedScript[])FindObjectsOfType(typeof(BedScript)))
            {
                if (bed.player == "" || bed.player == null)
                    beds.Add(bed);

            }
            // sorts by position so all Controllers have an IDENTICAL list of beds
            beds = SortBeds(beds);

            // get a list of all awake players and sort them according to PhotonNetork ID
            List<Controller> players = new List<Controller>();
            foreach (Controller player in (Controller[])FindObjectsOfType(typeof(Controller)))
            {
                if (!player.GetSleeping())
                    players.Add(player);

            }
            // sorts by network ID so all Controllers have an IDENTICAL list of players
            players = SortPlayers(players);

            // check if there are too many players and not enough beds (should never be the case)
            if (players.IndexOf(this) >= beds.Count)
            {
                Debug.Log("ERROR: Not enought beds");
                PhotonNetwork.LeaveRoom();

            }
            // players are assigned to beds based off their corresponding list indexes (the player
            // with the lowest ID goes in the left-most bed, the second player goes in the 2nd left-most, ect)
            else
                beds[players.IndexOf(this)].gameObject.GetComponent<BedScript>().EnterBed(this);

        }

    } 

    // Method SortPlayers() - insertion sorts a list of Controllers (Players) in order of ViewID
    private List<Controller> SortPlayers(List<Controller> players)
    {

        List<int> list = new List<int>();

        // create a list of ViewIDs
        foreach (Controller player in players)
        {
            list.Add(player.view.ViewID);
        }

        // sort the list of ViewIDs from least to greatest
        int i; int j;
        for (i = 1; i < list.Count; i++)
        {

            j = i;

            while (j >= 1 && list[j - 1] >= list[i])
            {
                j--;
            }

            // whenever the index of a ViewID is shifted, shift its corosponding Controller/Player
            list.Insert(j, list[i]);
            list.RemoveAt(i + 1);
            players.Insert(j, players[i]);
            players.RemoveAt(i + 1);

        }

        return players;

    }

    // Method SortBeds() - insertion sorts a list of beds (BedScript) in order of transform.position
    private List<BedScript> SortBeds(List<BedScript> beds)
    {

        List<float> list = new List<float>();

        // create a list of position.x
        foreach (BedScript bed in beds)
        {
            list.Add(bed.gameObject.transform.position.x);
        }

        // sort the list of positions from least to greatest
        int i; int j;
        for (i = 1; i < list.Count; i++)
        {

            j = i;

            while (j >= 1 && list[j - 1] >= list[i])
            {
                j--;
            }

            // whenever the index of a position is shifted, shift its corosponding bed
            list.Insert(j, list[i]);
            list.RemoveAt(i + 1);
            beds.Insert(j, beds[i]);
            beds.RemoveAt(i + 1);

        }

        return beds;

    }

    public bool GetSleeping()
    {
        return this.sleeping;
    }
    public void SetSleeping(bool sleeping)
    {
        this.sleeping = sleeping;
    }

    public bool GetDay()
    {
        return this.day;
    }
    public void SetDay(bool day)
    {
        this.day = day;
    }

    public bool GetInfected()
    {
        return infected;
    }
    public void SetInfected(bool infected)
    {

        this.infected = infected;
        object[] objectArray = { infected };
        view.RPC("UpdateInfectedRPC", RpcTarget.All, objectArray as object);

    }

    // Method GetCurrentRoom() - returns the RoomScript of the room the player is standing in
    public RoomScript GetCurrentRoom()
    {
        RaycastHit2D currentRoom = Physics2D.Raycast(transform.position, Vector2.up, 0.1f, (int) Layers.room);
        if (currentRoom.collider != null)
            return currentRoom.collider.gameObject.GetComponent<RoomScript>();
        return null;
    }

    // Method UpdateInfectedRPC() - syncs a character's infected status across all clients
    [PunRPC]
    public void UpdateInfectedRPC(object[] objectArray)
    {

        infected = (bool) objectArray[0];

    }

    // Method JoinBedRPC() - called by BedScript when player is successfully registered 
    // as that bed's sleeper. Prevents the this player from moving.
    [PunRPC]
    public void JoinBedRPC(object[] objectArray)
    {

        // runs on all instances of this player across all clients

        SetSleeping(true);

        if (view.IsMine)
            movement.frozen = true;

        // move player to the center of bed
        transform.position = new Vector2((float)objectArray[0], (float)objectArray[1]);

    }

    // Method UpdateRoleRPC() - Called by GameLauncher at the beginning of the game in order
    // to assign this character (and its duplicates on other clients) a role
    [PunRPC]
    public void UpdateRoleRPC(int role)
    {

        switch (role)
        {
            // ALIEN
            case 0:
                this.role = new Alien();

                // enable the infected inicator for all players if this character is the alien
                if (view.IsMine)
                {
                    foreach (PlayerScript player in (PlayerScript[])FindObjectsOfType(typeof(PlayerScript)))
                    {
                        player.Objects.indicator.enabled = true;

                    }
                }
                else
                {
                    foreach (PlayerScript player in (PlayerScript[])FindObjectsOfType(typeof(PlayerScript)))
                    {
                        player.Objects.indicator.enabled = false;

                    }
                }

                break;

            // DOCTOR
            case 1:
                this.role = new Doctor();
                break;
        }

    }
    // Method OnEvent() - called on all clients
    public void OnEvent(EventData photonEvent)
    {

        switch (photonEvent.Code)
        {
            case 2:
                StartTransitionToNight();
                break;
            case 3:
                object[] data = (object[])photonEvent.CustomData;
                timer.SetTimer(Convert.ToSingle(data[0]), Convert.ToSingle(data[1]));
                break;
            case 4:
                if (view.IsMine)
                {
                    object[] data2 = (object[])photonEvent.CustomData;
                    PlayerScript[] allPlayers = (PlayerScript[])FindObjectsOfType(typeof(PlayerScript));
                    showingResults = true;
                    foreach (PlayerScript p in allPlayers)
                    {

                        if (p.gameObject.GetComponent<PhotonView>().Owner.UserId.Equals(data2[0].ToString()))
                            role.endNight(this, p);
                    }
                    if (this.gameObject.GetComponent<PhotonView>().Owner.UserId.Equals(data2[0].ToString()))
                        infected = true;
                }
                break;
            case 5:
                Debug.Log(PhotonNetwork.NickName + " has recieved Event (code:5) to transition to day");
                if (!transitioningToSleep && view.IsMine)
                {
                    Debug.Log(PhotonNetwork.NickName + " begining transition to day");
                    sleeping = false;

                    day = true;

                    Debug.Log(view.name + " is now transitioning to day");
                    transitioningToDay = true;
                    Objects.transitioner.SetTrigger("Fade");
                    showingResults = false;
                    if (PhotonNetwork.IsMasterClient)
                    {
                        ((HealingMachineScript[])FindObjectsOfType(typeof(HealingMachineScript)))[0].available = true;
                        ((HealingMachineScript[])FindObjectsOfType(typeof(HealingMachineScript)))[0].updateOtherClients();
                    }
                }
                break;
            case 6:
                EndTransitionToNight();
                break;

        }

    }

}
