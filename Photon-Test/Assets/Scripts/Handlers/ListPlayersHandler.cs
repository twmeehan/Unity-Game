using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListPlayersHandler : MonoBehaviourPunCallbacks
{

    // Required to create a list of public rooms
    [SerializeField] public Transform Content;
    [SerializeField] public GameObject NameEntryPrefab;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            NameEntryScript entry = Instantiate(NameEntryPrefab, Content).GetComponent<NameEntryScript>();
            entry.SetEntryInfo(p);
            
        }

    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);
        for (int i=0; i < Content.childCount; i++)
        {
            Content.GetChild(i).GetComponent<NameEntryScript>().OnMasterChange();
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        NameEntryScript entry = Instantiate(NameEntryPrefab, Content).GetComponent<NameEntryScript>();
        entry.SetEntryInfo(newPlayer);

    }
    public override void OnPlayerLeftRoom(Player newPlayer)
    {
        base.OnPlayerLeftRoom(newPlayer);
        for (int i = 0; i < Content.childCount; i++)
        {
            if (Content.GetChild(i).GetComponent<NameEntryScript>().name.text == newPlayer.NickName)
            {
                Destroy(Content.GetChild(i).gameObject);
            }
        }

    }

}
