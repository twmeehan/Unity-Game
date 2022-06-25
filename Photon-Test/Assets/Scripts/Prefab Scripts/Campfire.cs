using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Campfire : MonoBehaviour
{

    public Slider countdown;
    public Canvas canvas;
    private PhotonView view;
    private const int MAX_TIME = 30;
    private Timer timer = new Timer();
    private float pastTime = 0;

    public void Start()
    {
        this.view = this.gameObject.GetComponent<PhotonView>();
        if (PhotonNetwork.IsMasterClient)
        {
            AddWood();
        }
    }
    public void Update()
    {
        if (canvas.worldCamera == null)
        {
            canvas.worldCamera = Camera.main;
        }
        Debug.Log(timer.TimeRemaining());
        if (timer.IsRunning())
            countdown.value = timer.TimeRemaining() / MAX_TIME;
        if (timer.TimeRemaining() < -1 && timer.TimeRemaining() != pastTime)
        {
            foreach (Controller controller in (Controller[])FindObjectsOfType(typeof(Controller)))
            {
                if (controller.view.Owner.UserId == PhotonNetwork.LocalPlayer.UserId)
                {
                    if (controller.role == "killer")
                    {
                        ((WinLoseScreen[])FindObjectsOfType(typeof(WinLoseScreen)))[0].Win();

                    }
                    else if (controller.role == "gloomling")
                    {
                        ((WinLoseScreen[])FindObjectsOfType(typeof(WinLoseScreen)))[0].Lose();

                    }
                }
            }
        }
        pastTime = timer.TimeRemaining();
        if (PhotonNetwork.IsMasterClient)
        {
            Log[] logs = (Log[])FindObjectsOfType(typeof(Log));
            foreach (Log log in logs)
            {
                if (Mathf.Sqrt(Mathf.Pow(Mathf.Abs(log.gameObject.transform.position.x - this.transform.position.x), 2) + 
                    Mathf.Pow(Mathf.Abs(log.gameObject.transform.position.y - this.transform.position.y),2)) < 3)
                {
                    PhotonNetwork.Destroy(log.gameObject.GetComponent<PhotonView>());
                    AddWood();
                }
            }
        }
    }
    public void AddWood()
    {
        view.RPC("AddTimeRPC", RpcTarget.All, (float)PhotonNetwork.Time);
    }
    [PunRPC]
    public void AddTimeRPC(float startTime)
    {
        timer.SetTimer(MAX_TIME, startTime);
    }


}
