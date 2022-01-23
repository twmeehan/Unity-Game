using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InfectionIndicator : MonoBehaviour
{

    public PlayerScript p;
    private bool init = false;

    // Start is called before the first frame update
    void Start()
    {

        this.gameObject.GetComponent<SpriteRenderer>().enabled = false;

    }

    // Update is called once per frame
    void Update()
    {
        
        if (p.getInfected())
            this.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        else
            this.gameObject.GetComponent<SpriteRenderer>().color = Color.green;

    }
}
