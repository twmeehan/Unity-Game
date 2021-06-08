using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject playerPrefab;

    [SerializeField]
    private Text code;

    public float minX, maxX, minY, maxY;

    public void Start()
    {
        code.text = PhotonNetwork.CurrentRoom.Name.ToString();
        Vector2 randomPos = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
        PhotonNetwork.Instantiate(playerPrefab.name, randomPos, Quaternion.identity);
        
    }
}
