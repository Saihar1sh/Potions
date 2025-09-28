using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PotionsGalleryScreen : ScreenBase
{
    [SerializeField] private PotionsCollectionScriptable potionCollectionScriptable;

    [SerializeField] private PotionView potionViewPrefab;

    [SerializeField] private Transform potionsContainer;

    [SerializeField] private TextMeshProUGUI potionNameText;
    [SerializeField] private TextMeshProUGUI potionDescriptionText;
    [SerializeField] private Image potionImage;

    private void Awake()
    {
        foreach (var potionScriptable in potionCollectionScriptable.potionScriptableList)
        {
            CreatePotionView(potionScriptable);
        }
    }

    private void CreatePotionView(PotionScriptable potionScriptable)
    {
        var potionView = Instantiate(potionViewPrefab, potionsContainer);
        potionView.Init(potionScriptable, OnPotionClicked);
    }

    private void OnPotionClicked(PotionScriptable potionScriptable)
    {
        potionNameText.text = potionScriptable.potionName;
        potionImage.sprite = potionScriptable.potionIcon;
        potionDescriptionText.text = $"Score acquired by collecting: {potionScriptable.potionScoreValue}\n\n\nDescription: {potionScriptable.potionDescription}";
    }
}