using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InfectionIndicator : MonoBehaviour
{

    public PlayerScript p;

    // Start is called before the first frame update
    void Start()
    {

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
