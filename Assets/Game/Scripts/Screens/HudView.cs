using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Arixen.ScriptSmith;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudView : MonoBehaviour
{
    [SerializeField] private Button pauseButton;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI scoreDeltaText;

    private CancellationTokenSource _cts;

    private void Awake()
    {
        EventBusService.Subscribe<ScoreUpdatedEvent>(OnScoreUpdated);
        scoreDeltaText.text = "";

        pauseButton.onClick.AddListener(OnPauseButtonPressed);
        EventBusService.Subscribe<GameStartedEvent>(OnGameStarted);

    }

    private void OnGameStarted(GameStartedEvent e)
    {
        scoreText.text = $"Score: 000";
    }

    private void OnDestroy()
    {
        EventBusService.UnSubscribe<ScoreUpdatedEvent>(OnScoreUpdated);
    }

    private void OnPauseButtonPressed()
    {
        EventBusService.InvokeEvent(new GamePausedEvent(){timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()});
        EventBusService.InvokeEvent(new ShowScreenEvent(ScreenType.PauseScreen));
    }

    private async void OnScoreUpdated(ScoreUpdatedEvent e)
    {
        scoreText.text = $"Score: {e.newScore}";
        scoreDeltaText.text = $"+{e.scoreDelta}";

        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        try
        {
            await Task.Delay(TimeSpan.FromSeconds(1), _cts.Token);
        }
        catch (TaskCanceledException exception)
        {
        }

        scoreDeltaText.text = "";
    }
}