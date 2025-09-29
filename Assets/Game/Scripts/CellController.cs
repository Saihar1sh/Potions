using System.Collections;
using System.Collections.Generic;
using Arixen.ScriptSmith;
using UnityEngine;
using UnityEngine.EventSystems;

public class CellController : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private CellView cellView;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        cellView.OnClick();
    }
}
