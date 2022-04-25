using Photon.Pun;
using UnityEngine;

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
    
    private string[] playerIDs;
    private Controller[] players;

    // Start is called before the first frame update
    void Start()
    {
        view = this.gameObject.GetComponent<PhotonView>();
        playerIDs = new string[size];
        UpdatePlayerList();

    }

    // Method UpdatePlayerList() - takes the list of PlayerIDs and finds the corresponding
    // player for each ID and updates the players array with these corresponding controllers
    private Controller[] UpdatePlayerList()
    {

        // empty array
        players = new Controller[size];

        // find the Controller for each ID in playerIDs (view.UserID)
        foreach (string playerID in playerIDs)
        {

            // get a list of all players in the scene and check if that player's ID matches this playerID
            Controller[] allControllersInScene = (Controller[])FindObjectsOfType(typeof(Controller));
            foreach (Controller controller in allControllersInScene)
            {

                // if the IDs match, then add that player to the array on players
                if (controller.view.Owner.UserId.Equals(playerID))
                    players[players.Length] = controller;

            }
        }

        // return the newly updated list of players(controllers) based off playerIDs
        return players;

    }

    // Method AttemptToJoinBed() - called by a player when they try to sleep
    // near this fire. Returns true if the player successfully joins the bed
    public bool AttemptToJoinBed(Controller controller)
    {
        Debug.Log(playerIDs.Length);
        if (playerIDs.Length < size && isLit)
        {
            playerIDs[playerIDs.Length] = controller.view.Owner.UserId;
            view.RPC("UpdatePlayerListAcrossClientsRPC", RpcTarget.All, playerIDs);
            return true;
        }
        return false;
    }

    // Method LeaveBed() - called by a player when they try to wake up
    // near this fire
    public void LeaveBed(Controller controller)
    {

        // create a duplicate of playerIDs
        string[] IDs = playerIDs;
        playerIDs = new string[size];

        // foreach ID in playerIDs...
        for (int i = 0; i < IDs.Length;  i++)
        {

            // if the ID/player is not the one being removed add it back to playerIDs
            if (!controller.view.Owner.UserId.Equals(playerIDs[i]))
                playerIDs[playerIDs.Length] = IDs[i];

        }

        // sync new list of players across all clients
        view.RPC("UpdatePlayerListAcrossClientsRPC", RpcTarget.All, playerIDs);

    }
    // Method LightFire() - starts fire particles and allows players to sleep
    public void LightFire()
    {
        isLit = true;
        fire.SetActive(true);
    }

    // Method ExtingishFire() - stops fire particles and prevents players from sleeping
    public void ExtingishFire()
    {
        isLit = false;
        fire.SetActive(false);
    }

    // Method UpdatePlayerListAcrossClientsRPC() - takes an array of players and syncs
    // it across all clients
    [PunRPC]
    public void UpdatePlayerListAcrossClientsRPC(string[] playerIDs)
    {

        this.playerIDs = playerIDs;

        // make sure 'players' is synced as well
        UpdatePlayerList();

    }
    public Controller[] GetPlayers()
    {
        return players;
    }
}
