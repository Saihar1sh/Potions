using Arixen.ScriptSmith;

public class ShowScreenEvent : IGameEventData
{
    public ScreenType screenType;

    public ShowScreenEvent(ScreenType screenType)
    {
        this.screenType = screenType;
    }
}

public class HideScreenEvent : IGameEventData
{
    public ScreenType screenType;

    public HideScreenEvent(ScreenType screenType)
    {
        this.screenType = screenType;
    }
}

public class GameStartedEvent : IGameEventData
{
    public long timestamp;
    public string sessionId;
}

public class GamePausedEvent : IGameEventData
{
    public long timestamp;
}

public class GameResumedEvent : IGameEventData
{
    public long timestamp;
}

public class GameEndedEvent : IGameEventData
{
    public long timestamp;
    public int totalScore;
}

public class PotionSpawnedEvent : IGameEventData
{
    public PotionType potionType;
    public int cellIndex;
}

public class PotionCollectedEvent : IGameEventData
{
    public long timestamp;
    public PotionType potionType;
    public int scoreValue;
}

public class ScoreUpdatedEvent : IGameEventData
{
    public int newScore;
    public int scoreDelta;
}

public class LeaderboardLoadedEvent : IGameEventData
{
    public LeaderboardEntry[] topScores;
}

public class FirebaseSyncStartedEvent : IGameEventData
{
    public string operationType;
}

public class FirebaseSyncCompletedEvent : IGameEventData
{
    public bool success;
    public string operationType;
}