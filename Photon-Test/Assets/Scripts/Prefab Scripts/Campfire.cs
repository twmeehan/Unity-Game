using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// Class Campfire - attached to all campfire prefabs, allows a # of players
// to sleep nearby and toggles fire on/off. Some night actions may use 
// campfire.cs to get information on all players sleeping at that campfire
public class Campfire : MonoBehaviour
{

    private PhotonView view;

    public bool isLit = true;

    // number of players that can sleep at this campfire
    public int size;
    public GameObject fire;
    
    private List<string> playerIDs;
    private List<Controller> players;

    // Start is called before the first frame update
    void Start()
    {
        view = this.gameObject.GetComponent<PhotonView>();
        playerIDs = new List<string>();
        players = new List<Controller>();
        UpdatePlayerList();

    }

    // Method UpdatePlayerList() - takes the list of PlayerIDs and finds the corresponding
    // player for each ID and updates the players array with these corresponding controllers
    private List<Controller> UpdatePlayerList()
    {

        // empty array
        players.Clear();

        // get a list of all players in the scene
        Controller[] allControllersInScene = (Controller[])FindObjectsOfType(typeof(Controller));

        // check if controller's ID is in playerIDs if so add that controller to 'players'
        foreach (Controller controller in allControllersInScene)
        {

            if (playerIDs.Contains(controller.view.Owner.UserId))
            {
                players.Add(controller);
            }

        }

        // return the newly updated list of players(controllers) based off playerIDs
        return players;

    }

    // Method OnPlayerLeave() - if the player that left was sleeping by this fire, then find
    // and remove that player from playerIDs and update 'players' list for all clients
    public void OnPlayerLeave()
    {

        if (PhotonNetwork.IsMasterClient)
        {
            // get a list of all players in the scene
            Controller[] allControllersInScene = (Controller[])FindObjectsOfType(typeof(Controller));

            // check each ID in playerIDs
            foreach (string ID in playerIDs)
            {
                foreach (Controller controller in allControllersInScene)
                {

                    // if it matchs any controller (player) in the scene check the next ID
                    if (controller.view.Owner.UserId == ID)
                    {
                        break;
                    }

                    // if not remove this ID from playerIDs and sync this change across all clients
                    playerIDs.Remove(ID);
                    view.RPC("UpdatePlayerListAcrossClientsRPC", RpcTarget.All, playerIDs);
                    break;

                }
            }
        }
    }
    // Method AttemptToJoinBed() - called by a player when they try to sleep
    // near this fire. Returns true if the player successfully joins the bed
    public bool AttemptToJoinCampfire(Controller controller)
    {
        if (playerIDs.Count < size && isLit)
        {
            playerIDs.Add(controller.view.Owner.UserId);
            view.RPC("UpdatePlayerListAcrossClientsRPC", RpcTarget.All, playerIDs.ToArray());
            return true;
        }
        return false;
    }

    // Method LeaveBed() - called by a player when they try to wake up
    // near this fire
    public void LeaveCampfire(Controller controller)
    {

        playerIDs.Remove(controller.view.Owner.UserId);

        // sync new list of players across all clients
        view.RPC("UpdatePlayerListAcrossClientsRPC", RpcTarget.All, playerIDs.ToArray());

    }
    // Method SetPlayerList() - sets playerIDs to whatever masterclient wants across all clients
    public void SetPlayerList(List<string> playerIDs)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            this.playerIDs = playerIDs;
            view.RPC("UpdatePlayerListAcrossClientsRPC", RpcTarget.All, playerIDs.ToArray());
        }
    }
    // Method LightFire() - starts fire particles and allows players to sleep across all clients
    public void LightFire()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            isLit = true;
            fire.SetActive(true);
            view.RPC("SetLit", RpcTarget.All, true);
        }

    }

    // Method ExtingishFire() - stops fire particles and prevents players from sleeping across all clients
    public void ExtingishFire()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            isLit = false;
            fire.SetActive(false);
            view.RPC("SetLit", RpcTarget.All, false);
        }
    }

    [PunRPC]
    // Method SetLit() - sets lit/unlit to whatever masterclient sets
    public void SetLit(bool lit)
    {
        isLit = lit;
        fire.SetActive(lit);
    }
    // Method UpdatePlayerListAcrossClientsRPC() - takes an array of players and syncs
    // it across all clients
    [PunRPC]
    public void UpdatePlayerListAcrossClientsRPC(string[] playerIDs)
    {

        this.playerIDs = playerIDs.ToList();

        // make sure 'players' is synced as well
        UpdatePlayerList();

    }
    public List<Controller> GetPlayers()
    {
        return players;
    }
}
