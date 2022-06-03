using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campfire : MonoBehaviour
{

    public Timer timer;

    public void startCampfireTimer()
    {
        timer.SetTimer(60, (float) PhotonNetwork.Time);
    }

}
