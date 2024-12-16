using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class GUIManagerPhoton : Singleton<GUIManagerPhoton>, IEventListener<GameEvent>
{
    [Header("Room")]
    [SerializeField] private GameObject roomPrefab;
    [SerializeField] private Transform container;
    
    [Header("PlayerUI")] [SerializeField]
    GameObject playerUIPrefab;

    [SerializeField] private Transform playerUIContainer;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TMP_InputField playerName;
    
    [Header("Panel")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject inGamePanel;
    [SerializeField] private GameObject inRoomPanel;

    [SerializeField] private MenuPanelController menuPanel;
    
    private Dictionary<string, Room> roomList = new();
    private Dictionary<Player, PlayerUI> playerList = new();
    
    private PhotonView photonView;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        SetInRoomPanel(false);
        SetInGamePanel(false);
        SetMainMenuPanel(true);
    }
    
    public void SetNamePlayer(string name)
    {
        playerName.text = name;
    }
    
    public void AddListenerEditName(Action<string> action)
    {
        playerName.onEndEdit.AddListener(action.Invoke);
    }
    
    public void SetInteractableCreateRoom(bool b)
    {
        menuPanel.SetInteractableCreateRoom(b);
    }
    
    public void SetStatusTextVisible(bool b)
    {
        statusText.gameObject.SetActive(b);
    }
    
    public void SetStatusText(string text)
    {
        statusText.text = text;
    }

    public void StartGame()
    {
        SetStatusText("");
        photonView.RPC("StartGamePunRPC", RpcTarget.All);
    }
    
    [PunRPC]
    public void StartGamePunRPC()
    {
        GameEvent.Trigger(GameEventType.GameStart);
    }

    public void SetMainMenuPanel(bool b)
    {
        if (mainMenuPanel == null)
        {
            Debug.LogWarning("MainMenuPanel is null");
            return;
        }

        mainMenuPanel.SetActive(b);
    }
    
    public void SetInGamePanel(bool b)
    {
        if (inGamePanel == null)
        {
            Debug.LogWarning("InGamePanel is null");
            return;
        }

        inGamePanel.SetActive(b);
    }
    
    public void SetInRoomPanel(bool b)
    {
        if (inRoomPanel == null)
        {
            Debug.LogWarning("InRoomPanel is null");
            return;
        }

        inRoomPanel.SetActive(b);
    }
    
    public void SetStartGameButtonActive(bool active)
    {
        menuPanel.SetStartGameButtonActive(active);
    }

    public void RoomListUpdate(List<RoomInfo> rooms)
    {
        foreach (var room in rooms)
        {
            if (room.RemovedFromList)
            {
                if(roomList.TryGetValue(room.Name, out var roomElement))
                {
                    Room.Destroy(roomElement);
                    roomList.Remove(room.Name);
                }
            }
            else
            {
                if (roomList.TryGetValue(room.Name, out var roomElement))
                {
                    roomElement.SetPlayerCount(room.PlayerCount,room.MaxPlayers);
                }
                else
                {
                    var newRoomElement = Room.Create(roomPrefab, container, room.Name, room.PlayerCount, room.MaxPlayers);
                    roomList.Add(room.Name, newRoomElement);
                }
            }
        }
    }
    
    public void AddPlayerElement(Player player)
    {
        Debug.Log(player.NickName);
        var playerElement = PlayerUI.Create(playerUIPrefab, playerUIContainer, player.NickName);
        playerList.Add(player, playerElement);
    }
    
    public void RemovePlayerElement(Player player)
    {
        if (playerList.TryGetValue(player, out var playerElement))
        {
            PlayerUI.Destroy(playerElement);
            playerList.Remove(player);
        }
    }
    
    public void ClearPlayerElements()
    {
        foreach (var player in playerList.ToList())
        {
            RemovePlayerElement(player.Key);
        }
        playerList.Clear();

        for (int i = 0; i < playerUIContainer.childCount; i++)
        {
            Destroy(playerUIContainer.GetChild(i).gameObject);
        }
    }

    public void UpdateCurrentRoomPlayers()
    {
        var players = PhotonNetwork.CurrentRoom.Players;
        
        foreach (var player in players)
        {
            if (playerList.ContainsKey(player.Value)) continue;
            AddPlayerElement(player.Value);
        }
    }


    public void OnEvent(GameEvent e)
    {
        switch (e.EventType)
        {
            case GameEventType.GameStart:
                SetInGamePanel(true);
                SetMainMenuPanel(false);
                break;
        }
    }
    
    private void OnEnable()
    {
        this.StartListening();
    }
    
    private void OnDisable()
    {
        this.StopListening();
    }
    
    
}