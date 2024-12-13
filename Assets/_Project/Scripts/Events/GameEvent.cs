public enum GameEventType
{
    GameMainMenu,
    GamePreStart,
    GameStart,
    GameRunning,
    GameOver,
    GamePause,
    TogglePause,
}
public struct GameEvent
{
    public GameEventType EventType;

    private GameEvent(GameEventType eventType) { EventType = eventType; }

    public static void Trigger(GameEventType eventType) { EventManger.TriggerEvent(new GameEvent(eventType)); }
}