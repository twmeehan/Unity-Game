using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingRoomHandler : MonoBehaviourPunCallbacks
{

    public GameObject masterOptions;
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            masterOptions.SetActive(true);
        }
    }

    public void leave()
    {
        Debug.Log("leaving room...");
        PhotonNetwork.LeaveRoom();
        //PhotonNetwork.JoinLobby();
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);
        if (PhotonNetwork.IsMasterClient)
        {
            masterOptions.SetActive(true);
        }
    }
    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        Debug.Log("Joined Lobby");
        PhotonNetwork.LoadLevel("Menu");
    }

}
