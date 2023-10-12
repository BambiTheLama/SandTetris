using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum CellType{
    sand,fire,wood,
}
public class CellScript : MonoBehaviour
{
    public bool isEmpty { private set; get; } = true;
    public CellType type { private set; get; }

    public Color color;

    SpriteRenderer sprite;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = color;
    }

    public void disactivateCell() {

        isEmpty = true;
        if(!sprite)
            sprite = GetComponent<SpriteRenderer>();
        //sprite.enabled = false;
        color=Color.white;
        sprite.color = color;

    }

    public void setCellValue(CellType type,Color c)
    {
        if (!isEmpty)
            return;
        color = c;
        this.type = type;
        //if (!sprite)
        //    sprite = GetComponent<SpriteRenderer>();
        //sprite.enabled = true;
        sprite.color = c;
        isEmpty = false;
    }
}
