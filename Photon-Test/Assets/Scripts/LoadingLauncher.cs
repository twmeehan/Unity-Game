using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Class - attached to Launcher emtpy object; runs when Loading.unity scene is opened
/// by any client; connects client to the Photon servers
/// </summary>
public class LoadingLauncher : MonoBehaviourPunCallbacks
{
    
    // Method - connects to central server based on optimal ping route
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

        SceneManager.LoadScene("Menu");

    }

}
