using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class MenuPanelController : MonoBehaviour
{
    [SerializeField] private Button createRoomButton;

    [SerializeField] private Button startGameButton;
    [SerializeField] private Button cancelButton;
    

    private void Start()
    {
        createRoomButton.onClick.AddListener(ButtonCreateRoomOnClick);
        startGameButton.onClick.AddListener(ButtonStartGameOnClick);
        cancelButton.onClick.AddListener(ButtonCancelOnClick);
    }

    private void ButtonCancelOnClick()
    {
        PhotonNetwork.LeaveRoom();
    }
    
    public void SetStartGameButtonActive(bool active)
    {
        startGameButton.gameObject.SetActive(active);
    }

    private void ButtonStartGameOnClick()
    {
        if(PhotonNetwork.CurrentRoom.PlayerCount < 2)
            return;
        GUIManagerPhoton.Instance.StartGame();
    }
    
    public void SetInteractableCreateRoom(bool b)
    {
        createRoomButton.interactable = b;
    }

    private void ButtonCreateRoomOnClick()
    {
        var name = "Room's " + PhotonNetwork.NickName;
        var maxPlayers = 2;
        var roomOptions = new Photon.Realtime.RoomOptions {MaxPlayers = (byte) maxPlayers};
        PhotonNetwork.CreateRoom(name, roomOptions);
    }
}
