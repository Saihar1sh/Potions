using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellView : MonoBehaviour
{
    [SerializeField] private Image cellImage;
    
    public void Init(Sprite sprite)
    {
        cellImage.sprite = sprite;
    }

    public void OnClick()
    {
        cellImage.sprite = null;
    }
}
