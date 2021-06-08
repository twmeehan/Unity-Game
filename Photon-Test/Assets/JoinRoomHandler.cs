using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoinRoomHandler : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject CreateRoomPanel;
    [SerializeField] private GameObject MainMenuPanel;
    [SerializeField] private GameObject JoinRoomPanel;

    [SerializeField]
    private InputField code;
    [SerializeField]
    private Transform content;
    [SerializeField]
    private RoomListing RoomPrefab;

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(RoomInfo info in roomList)
        {
            Debug.Log("Room Found");
            RoomListing listing = Instantiate(RoomPrefab, content);
            if (listing != null)
            {
                listing.SetRoomInfo(info);
            }
            
        }
    }

    public void joinRoom()
    {
        if (code.text.Length == 4)
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 5;
            roomOptions.IsVisible = true;
            PhotonNetwork.JoinOrCreateRoom(code.text, roomOptions, TypedLobby.Default);
        }
        
    }
    public void back()
    {
        CreateRoomPanel.SetActive(false);
        JoinRoomPanel.SetActive(false);
        MainMenuPanel.SetActive(true);
    }

}
