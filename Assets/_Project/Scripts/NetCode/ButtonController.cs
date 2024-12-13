using System;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private GameObject inGamePanel;
    
    private void Awake()
    {
        hostButton.onClick.AddListener(Host);
        clientButton.onClick.AddListener(Client);
    }

    private void Host()
    {
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(IPManager.Instance.GetLocalIPAddress(), 7777);
        NetworkManager.Singleton.StartHost();
        GameEvent.Trigger(GameEventType.GameStart);
    }

    private void Client()
    {
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(inputField.text, 7777);
        NetworkManager.Singleton.StartClient();
        GameEvent.Trigger(GameEventType.GameStart);
    }
}
