using System;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Room : MonoBehaviour
{
    public static Room Create(GameObject prefab, Transform parent, string name, int count, int max)
    {
        GameObject obj = Instantiate(prefab, parent);
        Room room = obj.GetComponent<Room>();
        room.SetRoomInfo(name, count, max);
        return room;
    }
    
    public static void Destroy(Room room)
    {
        Destroy(room.gameObject);
    }

    
    [SerializeField] private TextMeshProUGUI roomName;
    [SerializeField] private TextMeshProUGUI playerCount;
    private Button joinButton;

    private void Start()
    {
        joinButton = GetComponent<Button>();
        joinButton.onClick.AddListener(JoinRoom);
    }

    private void JoinRoom()
    {
        PhotonNetwork.JoinRoom(roomName.text);
    }

    public void SetRoomInfo(string name, int count, int max)
    {
        SetRoomName(name);
        SetPlayerCount(count, max);
    }
    
    public void SetRoomName(string name)
    {
        roomName.text = name;
    }
    
    public void SetPlayerCount(int count, int max)
    {
        playerCount.text = $"{count}/{max}";
    }
}