using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PotionView : MonoBehaviour,IPointerDownHandler
{
    [SerializeField] private Image potionImage;

    //TODO: there's coupling here for potionscriptable
    private Action<PotionScriptable> _onPotionClicked;

    private PotionScriptable _potionScriptable;

    public void Init(PotionScriptable potionScriptable, Action<PotionScriptable> onPotionClicked)
    {
        potionImage.sprite = potionScriptable.potionIcon;
        _onPotionClicked = onPotionClicked;
        _potionScriptable = potionScriptable;
    }
    
    private void OnDestroy()
    {
        _onPotionClicked = null;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Potion clicked");
        _onPotionClicked?.Invoke(_potionScriptable);
    }
}