using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class text : MonoBehaviourPunCallbacks
{

    private void Update()
    {

    }
    void Start()
    {

        PhotonNetwork.ConnectUsingSettings();
    }

    // Method - runs after connected to server and allows client to access rooms
    public override void OnConnectedToMaster()
    {

        PhotonNetwork.JoinLobby();

    }

    // Method - after setup, client switches to menu scene
    public override void OnJoinedLobby()
    {

    }
}
