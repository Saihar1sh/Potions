using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Analytics;
using System.Threading.Tasks;
using Arixen.ScriptSmith;

public class FirebaseManager : MonoGenericLazySingleton<FirebaseManager>
{
    private FirebaseAuth auth;
    private FirebaseUser user;
    private DatabaseReference databaseReference;

    private string userId = "";
    private string username => SystemInfo.deviceName;

    private long sessionStartTime;
    private int sessionCount = 0;
    private Dictionary<PotionType, int> potionCollectionCounts = new Dictionary<PotionType, int>();

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        InitializeFirebase();
        EventBusService.Subscribe<GameStartedEvent>(OnGameStarted);
        EventBusService.Subscribe<GameEndedEvent>(OnGameEnded);
        EventBusService.Subscribe<PotionCollectedEvent>(OnPotionCollected);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventBusService.UnSubscribe<GameStartedEvent>(OnGameStarted);
        EventBusService.UnSubscribe<GameEndedEvent>(OnGameEnded);
        EventBusService.UnSubscribe<PotionCollectedEvent>(OnPotionCollected);
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }

    private void Start()
    {
    }

    private void OnGameStarted(GameStartedEvent e)
    {
        sessionStartTime = e.timestamp;
        sessionCount++;
        potionCollectionCounts.Clear();
        FirebaseAnalytics.SetUserProperty("session_count", sessionCount.ToString());
    }

    private void OnPotionCollected(PotionCollectedEvent e)
    {
        if (potionCollectionCounts.ContainsKey(e.potionType))
        {
            potionCollectionCounts[e.potionType]++;
        }
        else
        {
            potionCollectionCounts[e.potionType] = 1;
        }
    }

    void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
                auth.StateChanged += AuthStateChanged;
                AuthStateChanged(this, null);
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                Debug.Log("Firebase initialized and Analytics collection enabled.");
            }
            else
            {
                Debug.LogError(string.Format("Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
            SignInAnonymously();

        });
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            user = auth.CurrentUser;
            if (signedIn)
            {
                userId = user.UserId;
                Debug.Log("Signed in: " + userId);
            }
        }
    }
    
    private void OnGameEnded(GameEndedEvent e)
    {
        SaveToLeaderboard(username, e.totalScore);
        SaveUserData(e.totalScore);
        
        Debug.Log($"Setting user property: player_level = 1");
        FirebaseAnalytics.SetUserProperty("player_level", "1");
        Debug.Log($"Setting user property: last_active_date = {DateTime.UtcNow.ToString("yyyy-MM-dd")}");
        FirebaseAnalytics.SetUserProperty("last_active_date", DateTime.UtcNow.ToString("yyyy-MM-dd"));

        if (potionCollectionCounts.Count > 0)
        {
            var preferredPotion = potionCollectionCounts.OrderByDescending(kvp => kvp.Value).First().Key;
            Debug.Log($"Setting user property: preferred_potion_type = {preferredPotion.ToString()}");
            FirebaseAnalytics.SetUserProperty("preferred_potion_type", preferredPotion.ToString());
        }
    }

    private void SignInAnonymously()
    {
        auth.SignInAnonymouslyAsync().ContinueWith(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                AuthResult result = task.Result;
                userId = result.User.UserId;
                Debug.Log("Signed in: " + userId);
                LoadUserData();
            }
        });
    }

    public void SignOut()
    {
        auth.SignOut();
    }


    private void SaveUserData(int score)
    {
        if (string.IsNullOrEmpty(userId))
            return;

        UserData userData = new UserData(username, score,  userId,DateTimeOffset.UtcNow.ToUnixTimeSeconds(), sessionStartTime, sessionCount);

        string json = JsonUtility.ToJson(userData);

        EventBusService.InvokeEvent(new FirebaseSyncStartedEvent() { operationType = FirebaseSyncOperation.SaveUserData });
        databaseReference.Child("users").Child(userId).SetRawJsonValueAsync(json)
            .ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("SaveUserData failed with exception: " + task.Exception);
                }
                EventBusService.InvokeEvent(new FirebaseSyncCompletedEvent() { success = task.IsCompleted && !task.IsFaulted, operationType = FirebaseSyncOperation.SaveUserData });
                if (task.IsCompleted && !task.IsFaulted)
                {
                    Debug.Log("User score saved!");
                }
            });
    }

    private void LoadUserData()
    {
        if (string.IsNullOrEmpty(userId))
            return;

        EventBusService.InvokeEvent(new FirebaseSyncStartedEvent() { operationType = FirebaseSyncOperation.LoadUserData });
        databaseReference.Child("users").Child(userId).GetValueAsync()
            .ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("LoadUserData failed with exception: " + task.Exception);
                    EventBusService.InvokeEvent(new FirebaseSyncCompletedEvent() { success = false, operationType = FirebaseSyncOperation.LoadUserData });
                    return;
                }

                if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    if (snapshot.Exists)
                    {
                        string jsonData = snapshot.GetRawJsonValue();
                        UserData userData = JsonUtility.FromJson<UserData>(jsonData);
                        sessionCount = userData.sessionCount;
                        Debug.Log($"Loaded: {userData.username}, Score: {userData.score}, Sessions: {userData.sessionCount}");
                    }
                }

                EventBusService.InvokeEvent(new FirebaseSyncCompletedEvent() { success = task.IsCompleted && !task.IsFaulted, operationType = FirebaseSyncOperation.LoadUserData });
            });
    }

    public void SaveToLeaderboard(string username, int score)
    {
        if (string.IsNullOrEmpty(userId))
            return;

        DatabaseReference userLeaderboardEntryRef = databaseReference.Child("leaderboard").Child(userId);

        userLeaderboardEntryRef.GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted) {
                Debug.LogError("Failed to read leaderboard entry: " + task.Exception);
                EventBusService.InvokeEvent(new FirebaseSyncCompletedEvent() { success = false, operationType = FirebaseSyncOperation.SaveToLeaderboard });
                return;
            }
            string scoreName = nameof(LeaderboardEntry.score);

            if (task.IsCompleted) {
                DataSnapshot snapshot = task.Result;
                int existingScore = 0;
                if (snapshot.Exists && snapshot.Child(scoreName).Value != null) {
                    existingScore = Convert.ToInt32(snapshot.Child(scoreName).Value);
                }
                
                if (!snapshot.Exists || score > existingScore) {
                    LeaderboardEntry entry = new LeaderboardEntry(username, score, userId);
                    string json = JsonUtility.ToJson(entry);
                    EventBusService.InvokeEvent(new FirebaseSyncStartedEvent() { operationType = FirebaseSyncOperation.SaveToLeaderboard });

                    userLeaderboardEntryRef.SetRawJsonValueAsync(json)
                        .ContinueWith(writeTask =>
                        {
                            if (writeTask.IsCompleted && !writeTask.IsFaulted) Debug.Log("Leaderboard updated with new high score!");
                            else if (writeTask.IsFaulted) Debug.LogError("SaveToLeaderboard write failed with exception: " + writeTask.Exception);

                            EventBusService.InvokeEvent(new FirebaseSyncCompletedEvent() { success = writeTask.IsCompleted && !writeTask.IsFaulted, operationType = FirebaseSyncOperation.SaveToLeaderboard });
                        });
                } else {
                    Debug.Log("New score is not higher than the existing leaderboard score. No update needed.");
                    EventBusService.InvokeEvent(new FirebaseSyncCompletedEvent() { success = true, operationType = FirebaseSyncOperation.SaveToLeaderboard });
                }
            }
        });
    }

    public async void LoadLeaderboard()
    {
        EventBusService.InvokeEvent(new FirebaseSyncStartedEvent() { operationType = FirebaseSyncOperation.LoadLeaderboard });

        string scoreName = nameof(LeaderboardEntry.score);
        await databaseReference.Child("leaderboard").OrderByChild(scoreName).LimitToLast(5)
            .GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("LoadLeaderboard failed with exception: " + task.Exception);
                    EventBusService.InvokeEvent(new FirebaseSyncCompletedEvent() { success = false, operationType = FirebaseSyncOperation.LoadLeaderboard });
                    return;
                }

                if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    Debug.Log($"LoadLeaderboard - Snapshot Exists: {snapshot.Exists}");
                    if (snapshot.Exists)
                    {
                        Debug.Log($"LoadLeaderboard - Snapshot Children Count: {snapshot.ChildrenCount}");
                        List<LeaderboardEntry> leaderboard = new List<LeaderboardEntry>();
                        foreach (DataSnapshot childSnapshot in snapshot.Children)
                        {
                            string json = childSnapshot.GetRawJsonValue();
                            Debug.Log($"LoadLeaderboard - Child JSON: {json}");
                            if (!string.IsNullOrEmpty(json))
                            {
                                LeaderboardEntry entry = JsonUtility.FromJson<LeaderboardEntry>(json);
                                leaderboard.Add(entry);
                            }
                        }

                        leaderboard.Sort((a, b) => b.score.CompareTo(a.score));

                        EventBusService.InvokeEvent(new LeaderboardLoadedEvent() { topScores = leaderboard });
                    }
                    else
                    {
                        Debug.Log("LoadLeaderboard - No data found in leaderboard node.");
                    }
                }

                EventBusService.InvokeEvent(new FirebaseSyncCompletedEvent() { success = task.IsCompleted && !task.IsFaulted, operationType = FirebaseSyncOperation.LoadLeaderboard });
            });
    }
}

[System.Serializable]
public class UserData
{
    public string username;
    public int score;
    public long lastUpdated;
    public string userId;
    public long sessionStartTime;
    public int sessionCount;

    public UserData(string username, int score, string userId, long lastUpdated, long sessionStartTime, int sessionCount)
    {
        this.username = username;
        this.score = score;
        this.lastUpdated = lastUpdated;
        this.userId = userId;
        this.sessionStartTime = sessionStartTime;
        this.sessionCount = sessionCount;
    }
}

[System.Serializable]
public class LeaderboardEntry
{
    public string username;
    public int score;
    public string userId;

    public LeaderboardEntry(string username, int score, string userId)
    {
        this.username = username;
        this.score = score;
        this.userId = userId;
    }
}