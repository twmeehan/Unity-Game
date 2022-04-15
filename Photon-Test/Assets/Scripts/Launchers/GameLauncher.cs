using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Linq;

/// <summary>
/// Class - attached to Launcher emtpy object; runs when Game.unity scene is opened
/// by any client; prepares game to run on local client
/// </summary>
public class GameLauncher : MonoBehaviour
{

    public GameObject CharacterPrefab;
    public AudioSource audio;

    private float timeSinceSleeping;
    private bool allSleepingLastFrame;
    private Master master;

    /// <summary>
    /// Method - runs on init; displays room code on screen; instantiates character prefab
    /// on Photon servers; disables scene camera(MainCamera)
    /// </summary>
    public void Start()
    {

        audio.volume = PlayerPrefs.GetFloat("musicVolume");

        Vector2 StartingPos = new Vector2(0, 15);
        GameObject player = PhotonNetwork.Instantiate(CharacterPrefab.name, StartingPos, Quaternion.identity);
        player.GetComponent<Controller>().camera.SetActive(true);
        player.GetComponent<Controller>().transitionState = (int)States.startingGame;
        if (PhotonNetwork.IsMasterClient)
        {
            player.GetComponent<Master>().enabled = true;
            master = player.GetComponent<Master>();
        }

    }
    public void Update()
    {
        List<Controller> players = ((Controller[])FindObjectsOfType(typeof(Controller))).ToList<Controller>();
        
        if (players.Count == PhotonNetwork.CurrentRoom.Players.Count && PhotonNetwork.IsMasterClient)
        {
            foreach (Controller player in players)
            {
                player.FindBed();
            }
            master.AssignRoles(players);
            master.gameObject.GetComponent<Controller>().timer.stopwatch.Reset();
            Destroy(this.gameObject);
        }
    }
}
