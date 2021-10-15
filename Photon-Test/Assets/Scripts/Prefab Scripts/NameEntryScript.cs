using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NameEntryScript : MonoBehaviour
{

    public Player player;
    public TextMeshProUGUI name;
    public Button kick;

    public void SetEntryInfo(Player player)
    {
        this.player = player;
        name.text = player.NickName;
        OnMasterChange();
    }

    // Update is called once per frame
    public void OnMasterChange()
    {

        if (PhotonNetwork.IsMasterClient && name.text != PhotonNetwork.NickName)
        {
            kick.interactable = true;
        } else
        {
            kick.interactable = false;
        }
    }

    public void kickPlayer()
    {
        if (player.IsMasterClient)
        {
            return;
        }
        PhotonNetwork.CloseConnection(player);
    }
}
