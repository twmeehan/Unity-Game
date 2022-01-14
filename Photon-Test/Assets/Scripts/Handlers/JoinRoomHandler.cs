using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

/// <summary>
/// Class - attached to Handler emtpy object; runs when JoinRoomCanvas is opened;
/// allows user to choose from all public rooms and also join private ones using a code
/// </summary>
public class JoinRoomHandler : MonoBehaviourPunCallbacks
{
    #region Private Serializable Fields

    // Used to open and close various menus
    [SerializeField] private GameObject CreateRoomCanvas;
    [SerializeField] private GameObject MainMenuCanvas;
    [SerializeField] private GameObject JoinRoomCanvas;
    [SerializeField] private GameObject OptionsCanvas;

    // User entered code to join private rooms
    [SerializeField] private InputField Code;

    // Required to create a list of public rooms
    [SerializeField] private Transform Content;
    [SerializeField] private RoomListing RoomListingPrefab;

    #endregion

    // Contains a list of all rooms currently listed
    private List<RoomListing> listings = new List<RoomListing>();

    /// <summary>
    /// Method - from MonoBehaviourPunCallbacks; automatically called when any room is changed
    /// or is called when a client uses .joinLobby(); used to keep a accurate list of public rooms
    /// in the "content" section of a ScrollView in JoinRoomCanvas; updated in live time
    /// </summary>
    /// <param name="roomList"> contains a list of all rooms that have changed since last
    /// method call </param>
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        
        foreach (RoomInfo info in roomList)
        {

            // checks for any new listings that have old duplicate listings
            /// <example>
            /// A player joins a room, updating all other clients through the .OnRoomListUpdate()
            /// method. Every client adds a new listing containing the updated info (+1 player).
            /// However, each client still has the old listing with the original number of players.
            /// Since the old listing is a duplicate of the new listing about to be added, it is deleted.
            /// </example>
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

            // Creates a new listing for any room that has been updated
            RoomListing listing = Instantiate(RoomListingPrefab, Content);
            if (listing != null)
            {

                listing.SetRoomInfo(info);
                listings.Add(listing);
                
            }

            // Deletes any rooms that no longer exist. 
            /// <example>
            /// The last player in a lobby leaves, updating all clients with the .OnRoomListUpdate()
            /// method. The old listing (with 1 player) is deleted and a new 1 (with 0/0 players) is
            /// added. This new listing is imediately deleted by checking for RoomListings with less than
            /// 1 player.
            /// </example>
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

    /// <summary>
    /// Method - Called by the ArrowButton; takes code from InputField and attempts to join a room with 
    /// that code
    /// </summary>
    public void joinRoom()
    {
        if (Code.text.Length == 4)
        {
            PhotonNetwork.JoinRoom(Code.text);
        }
        
    }

    // Method - Called by the BackButton; deletes all current listings and opens MainMenuCanvas
    public void back()
    {
        foreach (RoomListing room in listings)
        {
            room.delete();
        }
        CreateRoomCanvas.SetActive(false);
        JoinRoomCanvas.SetActive(false);
        MainMenuCanvas.SetActive(true);
        OptionsCanvas.SetActive(false);

    }

}
