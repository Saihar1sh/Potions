using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Arixen.ScriptSmith;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Random = UnityEngine.Random;

public class GameScreen : ScreenBase
{
    [SerializeField] private int spawnCount = 10;
    [SerializeField] private bool infiniteSpawn = true;
    [SerializeField] private int spawnInterval = 5;

    [SerializeField] private AssetReferenceT<PotionsCollectionScriptable> potionCollectionRef;

    private PotionsCollectionScriptable potionCollectionScriptable;
    private float spawnTimer = 0;

    private async void Start()
    {
        potionCollectionScriptable = null;
        var scriptable = await AddressablesManager.Instance.LoadAssetAsync<PotionsCollectionScriptable>(potionCollectionRef);

        potionCollectionScriptable = scriptable;
        Debug.Log("PotionCollection loaded successfully.");
        await PotionSpawnLoop();
    }

    private async Task PotionSpawnLoop()
    {
        while (infiniteSpawn || spawnCount > 0)
        {
            SpawnPotions();
            await Task.Delay(TimeSpan.FromSeconds(spawnInterval));
            if (!infiniteSpawn) spawnCount--;
        }
    }

    private void SpawnPotions()
    {
        var cell = GridManager.Instance.GetRandomCell();
        var index = Random.Range(0, potionCollectionScriptable.potionScriptableList.Count);
        var randomPotionData = potionCollectionScriptable.potionScriptableList[index];
        cell.SetPotion(randomPotionData);

        EventBusService.InvokeEvent(new PotionSpawnedEvent()
        {
            potionType = randomPotionData.potionType,
            cellIndex = cell.Index
        });
    }
}