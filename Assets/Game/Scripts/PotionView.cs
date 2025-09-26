using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PotionView : MonoBehaviour
{
    [SerializeField] private Image potionImage;

    


    public void Init(PotionScriptable potionScriptable)
    {
        potionImage.sprite = potionScriptable.potionIcon;
            }
}