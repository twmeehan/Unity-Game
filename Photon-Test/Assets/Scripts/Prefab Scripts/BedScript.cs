using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedScript : MonoBehaviour, IPunObservable
{

    public GameObject covers;
    public Transform transform;

    private GameObject player;
    
    // Start is called before the first frame update
    void Start()
    {

        // The covers are disabled so there is no breathing animation playing when the bed is created
        covers.GetComponent<SpriteRenderer>().enabled = false;

    }

    // Called by the player when they want to join this bed
    public void  EnterBed(GameObject p)
    {

        // If there is a player in this bed, yeet them
        if (this.player != null)
            player.GetComponent<BasicCharacter>().KickFromBed();

        // The new player joins the bed and BasicCharacter.JoinBed() tells the player to start sleeping animations
        player = p;
        p.GetComponent<BasicCharacter>().JoinBed(this, transform);

        // The covers are enabled and begin playing a breathing animation
        covers.GetComponent<SpriteRenderer>().enabled = true;

    }
    // Called by the player when they want to leave their bed
    public void LeaveBed()
    {

        // If there is a player in this bed, yeet them
        if (this.player != null)
            player.GetComponent<BasicCharacter>().KickFromBed();
        player = null;

        // The covers are disabled so there is no breathing animation playing
        covers.GetComponent<SpriteRenderer>().enabled = false;

    }

    // Used to get the object that contains the BasicCharacter script for the player that is sleeping
    public GameObject getPlayer()
    {
        return player;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

        if (stream.IsWriting)
        {
            stream.SendNext(player);
            stream.SendNext(covers);
        }
        else
        {
            player = (GameObject) stream.ReceiveNext();
            covers = (GameObject)stream.ReceiveNext();
        }
    }
}
