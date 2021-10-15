﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Class - attached to Launcher emtpy object; runs when Game.unity scene is opened
/// by any client; prepares game to run on local client
/// </summary>
public class GameLauncher : MonoBehaviour
{

    public GameObject CharacterPrefab;
    public GameObject MainCamera;

    // Text that is displayed at the center of the scene
    [SerializeField]
    private Text code;

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

        if (!PhotonNetwork.CurrentRoom.IsVisible)
        {
            code.text = PhotonNetwork.CurrentRoom.Name;
        }
        
        Vector2 StartingPos = new Vector2(0, 5);
        PhotonNetwork.Instantiate(CharacterPrefab.name, StartingPos, Quaternion.identity);

        MainCamera.SetActive(false);

    }
}
