using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class Spawn - spawns logs every 30seconds or so if there is not enough logs
public class Spawn : MonoBehaviour
{

    // log prefab
    public GameObject log;

    private float timeTillNextLog;
    public int maxLogs;
    private int timeBetweenLogs;
    private float time;


    // Start is called before the first frame update
    void Start()
    {
        // only the master client spawns logs
        if (!PhotonNetwork.IsMasterClient)
        {
            Destroy(this.gameObject);
        }
        maxLogs--;

        // always spawn a log when starting the game
        SpawnLog();

        // get the time between logs as set by the master client at the start of the game
        PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("l", out object val);
        timeBetweenLogs = (int)val;
    }

    // Update is called once per frame
    void Update()
    {

        // has a random amount of time passed between 30-40 seconds (by default)
        time += Time.deltaTime;
        if (time > timeTillNextLog)
        {
            // if so try and spawn a log
            TryToSpawnLog();
            time = 0;
        }
    }
    // Method TryToSpawnLog() - if all conditions are met run SpawnLog();
    private bool TryToSpawnLog()
    {

        // get list of all logs
        Log[] logs = (Log[])FindObjectsOfType(typeof(Log));

        // if there are more logs than maxLogs do not spawn another log and wait another 30-40 seconds (by default)
        if (logs.Length > maxLogs)
        {
            timeTillNextLog = Mathf.Floor(Random.value * 10) + timeBetweenLogs;
            return false;
        }
        else
        {
            
            // check if there are any logs nearby
            foreach (Log log in logs)
            {
                if (Mathf.Abs(log.gameObject.transform.position.x - transform.position.x) < 5)
                {
                    timeTillNextLog = Mathf.Floor(Random.value * 10) + timeBetweenLogs;
                    return false;
                }

            }
            // if none of the logs are withing 5 units and there are less than maxLogs, spawn a new log
            SpawnLog();
            return true;
        }

    }
    // Method SpawnLog() - creates log
    private void SpawnLog()
    {
        GameObject newLog = PhotonNetwork.Instantiate(log.name, transform.position + new Vector3(0,50,0), Quaternion.identity);
        newLog.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        newLog.GetComponent<Rigidbody2D>().angularVelocity = -150;
        timeTillNextLog = Mathf.Floor(Random.value * 10) + timeBetweenLogs;
    }
}
