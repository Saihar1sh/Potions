using System;
using System.Collections;
using System.Collections.Generic;
using Arixen.ScriptSmith;
using UnityEngine;
using UnityEngine.UI;

public class StartScreen : ScreenBase
{
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button potionsGalleryButton;
    [SerializeField] private Button scoreScreenButton;
    [SerializeField] private Button exitGameButton;
    
    
    private void Awake()
    {
        startGameButton.onClick.AddListener(StartGame);
        potionsGalleryButton.onClick.AddListener(OpenPotionsGallery);
        scoreScreenButton.onClick.AddListener(OpenScoreScreen);
        exitGameButton.onClick.AddListener(ExitGame);
    }

    private void StartGame()
    {
        EventBusService.InvokeEvent(new ShowScreenEvent(ScreenType.GameScreen));
        EventBusService.InvokeEvent(new GameStartedEvent(){ sessionId = Guid.NewGuid().ToString(), timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()});
    }

    private void OpenPotionsGallery()
    {
        EventBusService.InvokeEvent(new ShowScreenEvent(ScreenType.PotionsGallery));
    }

    private void OpenScoreScreen()
    {
        EventBusService.InvokeEvent(new ShowScreenEvent(ScreenType.ScoreScreen));
    }

    private void ExitGame()
    {
        Debug.Log("Exit Game");
        Application.Quit();
    }
}