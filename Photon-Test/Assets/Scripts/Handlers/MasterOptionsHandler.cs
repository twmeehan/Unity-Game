using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MasterOptionsHandler : MonoBehaviourPunCallbacks
{
    public Toggle PrivatePublic;
    public Toggle InfinateJump;

    // Update is called once per frame
    void Awake()
    {
        privateOnChange();
        infiniteOnChange();
    }
    public void privateOnChange()
    {
        ExitGames.Client.Photon.Hashtable h = new ExitGames.Client.Photon.Hashtable();
        h = PhotonNetwork.CurrentRoom.CustomProperties;
        h.Remove("p");
        h.Add("p", PrivatePublic.isOn);
        PhotonNetwork.CurrentRoom.SetCustomProperties(h);
    }
    public void infiniteOnChange()
    {
        ExitGames.Client.Photon.Hashtable h = new ExitGames.Client.Photon.Hashtable();
        h = PhotonNetwork.CurrentRoom.CustomProperties;
        h.Remove("t");
        if (InfinateJump.isOn)
            h.Add("t", "Infinite");
        else
            h.Add("t", "Standard");
        PhotonNetwork.CurrentRoom.SetCustomProperties(h);
    }
}

