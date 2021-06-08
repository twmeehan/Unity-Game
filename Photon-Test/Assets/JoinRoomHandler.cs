using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinRoomHandler : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Transform content;
    [SerializeField]
    private RoomListing RoomPrefab;

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(RoomInfo info in roomList)
        {
            Debug.Log("Room Found");
            RoomListing listing = Instantiate(RoomPrefab, content);
            if (listing != null)
            {
                listing.SetRoomInfo(info);
            }
            
        }
    }
}
