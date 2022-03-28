﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingMachineScript : MonoBehaviour
{

    PhotonView view;
    public bool available = true;

    void Start()
    {
        view = this.gameObject.GetComponent<PhotonView>();
        PhotonNetwork.SerializationRate = 20;
    }

    public void updateOtherClients()
    {
        view.RPC("updateOtherClientsRPC", RpcTarget.All, available);
    }

    [PunRPC]
    public void updateOtherClientsRPC(bool available)
    {
        this.available = available;
    }

    public void enterHealingMachine(Controller player)
    {
        if (available) 
        {
            object[] objectArray = { false };
            player.view.RPC("UpdateInfectedRPC", RpcTarget.All, objectArray as object);
            available = false;
            updateOtherClients();
        }

        Debug.Log("Healed");
    }
    
}
