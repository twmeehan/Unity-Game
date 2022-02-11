using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedScript : MonoBehaviour
{

    PhotonView view;

    public GameObject covers;
    public new Transform transform;
    public LayerMask PlayerLayer;

    public string player = "";
    
    // Start is called before the first frame update
    void Start()
    {

        view = this.gameObject.GetComponent<PhotonView>();
        // The covers are disabled so there is no breathing animation playing when the bed is created
        covers.GetComponent<SpriteRenderer>().enabled = false;

        PhotonNetwork.SerializationRate = 20;

    }

    // Called by the player when they want to join this bed
    public void EnterBed(Controller p)
    {

        // The new player joins the bed and BasicCharacter.JoinBed() tells the player to start sleeping animations
        player = p.view.Owner.UserId;
        object[] objectArray = { transform.position.x, transform.position.y };
        p.view.RPC("JoinBedRPC", RpcTarget.All, objectArray as object);

        // The covers are enabled and begin playing a breathing animation
        covers.GetComponent<SpriteRenderer>().enabled = true;
        covers.GetComponent<SpriteRenderer>().sortingOrder = 2;
        updateOtherClients();

    }
    // Called by the player when they want to leave their bed
    public void LeaveBed()
    {

        player = "";

        // The covers are disabled so there is no breathing animation playing
        covers.GetComponent<SpriteRenderer>().enabled = false;

        updateOtherClients();
    }

    public void updateOtherClients()
    {
        view.RPC("updateOtherClientsRPC", RpcTarget.All, player, covers.GetComponent<SpriteRenderer>().enabled);
    }
    [PunRPC]
    public void updateOtherClientsRPC(string newSleeper, bool isBedInUse)
    {
        this.player = newSleeper;
        covers.GetComponent<SpriteRenderer>().enabled = isBedInUse;
        covers.GetComponent<SpriteRenderer>().sortingOrder = 2;

    }
    // Used to get the object that contains the PlayerScript script for the player that is sleeping
    public Controller getPlayer()
    {

        Controller[] players = (Controller[])FindObjectsOfType(typeof(Controller));
        foreach (Controller p in players) {

            if (p.view.Owner.UserId.Equals(player))
                return p;

        }
        Debug.Log("No player");
        return null;

    }

}
