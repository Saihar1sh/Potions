using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenBase : MonoBehaviour
{
   public ScreenType screenType;

   public bool showHUD;
   public bool shouldInitByDefault;
   
   public virtual void Init() { }
}

public enum ScreenType
{
    StartScreen,
    GameScreen,
    PauseScreen,
    ScoreScreen,
    PotionsGallery,
}