using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class - attached to Handler emtpy object; runs when CreateRoomCanvas is opened;
/// allows user to create rooms with private/public and max players options
/// </summary>
public class CreateRoomHandler : MonoBehaviourPunCallbacks
{

    #region Private Serializable Fields

    [SerializeField] private GameObject CreateRoomCanvas;
    [SerializeField] private GameObject MainMenuCanvas;
    [SerializeField] private GameObject JoinRoomCanvas;

    // UI elements of the CreateRoomCanvas
    [SerializeField] private Button CheckboxUnchecked;
    [SerializeField] private Button CheckboxChecked;
    [SerializeField] private Slider Slider;

    // The number above the slider that notes how many max players are currently selected
    [SerializeField] private Text MaxPlayers;

    #endregion

    private List<RoomInfo> _roomList;

    /// <summary>
    /// Method - runs when the CreateRoomButton is pressed; uses settings that user input
    /// </summary>
    public void createRoom()
    {

        RoomOptions roomOptions = new RoomOptions();

        roomOptions.MaxPlayers = (byte) Mathf.Ceil(Slider.value);

        // If the checkbox is checked
        if (CheckboxChecked.interactable)
        {
            roomOptions.IsVisible = false;
        } else
        {
            roomOptions.IsVisible = true;
        }


        string code = "0000";
        bool roomFound = false;
        ExitGames.Client.Photon.Hashtable h = new ExitGames.Client.Photon.Hashtable();
        h.Add("n", "noTim");
        roomOptions.CustomRoomProperties = h;

        string[] str = new string[1];
        str[0] = "n";
        roomOptions.CustomRoomPropertiesForLobby = str;

        // Generates a random code
        if (roomOptions.IsVisible)
        {
            code = PhotonNetwork.NickName + "-" + Mathf.Floor(Random.Range(0.01f, 9.99f)).ToString() +
                Mathf.Floor(Random.Range(0.01f, 9.99f)).ToString() +
                Mathf.Floor(Random.Range(0.01f, 9.99f)).ToString() +
                Mathf.Floor(Random.Range(0.01f, 9.99f)).ToString();
        } else
        {
            code = Mathf.Floor(Random.Range(0.01f, 9.99f)).ToString() +
                Mathf.Floor(Random.Range(0.01f, 9.99f)).ToString() +
                Mathf.Floor(Random.Range(0.01f, 9.99f)).ToString() +
                Mathf.Floor(Random.Range(0.01f, 9.99f)).ToString();
        }
        PhotonNetwork.CreateRoom(code, roomOptions, TypedLobby.Default);
        Debug.Log("Creating Room " + code);

    }
    // Method - runs when the unchecked box is clicked; switches to the checked box
    public void Check()
    {

        CheckboxChecked.interactable = true;
        CheckboxUnchecked.interactable = false;

        // Moves invisible box out of the way
        CheckboxChecked.transform.localPosition = new Vector3(-61, 40, 0);
        CheckboxUnchecked.transform.localPosition = new Vector3(-400,40,0);
    }
    // Method - runs when the checked box is clicked; switches to the unchecked box
    public void Uncheck()
    {

        CheckboxChecked.interactable = false;
        CheckboxUnchecked.interactable = true;
        CheckboxChecked.transform.localPosition = new Vector3(-400, 40, 0);
        CheckboxUnchecked.transform.localPosition = new Vector3(-59.75f, 40.25f, 0);

    }
    // Method - every time the slider is moved the number above the slider is updated
    public void UpdateMaxPlayers()
    {
        MaxPlayers.text = Slider.value.ToString();
    }

    // Method - open up the MainMenuCanvas
    public void back()
    {
        CreateRoomCanvas.SetActive(false);
        JoinRoomCanvas.SetActive(false);
        MainMenuCanvas.SetActive(true);
    }
    // Method - gets a list of running rooms when the client wants to open a room
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        _roomList = roomList;
    }
}
