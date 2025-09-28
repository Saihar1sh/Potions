using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PotionCollection", menuName = "ScriptableObjects/PotionCollection", order = 1)]
public class PotionsCollectionScriptable : ScriptableObject
{
    public List<PotionScriptable> potionScriptableList = new List<PotionScriptable>();
}
