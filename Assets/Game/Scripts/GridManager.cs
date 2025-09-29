using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Arixen.ScriptSmith;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GridManager : MonoGenericLazySingleton<GridManager>
{
    public Transform cellsParent;

    [SerializeField] private CellView[] gridCells;
    
    protected override void Awake()
    {
        base.Awake();
        gridCells = cellsParent.GetComponentsInChildren<CellView>();
        EventBusService.Subscribe<GameStartedEvent>(OnGameStarted);
    }

    private void OnGameStarted(GameStartedEvent e)
    {
        ResetAllCells();
    }

    private void ResetAllCells()
    {
        for (var index = 0; index < gridCells.Length; index++)
        {
            var gridCell = gridCells[index];
            gridCell.Init(index);
        }
    }

    public CellView GetRandomCell()
    {
        var randomIndex = Random.Range(0, gridCells.Length);
        return gridCells[randomIndex];
    }
}