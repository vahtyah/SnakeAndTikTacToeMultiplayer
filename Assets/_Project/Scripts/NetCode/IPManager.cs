using System.Net;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class IPManager : SingletonNetwork<IPManager>, IEventListener<GameEvent>
{
    [SerializeField] private TextMeshProUGUI IPText;
    private string localIP;

    void Start()
    {
        localIP = GetLocalIPAddress();
        Debug.Log("IP: " + localIP);
    }

    public string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }

        throw new System.Exception("Không tìm thấy địa chỉ IPv4 cục bộ!");
    }

    public void OnEvent(GameEvent e)
    {
        switch (e.EventType)
        {
            case GameEventType.GameStart:
                SetIPTextServerRpc();
                break;
        }
    }

    [ServerRpc]
    private void SetIPTextServerRpc()
    {
        if (IsServer && IPText != null)
            IPText.text = "IP: " + localIP;
    }

    private void OnEnable() { this.StartListening(); }

    private void OnDisable() { this.StopListening(); }
}