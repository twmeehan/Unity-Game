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
    private string code;

    public void SetRoomInfo(RoomInfo roomInfo)
    {
        // WIP
        code = roomInfo.Name;
        RoomName.text = "Username's Room";
        PlayerCount.text = "Players:" + roomInfo.PlayerCount.ToString() + "/" + roomInfo.MaxPlayers.ToString();
        RoomCode.text = "Room Code:" + roomInfo.Name.ToString();
    }
    public void joinRoom()
    {
        PhotonNetwork.JoinRoom(code);
    }
}
