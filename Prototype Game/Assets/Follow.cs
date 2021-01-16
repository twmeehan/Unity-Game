using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{

    public Transform player;

    // Start is called before the first frame update
    void Start()
    {
         
    }

    // Update is called once per frame
    void Update()
    {
        transform.position= new Vector3(player.position.x + (Mathf.Cos(player.rotation.eulerAngles.y*Mathf.Deg2Rad)*20) - 20, player.position.y+10, player.position.z + (Mathf.Sin(player.rotation.eulerAngles.y * Mathf.Deg2Rad) * 20) + 20);
        transform.eulerAngles = new Vector3(player.eulerAngles.x, player.eulerAngles.y+180, player.eulerAngles.z);
    }
}
