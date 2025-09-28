using System;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System.Threading.Tasks;
using Arixen.ScriptSmith;

public class FirebaseManager : MonoGenericLazySingleton<FirebaseManager>
{
    private FirebaseAuth auth;
    private FirebaseUser user;
    private DatabaseReference databaseReference;

    private string userId = "";

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        InitializeFirebase();
    }

    void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
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

    private void Start()
    {
        SignInAnonymously();
    }

    void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }

    public void SignInAnonymously()
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


    public void SaveUserScore(int score)
    {
        if (string.IsNullOrEmpty(userId))
            return;

        UserData userData = new UserData(SystemInfo.deviceName, score, System.DateTime.Now.ToString(), 1);

        string json = JsonUtility.ToJson(userData);

        EventBusService.InvokeEvent(new FirebaseSyncStartedEvent() { operationType = "SaveUserScore" });
        databaseReference.Child("users").Child(userId).SetRawJsonValueAsync(json)
            .ContinueWith(task =>
            {
                EventBusService.InvokeEvent(new FirebaseSyncCompletedEvent() { success = task.IsCompleted, operationType = "SaveUserScore" });
                if (task.IsCompleted)
                {
                    Debug.Log("User score saved!");
                }
            });
    }

    private void LoadUserData()
    {
        if (string.IsNullOrEmpty(userId))
            return;

        EventBusService.InvokeEvent(new FirebaseSyncStartedEvent() { operationType = "LoadUserScore" });
        databaseReference.Child("users").Child(userId).GetValueAsync()
            .ContinueWith(task =>
            {
                EventBusService.InvokeEvent(new FirebaseSyncCompletedEvent() { success = task.IsCompleted, operationType = "LoadUserScore" });
                if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    if (snapshot.Exists)
                    {
                        string jsonData = snapshot.GetRawJsonValue();
                        UserData userData = JsonUtility.FromJson<UserData>(jsonData);
                        Debug.Log($"Loaded: {userData.username}, Score: {userData.score}");
                    }
                }
            });
    }

    public void SaveToLeaderboard(string username, int score)
    {
        if (string.IsNullOrEmpty(userId))
            return;

        LeaderboardEntry entry = new LeaderboardEntry(username, score, userId);
        string json = JsonUtility.ToJson(entry);

        databaseReference.Child("leaderboard").Push().SetRawJsonValueAsync(json)
            .ContinueWith(task =>
            {
                if (task.IsCompleted) Debug.Log("Leaderboard updated!");
            });
    }

    public void LoadLeaderboard(System.Action<List<LeaderboardEntry>> onComplete)
    {
        databaseReference.Child("leaderboard").OrderByChild("score").LimitToLast(5)
            .GetValueAsync().ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    List<LeaderboardEntry> leaderboard = new List<LeaderboardEntry>();
                    foreach (DataSnapshot childSnapshot in snapshot.Children)
                    {
                        string json = childSnapshot.GetRawJsonValue();
                        if (!string.IsNullOrEmpty(json))
                        {
                            LeaderboardEntry entry = JsonUtility.FromJson<LeaderboardEntry>(json);
                            leaderboard.Add(entry);
                        }
                    }

                    leaderboard.Sort((a, b) => b.score.CompareTo(a.score));
                    onComplete?.Invoke(leaderboard);
                }
            });
    }
}

[System.Serializable]
public class UserData
{
    public string username;
    public int score;
    public string lastUpdated;
    public int level;

    public UserData(string username, int score, string lastUpdated, int level)
    {
        this.username = username;
        this.score = score;
        this.lastUpdated = lastUpdated;
        this.level = level;
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