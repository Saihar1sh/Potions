using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Arixen.ScriptSmith;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameLoopManager : MonoBehaviour
{
    [SerializeField] private int spawnCount = 10;
    [SerializeField] private bool infiniteSpawn = true;
    [SerializeField] private int spawnInterval = 5;

    [SerializeField] private PotionsCollectionScriptable potionCollectionScriptable;

    private float spawnTimer = 0;

    private void Start()
    {
        EventBusService.InvokeEvent(new ShowScreenEvent(ScreenType.GameScreen));

         PotionSpawnLoop();
    }

    private async Task PotionSpawnLoop()
    {
        while (infiniteSpawn || spawnCount > 0)
        {
            await SpawnPotionsRoutine();
        }
    }

    private async Task SpawnPotionsRoutine()
    {
        var cell = GridManager.Instance.GetRandomCell();
        var index = Random.Range(0, potionCollectionScriptable.potionScriptableList.Count);
        var randomPotionIcon = potionCollectionScriptable.potionScriptableList[index].potionIcon;
        cell.Init(randomPotionIcon);
        await Task.Delay(TimeSpan.FromSeconds(spawnInterval));

        if (!infiniteSpawn) spawnCount--;
    }
}