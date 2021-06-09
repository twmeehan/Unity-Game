using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoomHandler : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject CreateRoomPanel;
    [SerializeField] private GameObject MainMenuPanel;
    [SerializeField] private GameObject JoinRoomPanel;

    private List<RoomInfo> _roomList;
    [SerializeField]
    private Button checkboxUnchecked;
    [SerializeField]
    private Button checkboxChecked;
    [SerializeField]
    private Slider slider;
    [SerializeField]
    private Text maxPlayers;

    public void createRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte) Mathf.Ceil(slider.value);
        if (checkboxChecked.interactable)
        {
            roomOptions.IsVisible = false;
        } else
        {
            roomOptions.IsVisible = true;
        }
        string code = "0000";
        bool roomFound = false;

        while (!roomFound)
        {
            code = Mathf.Floor(Random.Range(0.01f, 9.99f)).ToString() +
                Mathf.Floor(Random.Range(0.01f, 9.99f)).ToString() +
                Mathf.Floor(Random.Range(0.01f, 9.99f)).ToString() +
                Mathf.Floor(Random.Range(0.01f, 9.99f)).ToString();
            roomFound = true;
            foreach (RoomInfo info in _roomList)
            {
                if (info.Name==code)
                {
                    roomFound = false;
                }
            }
        }
        PhotonNetwork.CreateRoom(code, roomOptions, TypedLobby.Default);
        Debug.Log("Creating Room " + code);

    }
    public void Check()
    {
        Debug.Log("check");
        checkboxChecked.interactable = true;
        checkboxUnchecked.interactable = false;
        checkboxChecked.transform.localPosition = new Vector3(-61, 40, 0);
        checkboxUnchecked.transform.localPosition = new Vector3(-400,40,0);
    }
    public void Uncheck()
    {
        Debug.Log("uncheck");

        checkboxChecked.interactable = false;
        checkboxUnchecked.interactable = true;
        checkboxChecked.transform.localPosition = new Vector3(-400, 40, 0);
        checkboxUnchecked.transform.localPosition = new Vector3(-59.75f, 40.25f, 0);

    }
    public void UpdateMaxPlayers()
    {
        maxPlayers.text = slider.value.ToString();
    }
    public void back()
    {
        CreateRoomPanel.SetActive(false);
        JoinRoomPanel.SetActive(false);
        MainMenuPanel.SetActive(true);
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        _roomList = roomList;
    }
}
