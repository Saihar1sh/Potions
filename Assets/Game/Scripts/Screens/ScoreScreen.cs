using System;
using System.Collections;
using System.Collections.Generic;
using Arixen.ScriptSmith;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class ScoreScreen : ScreenBase
{
    [SerializeField] private AssetReferenceT<GameObject> leaderboardEntryViewPrefabRef;

    [SerializeField] private Button backButton;

    [SerializeField] private Transform leaderboardEntryContainer;

    private List<LeaderboardEntryView> leaderboardEntryViews = new List<LeaderboardEntryView>();

    private LeaderboardEntryView leaderboardEntryViewPrefab;

    private bool isDoneInit = false;

    public override async void Init()
    {
        isDoneInit = false;
        leaderboardEntryViews = new List<LeaderboardEntryView>();

        Debug.Log($"[{screenType}] Init");
        backButton.onClick.AddListener(OnBackButtonPressed);
        EventBusService.Subscribe<LeaderboardLoadedEvent>(OnLeaderBoardLoaded);

        var leaderboardEntryViewPrefabObject = await AddressablesManager.Instance.LoadAssetAsync(leaderboardEntryViewPrefabRef);
        if (leaderboardEntryViewPrefabObject.TryGetComponent(out leaderboardEntryViewPrefab))
        {
            CreateAndCacheLeaderboardEntries();
        }
        else
        {
            Debug.Log("Error loading leaderboard entry prefab");
        }

        isDoneInit = true;
    }

    private void CreateAndCacheLeaderboardEntries()
    {
        for (var index = 0; index < 5; index++)
        {
            var leaderboardEntryView = Instantiate(leaderboardEntryViewPrefab, leaderboardEntryContainer);
            leaderboardEntryViews.Add(leaderboardEntryView);
            leaderboardEntryView.gameObject.SetActive(false);
        }
    }


    private void OnBackButtonPressed()
    {
        EventBusService.InvokeEvent(new HideScreenEvent(screenType));
    }
    
    private void OnEnable()
    {
        if (isDoneInit)
            FirebaseManager.Instance.LoadLeaderboard();
    }

    private async void OnLeaderBoardLoaded(LeaderboardLoadedEvent e)
    {
        // Ensure this method continues on the main thread
        await System.Threading.Tasks.Task.Yield(); 

        Debug.Log("OnLeaderBoardLoaded");
        LeaderboardEntryView leaderboardEntryView = null;
        for (var index = 0; index < e.topScores.Count; index++)
        {
            var entry = e.topScores[index];
            if (index >= leaderboardEntryViews.Count) //if there are more entries than there are views.
            {
                if (leaderboardEntryViewPrefab == null)
                    continue;
                var entryView = GameObject.Instantiate(leaderboardEntryViewPrefab, leaderboardEntryContainer);
                leaderboardEntryViews.Add(entryView);
                leaderboardEntryView = entryView;
            }
            else
            {
                leaderboardEntryView = leaderboardEntryViews[index];
            }

            leaderboardEntryView.gameObject.SetActive(true);

            leaderboardEntryView.Init(entry.username, entry.userId, entry.score);
        }
    }
}