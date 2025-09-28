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

    private void Awake()
    {
        CacheScreensToDictionary();
        EventBusService.Subscribe<ShowScreenEvent>(ShowScreen);
        EventBusService.Subscribe<HideScreenEvent>(HideScreen);
        ShowAllScreens(false);
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

    public void ShowAllScreens(bool enable = true)
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

        if (activeScreen != null)
            HideScreen(activeScreen);
        activeScreen = screen;
    }

    private void HideScreen(HideScreenEvent e)
    {
        var screen = screenDictionary[e.screenType];
        HideScreen(screen);
        hud.gameObject.SetActive(false);
    }

    private void HideScreen(ScreenBase screen)
    {
        screen.gameObject.SetActive(false);
    }
}