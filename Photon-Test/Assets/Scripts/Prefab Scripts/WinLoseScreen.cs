using Michsky.UI.Dark;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinLoseScreen : MonoBehaviourPunCallbacks
{

    public GameObject win;
    public GameObject lose;
    public TextMeshProUGUI message;
    private bool displaying = false;
    private UIDissolveEffect dissolve;

    // Start is called before the first frame update
    void Start()
    {
        dissolve = this.gameObject.GetComponent<UIDissolveEffect>();

    }
    public void Win()
    {
        win.SetActive(true);
        lose.SetActive(false);
        displaying = true;
        message.text = "You won!";
    }
    public void Lose()
    {
        win.SetActive(false);
        lose.SetActive(true);
        displaying = true;
        message.text = "You lost.";
    }
    public void Quit()
    {
        PhotonNetwork.LeaveRoom();
    }
    // Update is called once per frame
    void Update()
    {
        if (displaying && dissolve.location > 0)
        {
            dissolve.location -= Time.deltaTime * 2;

        }
    }
    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        PhotonNetwork.LoadLevel("Menu");
    }
}
