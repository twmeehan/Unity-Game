using Photon.Pun;
using UnityEngine;

public class QuickStartRoomController : MonoBehaviourPunCallBacks
{
	[SerializeField]
	private int multiplayerSceneIndex;

	public override void OnEnable()
	{
		PhotonNetwork.AddCallbackTarget(this);
	}

	public override void OnDisable()
	{
		PhotonNetwork.RemoveCallbackTarget(this);
	}
	
	public override void OnJoinedRoom()
	{
		Debug.Log("Joined Room");
		StartGame();
	}

	public void StartGame()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			Debug.Log("Starting Game");
			PhotonNetwork LoadLevel(multiplayerSceneIndex);
		}
	}
}
