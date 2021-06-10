using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject sceneCamera;

    [SerializeField]
    private Text code;

    public float minX, maxX, minY, maxY;

    public void Start()
    {
        code.text = PhotonNetwork.CurrentRoom.Name;
        Vector2 randomPos = new Vector2(200, 3000);
        PhotonNetwork.Instantiate(playerPrefab.name, randomPos, Quaternion.identity);
        sceneCamera.SetActive(false);
    }
}
