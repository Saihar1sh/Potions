using System;
using System.Collections;
using System.Collections.Generic;
using Arixen.ScriptSmith;
using UnityEngine;
using UnityEngine.UI;

public class PauseScreen : ScreenBase
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button exitButton;

    private void Awake()
    {
        resumeButton.onClick.AddListener(OnResumeButtonPressed);
        exitButton.onClick.AddListener(OnExitButtonPressed);
    }

    private void OnExitButtonPressed()
    {
        EventBusService.InvokeEvent(new ShowScreenEvent(ScreenType.StartScreen, hideAllPreviousScreens: true));
        EventBusService.InvokeEvent(new GameEndedEvent(){ timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(), totalScore = GameLoopManager.Instance.Score});
    }

    private void OnResumeButtonPressed()
    {
        EventBusService.InvokeEvent(new GameResumedEvent(){ timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()});
        EventBusService.InvokeEvent(new ShowScreenEvent(ScreenType.GameScreen));
    }
}