using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private TextMeshProUGUI GameType;
    [SerializeField] private TextMeshProUGUI Status;


    // Reference to the prefab
    [SerializeField] public GameObject Self;

    // Reference to button component
    [SerializeField] public Button button;


    #endregion

    private string RoomCode;
    // The room that this listing is tied to
    public RoomInfo RoomInfo { get; private set; }

    /// <summary>
    /// Method - sets info for the listing; changes text display; ect
    /// </summary>
    /// <param name="roomInfo"> the room that the information is set from </param>
    public void SetRoomInfo(RoomInfo roomInfo)
    {

        RoomInfo = roomInfo;

        RoomCode = RoomInfo.Name;

        try
        {
            roomInfo.CustomProperties.TryGetValue("n", out object val);
            RoomName.text = val.ToString() + "'s Room";

            PlayerCount.text = "Players:" + roomInfo.PlayerCount.ToString() + "/" + roomInfo.MaxPlayers.ToString();

            roomInfo.CustomProperties.TryGetValue("t", out val);
            GameType.text = "Game Type:" + val.ToString();

            roomInfo.CustomProperties.TryGetValue("r", out val);
            if (val.Equals(true))
            {

                Status.text = "In Progress";
                button.interactable = false;
                return;

            }

            roomInfo.CustomProperties.TryGetValue("p", out val);
            if (val.Equals(true))
            {

                Status.text = "Private";
                button.interactable = false;

            }
            else
            {

                Status.text = "Public";
                button.interactable = true;

            }
        } catch
        {

        }

    }

    // Method - runs if the listing is clicked on
    public void joinRoom()
    {
        PhotonNetwork.JoinRoom(RoomCode);
    }

    // Method - delete this listing
    public void delete()
    {
        Destroy(Self);
    }
    
}
