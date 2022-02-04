using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{

    private bool sleeping = false;
    private bool day = true;
    private bool infected = false;

    public Movement movement;
    public Interact interact;
    public PhotonView view;
    public Role role;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool GetSleeping()
    {
        return this.sleeping;
    }
    public void SetSleeping(bool sleeping)
    {
        this.sleeping = sleeping;
    }
    public bool GetDay()
    {
        return this.day;
    }
    public void SetDay(bool day)
    {
        this.day = day;
    }
    public bool GetInfected()
    {
        return infected;
    }
    public void SetInfected(bool infected)
    {
        this.infected = infected;
    }
}
