using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PotionScriptable", menuName = "ScriptableObjects/PotionScriptable", order = 1)]
public class PotionScriptable : ScriptableObject
{
    public PotionType potionType;
    public string potionName;
    public Sprite potionIcon;
    public int potionScoreValue;
    public string potionDescription;
}

public enum PotionType
{
    Health,
    Mana,
    Poison,
    Freeze,
}