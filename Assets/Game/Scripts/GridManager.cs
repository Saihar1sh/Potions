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
    }
    
    public CellView GetRandomCell()
    {
        var index = Random.Range(0,gridCells.Length);
        return gridCells[index];
    }
}