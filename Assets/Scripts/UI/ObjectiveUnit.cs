using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//TODO удалить скрипт
public class ObjectiveUnit : MonoBehaviour
{
    [SerializeField] Image image;

    public void Init(Sprite sprite, Color col) 
    {
        image.sprite = sprite;
        image.color = col;
    }

    public void Init(Sprite sprite)
    {
        image.sprite = sprite;
    }
}
