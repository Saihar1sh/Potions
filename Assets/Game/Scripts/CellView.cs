using System;
using Arixen.ScriptSmith;
using UnityEngine;
using UnityEngine.UI;

public class CellView : MonoBehaviour
{
    [SerializeField] private Image cellImage;

    private PotionScriptable currentPotionScriptable;

    public int Index { get; private set; }

    public void Init(int index)
    {
        Index = index;
        ClearPotion();
    }

    public void SetPotion(PotionScriptable potionScriptable)
    {
        currentPotionScriptable = potionScriptable;
        if (cellImage != null && potionScriptable.potionIcon != null)
        {
            cellImage.sprite = potionScriptable.potionIcon;
        }
    }

    public void OnClick()
    {
        EventBusService.InvokeEvent(new PotionCollectedEvent()
        {
            potionType = currentPotionScriptable.potionType,
            scoreValue = currentPotionScriptable.potionScoreValue,
            timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        });

        ClearPotion();
    }

    public void ClearPotion()
    {
        currentPotionScriptable = null;
        if (cellImage != null)
        {
            cellImage.sprite = null;
        }
    }
}