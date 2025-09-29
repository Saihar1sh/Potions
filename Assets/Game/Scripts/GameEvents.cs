using System.Collections.Generic;
using Arixen.ScriptSmith;

public class ShowScreenEvent : IGameEventData
{
    public ScreenType screenType;
    
    public bool hideAllPreviousScreens = false;

    public ShowScreenEvent(ScreenType screenType, bool hideAllPreviousScreens = false)
    {
        this.screenType = screenType;
        this.hideAllPreviousScreens = hideAllPreviousScreens;
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

public class OnActiveScreenChangedEvent : IGameEventData
{
    public ScreenType screenType;

    public OnActiveScreenChangedEvent(ScreenType screenType)
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
    public List<LeaderboardEntry> topScores;
}
#region FireBase
public class FirebaseSyncStartedEvent : IGameEventData
{
    public FirebaseSyncOperation operationType;
}

public class FirebaseSyncCompletedEvent : IGameEventData
{
    public bool success;
    public FirebaseSyncOperation operationType;
}

public enum FirebaseSyncOperation
{
    SaveUserData,
    LoadUserData,
    SaveToLeaderboard,
    LoadLeaderboard
}

#endregion