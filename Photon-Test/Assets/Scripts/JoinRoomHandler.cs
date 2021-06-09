using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

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

    private List<RoomListing> listings = new List<RoomListing>();
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        
        foreach (RoomInfo info in roomList)
        {
            int index = listings.FindIndex(x => x.RoomInfo.Name == info.Name);
            if (index != -1)
            {
                try {
                    Destroy(listings[index].gameObject);
                } catch
                {
                    Debug.Log("Attempt to Destroy Non-existant Object");
                }
                listings.RemoveAt(index);
            }
           
            RoomListing listing = Instantiate(RoomPrefab, content);
            if (listing != null)
            {
                listing.SetRoomInfo(info);
                listings.Add(listing);
                //PhotonNetwork.JoinLobby();
            }
            int index2 = listings.FindIndex(x => x.RoomInfo.MaxPlayers < 1);
            if (index2 != -1)
            {
                try
                {
                    Destroy(listings[index2].gameObject);
                }
                catch
                {
                    Debug.Log("Attempt to Destroy Non-existant Object");
                }
                listings.RemoveAt(index2);
            }
        }
    }
    
    public void joinRoom()
    {
        if (code.text.Length == 4)
        {
            PhotonNetwork.JoinRoom(code.text);
        }
        
    }
    public void back()
    {
        foreach (RoomListing room in listings)
        {
            room.delete();
        }
        CreateRoomPanel.SetActive(false);
        JoinRoomPanel.SetActive(false);
        MainMenuPanel.SetActive(true);
        
    }

}
