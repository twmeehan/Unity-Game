using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Class - attached to Handler emtpy object; runs when Menu.unity scene is opened;
/// methods called when buttons are pressed to navigate UI
/// </summary>
public class MenuHandler : MonoBehaviourPunCallbacks
{

    #region Private Serializable Fields

    // Used to open and close various menus
    [SerializeField] private GameObject CreateRoomCanvas;
    [SerializeField] private GameObject MainMenuCanvas;
    [SerializeField] private GameObject JoinRoomCanvas;

    // Required to enable/disable based on valid/invalid username
    [SerializeField] private InputField UsernameInput;
    [SerializeField] private Button CreateRoomButton;
    [SerializeField] private Button JoinRoomButton;

    #endregion

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


    }


    /// <summary>
    /// Method - start on main menu and close all other menus
    /// </summary>
    void Start()
    {

        CreateRoomCanvas.SetActive(false);
        MainMenuCanvas.SetActive(true);
        JoinRoomCanvas.SetActive(false);

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

    /// <summary>
    /// Method - open JoinRoomCanvas when "JoinRoomButton" is pressed; saves client's username
    /// </summary>
    public void JoinRoom()
    {

        CreateRoomCanvas.SetActive(false);
        JoinRoomCanvas.SetActive(true);
        MainMenuCanvas.SetActive(false);

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
    /// Method - open CreateRoomCanvas when "CreateRoomButton" is pressed; saves client's username
    /// </summary>
    public void CreateRoom()
    {

        CreateRoomCanvas.SetActive(true);
        MainMenuCanvas.SetActive(false);
        JoinRoomCanvas.SetActive(false);

        PhotonNetwork.NickName = UsernameInput.text;

        // Used to get a list of rooms running
        PhotonNetwork.JoinLobby();

    }
    /// <summary>
    /// Method - from MonoBehaviourPunCallbacks; switches to Game.unity scene when client joins a room
    /// </summary>
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Game");

    }

}