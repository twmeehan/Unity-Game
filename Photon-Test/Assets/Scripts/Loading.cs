using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("1");
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        Debug.Log("2");

    }

    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene("MainMenu");
        Debug.Log("3");

    }
}
