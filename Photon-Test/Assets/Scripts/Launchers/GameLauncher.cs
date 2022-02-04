﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;
using ExitGames.Client.Photon;

/// <summary>
/// Class - attached to Launcher emtpy object; runs when Game.unity scene is opened
/// by any client; prepares game to run on local client
/// </summary>
public class GameLauncher : MonoBehaviour
{

    public GameObject CharacterPrefab;
    public GameObject MainCamera;
    public AudioSource audio;

    private float timeSinceSleeping;
    private bool allSleepingLastFrame;
    /// <summary>
    /// Method - runs on init; displays room code on screen; instantiates character prefab
    /// on Photon servers; disables scene camera(MainCamera)
    /// </summary>
    public void Start()
    {
        /*

        string[] array = new string[1];
        array[0] = "TIM";
        PhotonNetwork.CurrentRoom.SetPropertiesListedInLobby(array);

        */
        audio.volume = PlayerPrefs.GetFloat("musicVolume");

        Vector2 StartingPos = new Vector2(0, 15);
        PhotonNetwork.Instantiate(CharacterPrefab.name, StartingPos, Quaternion.identity);

        MainCamera.SetActive(false);

    }
    public void FixedUpdate()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (checkIfAllPlayersAreSleeping())
            {
                if (!allSleepingLastFrame)
                    timeSinceSleeping = Time.time; 
                else if (Time.time - timeSinceSleeping > 0.5f)
                {
                    Debug.Log("All players sleeping - Sending Event to transition to night");
                    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                    PhotonNetwork.RaiseEvent(2, null, raiseEventOptions, SendOptions.SendReliable);

                }

                allSleepingLastFrame = true;
                
                
            }
            else
            {
                allSleepingLastFrame = false;
            }
        }
    }

    public bool checkIfAllPlayersAreSleeping()
    {
        foreach (Controller player in FindObjectsOfType(typeof(Controller)))
        {
            if (!player.GetSleeping())
                return false;
        }
        return true;
    }
}
