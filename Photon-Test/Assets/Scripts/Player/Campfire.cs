using Photon.Pun;
using UnityEngine;

// Class Campfire - attached to all campfire prefabs, allows a # of players
// to sleep nearby and toggles fire on/off. Some night actions may use 
// campfire.cs to get information on all players sleeping at that campfire
public class Campfire : MonoBehaviour
{

    PhotonView view;

    // number of players that can sleep at this campfire
    public int size;

    private string[] playerIDs;
    private Controller[] players;

    // Start is called before the first frame update
    void Start()
    {

        playerIDs = new string[size];
        UpdatePlayerList();

    }

    // Update is called once per frame
    void Update()
    {
        
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

    public bool AttemptToJoinBed(Controller controller)
    {
        if (playerIDs.Length < size)
        {
            playerIDs[playerIDs.Length] = controller.view.Owner.UserId;
            return true;
        }
        return false;
    }
    public Controller[] GetPlayers()
    {
        return players;
    }
}
