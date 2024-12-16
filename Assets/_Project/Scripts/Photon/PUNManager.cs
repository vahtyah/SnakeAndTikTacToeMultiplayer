using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Random = UnityEngine.Random;

public class PUNManager : MonoBehaviourPunCallbacks
{
    private GUIManagerPhoton GUI;
    private void Start()
    {
        GUI = GUIManagerPhoton.Instance;
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.NickName = "Player_" + Random.Range(0, 1000);
        GUI.SetNamePlayer(PhotonNetwork.NickName);
        GUI.AddListenerEditName((name) => PhotonNetwork.NickName = name);
        GUI.SetInteractableCreateRoom(false);
        
        StartCoroutine(Reconnection());
    }
    
    IEnumerator Reconnection()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);

            if (!PhotonNetwork.IsConnected || PhotonNetwork.NetworkClientState == ClientState.ConnectingToMasterServer)
            {
                PhotonNetwork.ConnectUsingSettings();
            }
        }
    }

    private void Update()
    {
        if(!PhotonNetwork.IsConnectedAndReady)
            GUI.SetStatusText(PhotonNetwork.NetworkClientState.ToString());
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Joined Lobby");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        GUI.SetMainMenuPanel(true);
        GUI.SetStatusText("");
        GUI.SetInteractableCreateRoom(true);
    }

    public override void OnJoinedRoom()
    {
        GUI.SetInRoomPanel(true);
        GUI.UpdateCurrentRoomPlayers();
        GUI.SetStartGameButtonActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join room: " + message + " Code: " + returnCode);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to create room: " + message + " Code: " + returnCode);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        GUI.AddPlayerElement(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        GUI.RemovePlayerElement(otherPlayer);
        GUI.SetStartGameButtonActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnLeftRoom()
    {
        GUI.SetInRoomPanel(false);
        GUI.SetInGamePanel(false);
        GUI.SetMainMenuPanel(true);
        GUI.SetInteractableCreateRoom(true);
        GUI.SetStatusText("");
        GUI.ClearPlayerElements();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        GUI.RoomListUpdate(roomList);
    }
}
