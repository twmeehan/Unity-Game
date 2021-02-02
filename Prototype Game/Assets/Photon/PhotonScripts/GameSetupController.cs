using Photon.Pun;
using System.IO;
using UnityEngine;

public class GameStepController : MonoBehaviour
{
	void Start()
	{
		CreatePlayer();
	}

	private void CreatePlayer()
	{
		Debug.Log("Creating Player");
		PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonPlayer"), Vector3.zero, Quaternion.identity);
	}
}
