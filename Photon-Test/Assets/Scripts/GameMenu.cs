using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameMenu : MonoBehaviourPunCallbacks
{

    #region Private Serializable Fields

    [SerializeField] private GameObject RoomPanel;
    [SerializeField] private GameObject MainMenuPanel;


    [SerializeField] private InputField UsernameInput;
    [SerializeField] private Button CreateRoomInput;
    [SerializeField] private Button JoinRoomInput;

    [SerializeField] private InputField RoomCode;

    #endregion


    #region Private Fields


    /// <summary>
    /// This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
    /// </summary>
    string gameVersion = "1";


    #endregion

    private string JoinOrCreate = "Join";

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
    /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
    /// </summary>
    void Awake()
    {
        // #Critical
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;


    }


    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during initialization phase.
    /// </summary>
    void Start()
    {

        RoomPanel.SetActive(false);

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
            PhotonNetwork.GameVersion = gameVersion;
        }
    }


    #endregion

    /// <summary>
    /// Checks if username input has more than 3 characters. If so allow user to play
    /// </summary>
    public void ValidateName()
    {
        if (UsernameInput.text.Length >= 3)
        {

            CreateRoomInput.interactable = true;
            JoinRoomInput.interactable = true;
        }
        else
        {
            CreateRoomInput.interactable = false;
            JoinRoomInput.interactable = false;
        }
    }

    /// <summary>
    /// Prompt User for a room code.
    /// </summary>
    public void JoinRoom()
    {
        RoomPanel.SetActive(true);
        MainMenuPanel.SetActive(false);
        JoinOrCreate = "Join";
        PhotonNetwork.NickName = UsernameInput.text;

    }
    /// <summary>
    /// Prompt User for a room code.
    /// </summary>
    public void CreateRoom()
    {
        RoomPanel.SetActive(true);
        MainMenuPanel.SetActive(false);
        JoinOrCreate = "Create";
        PhotonNetwork.NickName = UsernameInput.text;

    }
    public void EnterCode()
    {
        if (RoomCode.text.Length == 4)
        {
            if (JoinOrCreate == "Join")
            {
                RoomOptions roomOptions = new RoomOptions();
                roomOptions.MaxPlayers = 5;
                PhotonNetwork.JoinOrCreateRoom(RoomCode.text, roomOptions, TypedLobby.Default);
                Debug.Log("Joining Room " + RoomCode.text.ToString());
            }
            else
            {
                PhotonNetwork.CreateRoom(RoomCode.text, new RoomOptions() { MaxPlayers = 5 }, null);

                Debug.Log("Created Room " + RoomCode.text.ToString());
            }
        }
    }
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Game");

    }

    public override void OnConnected()
    {
        Debug.Log("Connected");
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        foreach (RoomInfo info in roomList)
        {
            Debug.Log(info.Name);
        }
    }


}