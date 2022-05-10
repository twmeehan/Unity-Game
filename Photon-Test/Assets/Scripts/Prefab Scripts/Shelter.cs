using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// Class Campfire - attached to all campfire prefabs, allows a # of players
// to sleep nearby and toggles fire on/off. Some night actions may use 
// campfire.cs to get information on all players sleeping at that campfire
public class Shelter : MonoBehaviour
{

    private PhotonView view;

    // number of players that can sleep at this campfire
    public int size;

    public SpriteRenderer shelterOuterWall;

    private Transform player;
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

    private void Update()
    {
        if (player == null)
        {
            List<Controller> players = ((Controller[])FindObjectsOfType(typeof(Controller))).ToList<Controller>();
            foreach (Controller p in players)
            {
                if (p.view.Owner.IsLocal)
                {
                    player = p.transform;
                }
            }
        } else {
            Color color = shelterOuterWall.color;
            color.a = Mathf.Clamp(Mathf.Abs(player.position.x - this.transform.position.x) * 0.25f,0.5f,1);
            if (player.position.y - this.transform.position.y > 5)
                color.a = 1;
            shelterOuterWall.color = color;
            Debug.Log(shelterOuterWall.color.a + " should be " + color.a);

        }
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
    // Method AttemptToJoinCampfire() - called by a player when they try to sleep
    // near this shalder. Returns true if the player successfully joins the bed
    public bool AttemptToJoinCampfire(Controller controller)
    {
        if (playerIDs.Count < size)
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
    // Method UpdatePlayerListAcrossClientsRPC() - takes an array of players and syncs
    // it across all clients
    [PunRPC]
    public void UpdatePlayerListAcrossClientsRPC(string[] playerIDs)
    {
        
        this.playerIDs = playerIDs.ToList();

        // make sure 'players' is synced as well
        UpdatePlayerList();

    }
    [PunRPC]
    public void UpdatePlayerListAcrossClientsRPC(string playerID)
    {
        this.playerIDs.Clear();
        this.playerIDs.Add(playerID);

        // make sure 'players' is synced as well
        UpdatePlayerList();

    }
    [PunRPC]
    public void UpdatePlayerListAcrossClientsRPC()
    {

        this.playerIDs.Clear();

        // make sure 'players' is synced as well
        UpdatePlayerList();

    }
    public List<Controller> GetPlayers()
    {
        return players;
    }
}
