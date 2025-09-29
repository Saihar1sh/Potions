using System;
using System.Collections;
using System.Collections.Generic;
using Arixen.ScriptSmith;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PotionsGalleryScreen : ScreenBase
{
    [SerializeField] private string potionCollectionScriptableKey;

    [SerializeField] private PotionView potionViewPrefab;

    [SerializeField] private Transform potionsContainer;

    [SerializeField] private TextMeshProUGUI potionNameText;
    [SerializeField] private TextMeshProUGUI potionDescriptionText;
    [SerializeField] private Image potionImage;
    [SerializeField] private Button backButton;

    private PotionsCollectionScriptable potionCollectionScriptable;

    private async void Awake()
    {
        backButton.onClick.AddListener(OnBackButtonPressed);
        var scriptable = await AddressablesManager.Instance.LoadAssetAsync<PotionsCollectionScriptable>(potionCollectionScriptableKey);
        potionCollectionScriptable = scriptable;
        for (var index = 0; index < potionCollectionScriptable.potionScriptableList.Count; index++)
        {
            var potionScriptable = potionCollectionScriptable.potionScriptableList[index];
            var potionView = CreatePotionView(potionScriptable);

            //Clicking the first potion by default
            if (index == 0)
            {
                potionView.OnPointerDown(null);
            }
        }
    }

    private void OnBackButtonPressed()
    {
        EventBusService.InvokeEvent(new HideScreenEvent(ScreenType.PotionsGallery));
    }

    private PotionView CreatePotionView(PotionScriptable potionScriptable)
    {
        var potionView = Instantiate(potionViewPrefab, potionsContainer);
        potionView.Init(potionScriptable, OnPotionClicked);
        return potionView;
    }

    private void OnPotionClicked(PotionScriptable potionScriptable)
    {
        potionNameText.text = potionScriptable.potionName;
        potionImage.sprite = potionScriptable.potionIcon;
        potionDescriptionText.text = $"Score acquired by collecting: {potionScriptable.potionScoreValue}" +
                                     $"\n\nPotency: {potionScriptable.potency}" +
                                     $"\n\n\nDescription: {potionScriptable.potionDescription}";
    }
}