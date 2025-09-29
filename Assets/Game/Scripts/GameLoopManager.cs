using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Arixen.ScriptSmith;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameLoopManager : MonoGenericLazySingleton<GameLoopManager>
{
    public int Score { get; private set; }


    protected override void Awake()
    {
        base.Awake();
        EventBusService.Subscribe<OnActiveScreenChangedEvent>(OnActiveScreenChanged);
        EventBusService.Subscribe<GameStartedEvent>(OnGameStarted);
        EventBusService.Subscribe<PotionCollectedEvent>(OnPotionCollected);
        EventBusService.Subscribe<FirebaseSyncStartedEvent>(OnFirebaseSyncStarted);
        EventBusService.Subscribe<FirebaseSyncCompletedEvent>(OnFirebaseSyncCompleted);
    }

    private void OnGameStarted(GameStartedEvent e)
    {
        
        Score = 0;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventBusService.UnSubscribe<OnActiveScreenChangedEvent>(OnActiveScreenChanged);
    }
    private void OnPotionCollected(PotionCollectedEvent e)
    {
        EventBusService.InvokeEvent(new ScoreUpdatedEvent(){ newScore = Score+e.scoreValue, scoreDelta = e.scoreValue});

        Score += e.scoreValue;
    }

    private void OnFirebaseSyncStarted(FirebaseSyncStartedEvent e)
    {
        Debug.Log($"Started sync -- {e.operationType}");
    }

    private void OnFirebaseSyncCompleted(FirebaseSyncCompletedEvent e)
    {
        Debug.Log($"Completed sync -result: {e.success} -- {e.operationType}");
    }

    private void Start()
    {
        EventBusService.InvokeEvent(new ShowScreenEvent(ScreenType.StartScreen));
    }

    private void OnActiveScreenChanged(OnActiveScreenChangedEvent e)
    {
        Debug.Log($"screenType: {e.screenType}");
    }
}