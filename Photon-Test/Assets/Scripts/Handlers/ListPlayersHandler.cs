using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListPlayersHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnGUI()
    {
        if (!PhotonNetwork.InRoom)
        {
            return;
        }

        GUILayout.BeginVertical();

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            GUILayout.Label(p.NickName);

            // You can also use this one instead of the above one
            // GUILayout.Label(p.ToStringFull());
        }

        GUILayout.EndVertical();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
