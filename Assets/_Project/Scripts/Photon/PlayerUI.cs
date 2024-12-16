using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public static PlayerUI Create(GameObject prefab, Transform parent, string name)
    {
        var instance = Instantiate(prefab, parent).GetComponent<PlayerUI>();
        instance.SetPlayerName(name);
        return instance;
    }

    
    public static void Destroy(PlayerUI element)
    {
        Destroy(element.gameObject);
    }
    
    [SerializeField] private TextMeshProUGUI playerName;
    
    public void SetPlayerName(string name)
    {
        playerName.text = name;
    }
}
