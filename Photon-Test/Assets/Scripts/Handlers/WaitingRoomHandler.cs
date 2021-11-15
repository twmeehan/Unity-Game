using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaitingRoomHandler : MonoBehaviourPunCallbacks, IOnEventCallback
{

    public GameObject masterOptions;
    byte StartGameCode = 1;
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
    public void startGame()
    {
        Debug.Log("Starting game...");
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(StartGameCode, null, raiseEventOptions, SendOptions.SendReliable);
    }
    public void OnEvent(EventData photonEvent)
    {
        Debug.Log("Switching to Game");

        if (photonEvent.Code == 1)
            SceneManager.LoadScene("Game");
    }
}
