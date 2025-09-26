using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public Transform cellsParent;
    
    public CellView[] potions;

    private void Awake()
    {
        potions = GetComponentsInChildren<CellView>();
    }

    private void Start()
    {
        
    }
}
