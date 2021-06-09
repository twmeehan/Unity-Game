using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomListing : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI RoomName;
    [SerializeField]
    private TextMeshProUGUI PlayerCount;
    [SerializeField]
    private TextMeshProUGUI RoomCode;

    [SerializeField]
    public GameObject self;
    private string code;

    public RoomInfo RoomInfo { get; private set; }

    public void SetRoomInfo(RoomInfo roomInfo)
    {
        // WIP
        RoomInfo = roomInfo;
        code = roomInfo.Name;
        RoomName.text = "Username's Room";
        PlayerCount.text = "Players:" + roomInfo.PlayerCount.ToString() + "/" + roomInfo.MaxPlayers.ToString();
        RoomCode.text = "Room Code:" + roomInfo.Name.ToString();
    }
    public void joinRoom()
    {
        PhotonNetwork.JoinRoom(code);
    }
    public void delete()
    {
        Destroy(self);
    }
    
}
