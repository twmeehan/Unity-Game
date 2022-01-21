using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingMachineScript : MonoBehaviour
{

    PhotonView view;
    private bool available = true;

    void Start()
    {
        view = this.gameObject.GetComponent<PhotonView>();
        PhotonNetwork.SerializationRate = 20;
    }

    public void updateOtherClients()
    {
        view.RPC("updateOtherClientsRPC", RpcTarget.All);
    }

    [PunRPC]
    public void updateOtherClientsRPC()
    {
        this.available = false;
    }

    public void enterHealingMachine(PlayerScript player)
    {
        if (available == true) 
        {
            player.healPlayer();
            available = false;
        }

        Debug.Log("Entered Machine");
    }
    
}

