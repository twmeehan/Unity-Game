using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// RoomListingScript

/// <summary>
/// Class - attached to RoomListingPrefab script; code required to set up a room listing; room
/// listings display infomation about a room and allow a player to join it by clicking
/// on the prefab; listings are listed in the JoinRoomCanvas; required to have the same name as prefab
/// </summary>
public class RoomListing : MonoBehaviour
{
    #region Private Serializable Fields

    // Individual UI components in the prefab that contain info about the room
    [SerializeField] private TextMeshProUGUI RoomName;
    [SerializeField] private TextMeshProUGUI PlayerCount;
    [SerializeField] private TextMeshProUGUI RoomCode;

    // Reference to the prefab
    [SerializeField] public GameObject Self;

    #endregion

    // Code of room being referenced
    private string Code;

    // The room that this listing is tied to
    public RoomInfo RoomInfo { get; private set; }

    /// <summary>
    /// Method - sets info for the listing; changes text display; ect
    /// </summary>
    /// <param name="roomInfo"> the room that the information is set from </param>
    public void SetRoomInfo(RoomInfo roomInfo)
    {

        RoomInfo = roomInfo;
        
        Code = roomInfo.Name;

        RoomName.text = Code.Substring(0, Code.IndexOf('-')) + "'s Room";
        PlayerCount.text = "Players:" + roomInfo.PlayerCount.ToString() + "/" + roomInfo.MaxPlayers.ToString();
        RoomCode.text = "Room Code:" + Code.Substring(Code.IndexOf('-')+1);

    }

    // Method - runs if the listing is clicked on
    public void joinRoom()
    {
        PhotonNetwork.JoinRoom(Code);
    }

    // Method - delete this listing
    public void delete()
    {
        Destroy(Self);
    }
    
}
