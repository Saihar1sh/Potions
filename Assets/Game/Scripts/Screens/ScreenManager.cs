using System;
using System.Collections;
using System.Collections.Generic;
using Arixen.ScriptSmith;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    [SerializeField] private ScreenBase[] screens;

    private Dictionary<ScreenType, ScreenBase> screenDictionary = new Dictionary<ScreenType, ScreenBase>();

    [SerializeField] private GameObject hud;

    private ScreenBase activeScreen;
    private ScreenBase prevActiveScreen;

    private void Awake()
    {
        CacheScreensToDictionary();
        EventBusService.Subscribe<ShowScreenEvent>(ShowScreen);
        EventBusService.Subscribe<HideScreenEvent>(HideScreen);
        ShowAllScreens(false);

        foreach (var screen in screens)
        {
            if (screen.shouldInitByDefault)
                screen.Init();
        }
    }

    private void OnDestroy()
    {
        EventBusService.UnSubscribe<ShowScreenEvent>(ShowScreen);
        EventBusService.UnSubscribe<HideScreenEvent>(HideScreen);
    }

    private void CacheScreensToDictionary()
    {
        screenDictionary = new Dictionary<ScreenType, ScreenBase>();
        foreach (var screen in screens)
        {
            screenDictionary.Add(screen.screenType, screen);
        }
    }

    public bool GetScreen(ScreenType screenType, out ScreenBase screen) => screenDictionary.TryGetValue(screenType, out screen);

    private void ShowAllScreens(bool enable = true)
    {
        foreach (var screen in screenDictionary.Values)
        {
            screen.gameObject.SetActive(enable);
        }
    }

    private void ShowScreen(ShowScreenEvent e)
    {
        var screen = screenDictionary[e.screenType];
        screen.gameObject.SetActive(true);
        hud.SetActive(screen.showHUD);
        if (e.hideAllPreviousScreens)
        {
            ShowAllScreens(false);
        }

        OnActiveScreenChanged(screen);
    }

    private void OnActiveScreenChanged(ScreenBase screen)
    {
        if (activeScreen != null)
        {
            activeScreen.gameObject.SetActive(false);
            prevActiveScreen = activeScreen;
        }

        activeScreen = screen;
        screen.gameObject.SetActive(true);
        EventBusService.InvokeEvent(new OnActiveScreenChangedEvent(activeScreen.screenType));
    }

    private void HideScreen(HideScreenEvent e)
    {
        var screen = screenDictionary[e.screenType];
        screen.gameObject.SetActive(false);
        OnActiveScreenChanged(prevActiveScreen);
        hud.gameObject.SetActive(false);
    }
    
}