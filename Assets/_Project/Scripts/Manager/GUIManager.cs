using System;
using UnityEngine;

public class GUIManager : MonoBehaviour, IEventListener<GameEvent>
{
    [SerializeField] private GameObject inGamePanel;
    [SerializeField] private GameObject mainMenuPanel;
    
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

    private void SetInGamePanel(bool b)
    {
        if (inGamePanel == null)
        {
            Debug.LogWarning("InGamePanel is null");
            return;
        }

        inGamePanel.SetActive(b);
    }
    
    private void SetMainMenuPanel(bool b)
    {
        if (mainMenuPanel == null)
        {
            Debug.LogWarning("MainMenuPanel is null");
            return;
        }

        mainMenuPanel.SetActive(b);
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
