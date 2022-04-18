using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading;

/// <summary>
/// Class - attached to Handler emtpy object; runs when Menu.unity scene is opened;
/// methods called when buttons are pressed to navigate UI
/// </summary>
public class MenuHandler : MonoBehaviourPunCallbacks
{

    #region Private Serializable Fields

    // Used to open and close various menus
    [SerializeField] private GameObject MainMenuCanvas;
    [SerializeField] private GameObject JoinRoomCanvas;
    [SerializeField] private GameObject OptionsCanvas;


    // Required to enable/disable based on valid/invalid username
    [SerializeField] private InputField UsernameInput;
    [SerializeField] private Button CreateRoomButton;
    [SerializeField] private Button JoinRoomButton;

    #endregion

    private float timeSinceStart;
    #region MonoBehaviour CallBacks

    public override void OnConnectedToMaster()
    {
        Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
    }

    #endregion

    /// <summary>
    /// Method - MonoBehaviour method called on GameObject by Unity during early initialization phase.
    /// </summary>
    void Awake()
    {

        // #Critical
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;

        if (!PlayerPrefs.HasKey("name"))
        {
            PlayerPrefs.SetString("name", "");
        }
        UsernameInput.text = PlayerPrefs.GetString("name");



    }


    /// <summary>
    /// Method - start on main menu and close all other menus
    /// </summary>
    void Start()
    {

        MainMenuCanvas.SetActive(true);
        JoinRoomCanvas.SetActive(false);
        OptionsCanvas.SetActive(false);

    }

    #region Public Methods


    /// <summary>
    /// Start the connection process.
    /// - If already connected, we attempt joining a random room
    /// - if not yet connected, Connect this application instance to Photon Cloud Network
    /// </summary>
    public void Connect()
    {
        // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
        if (PhotonNetwork.IsConnected)
        {
            // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            // #Critical, we must first and foremost connect to Photon Online Server.
            PhotonNetwork.ConnectUsingSettings();
        }
    }


    #endregion

    /// <summary>
    /// Method - checks if username input has more than 3 characters; if so enable play buttons
    /// </summary>
    public void ValidateName()
    {

        if (UsernameInput.text.Length >= 3)
        {

            CreateRoomButton.interactable = true;
            JoinRoomButton.interactable = true;

        }
        else
        {

            CreateRoomButton.interactable = false;
            JoinRoomButton.interactable = false;

        }

    }
    public void Options()
    {

        JoinRoomCanvas.SetActive(false);
        MainMenuCanvas.SetActive(false);
        OptionsCanvas.SetActive(true);

    }

    /// <summary>
    /// Method - open JoinRoomCanvas when "JoinRoomButton" is pressed; saves client's username
    /// </summary>
    public void JoinRoom()
    {

        JoinRoomCanvas.SetActive(true);
        MainMenuCanvas.SetActive(false);
        OptionsCanvas.SetActive(false);
        PlayerPrefs.SetString("name", UsernameInput.text);


        PhotonNetwork.NickName = UsernameInput.text;

        // #Critical
        // Joining the lobby is considered a update to all rooms which calls .OnRoomListUpdate() for all rooms
        PhotonNetwork.JoinLobby();

    }

    public void Exit()
    {

        Application.Quit();

    }
    /// <summary>
    /// Method - runs when the CreateRoomButton is pressed
    /// </summary>
    public void CreateRoom()
    {
        PlayerPrefs.SetString("name", UsernameInput.text);

        PhotonNetwork.NickName = UsernameInput.text;

        RoomOptions roomOptions = new RoomOptions();

        roomOptions.MaxPlayers = 10;

        string code = "0000";
        ExitGames.Client.Photon.Hashtable h = new ExitGames.Client.Photon.Hashtable();
        h.Add("n", PhotonNetwork.NickName); // name
        h.Add("p", true); // private / public
        h.Add("r", false); // running
        h.Add("d", true); // day

        h.Add("t", "Standard"); // type
        roomOptions.CustomRoomProperties = h;
        roomOptions.PublishUserId = true;

        roomOptions.CustomRoomPropertiesForLobby = new string[] { "n", "p", "r", "t" };

        // Generates a random code
        code = Random.Range(1111, 9999).ToString();
        Debug.Log(code);

        MainMenuCanvas.SetActive(false);
        JoinRoomCanvas.SetActive(false);

        if (PhotonNetwork.JoinOrCreateRoom(code, roomOptions, TypedLobby.Default))
        {
            Debug.Log("created room");
        } else
        {
            Debug.Log("failed to create room");
        }

    }
    /// <summary>
    /// Method - from MonoBehaviourPunCallbacks; switches to Game.unity scene when client joins a room
    /// </summary>
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Waiting");

    }

}