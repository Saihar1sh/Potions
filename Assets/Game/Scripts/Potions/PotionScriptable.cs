using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "New PotionScriptable", menuName = "ScriptableObjects/PotionScriptable", order = 1)]
public class PotionScriptable : ScriptableObject
{
    public PotionType potionType;
    public string potionName;
    public Sprite potionIcon;
    public int potionScoreValue;
    [Range(0f, 1f)] public float potency; // Potency from 0 to 1
    public string potionDescription;
}

public enum PotionType
{
    Health,
    Mana,
    Poison,
    Freeze,
}