using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedScript : MonoBehaviour
{

    PhotonView view;

    public GameObject covers;
    public Transform transform;
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
    public void EnterBed(GameObject p)
    {

        // The new player joins the bed and BasicCharacter.JoinBed() tells the player to start sleeping animations
        player = p.GetComponent<PhotonView>().Owner.UserId;
        p.GetComponent<PlayerScript>().JoinBed(transform);

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
    public PlayerScript getPlayer()
    {

        PlayerScript[] players = (PlayerScript[])FindObjectsOfType(typeof(PlayerScript));
        foreach (PlayerScript p in players) {

            if (p.gameObject.GetComponent<PhotonView>().Owner.UserId.Equals(player))
                return p;

        }
        Debug.Log("No player");
        return null;

    }

}
