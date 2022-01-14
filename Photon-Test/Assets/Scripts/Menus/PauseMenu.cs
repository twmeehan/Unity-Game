using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviourPunCallbacks
{

    // Checks if game is paused.
    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    //Resumes the Game
    public void Resume ()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    //Pauses the Game
    void Pause ()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 1f;
        GameIsPaused = true;
    }
/*
    public void Options ()
    {

    }
*/
    //Loads the Start Menu
    public void LoadMenu()
    {
        Debug.Log("Leaving Room");
        PhotonNetwork.LeaveRoom();

    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        Debug.Log("Joined Lobby");
        PhotonNetwork.LoadLevel("Menu");
    }
}
