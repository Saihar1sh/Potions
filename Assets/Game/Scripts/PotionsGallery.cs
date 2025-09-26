using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class PotionsGallery : MonoBehaviour
{
    [SerializeField]
    private PotionScriptable[] potionScriptableObjects;
    
    public TextMeshProUGUI potionNameText;
    public TextMeshProUGUI potionDescriptionText;

    private void Awake()
    {
        
    }

    private void OnPotionClicked(PotionScriptable potionScriptable)
    {
        potionNameText.text = potionScriptable.potionName;
        potionDescriptionText.text = $"Score acquired by collecting: {potionScriptable.potionScoreValue}\nDescription: {potionScriptable.potionDescription}";

    }
}
