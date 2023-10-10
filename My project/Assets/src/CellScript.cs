using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum CellType{
    sand,fire,wood,
}
public class CellScript : MonoBehaviour
{
    public bool isEmpty { private set; get; }
    public CellType type { private set; get; }

    public Color color;

    SpriteRenderer sprite;

    void Start()
    {
        isEmpty = true;
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = color;
    }

    public void disactivateCell() {

        isEmpty = false;
        sprite.enabled = false;
    }

    public void setCellValue(CellType type,Color c)
    {
        if (!isEmpty)
            return;

        color = c;
        this.type = type;
        sprite.enabled = true;

        sprite.color = c;
    }
}
