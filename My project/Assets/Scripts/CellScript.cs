using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CellType
{
    SandYellow, SandRed, SandBlue, SandGreen //, Fire, Wood
}

public class CellScript : MonoBehaviour
{
    public bool IsEmpty { get; private set; } = true;
    public CellType Type { get; private set; }

    public Color Color;

    private SpriteRenderer _sprite;

    private void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _sprite.color = Color;
    }

    public void DeactivateCell()
    {
        IsEmpty = true;
        if (!_sprite)
            _sprite = GetComponent<SpriteRenderer>();
        Color = new Color(0.1f, 0.1f, 0.1f, 1f);
        _sprite.color = Color;
    }
    public void DeactivateCellClear()
    {
        IsEmpty = true;
        if (!_sprite)
            _sprite = GetComponent<SpriteRenderer>();
        Color = new Color(0f, 0f, 0f, 0f);
        _sprite.color = Color;
    }

    public void SetCellValue(CellType cellType, Color cellColor)
    {
        if (!IsEmpty)
            return;

        Color = cellColor;
        Type = cellType;
        _sprite.color = cellColor;
        IsEmpty = false;
    }
    public void SetWhite()
    {
        _sprite.color = Color.white;
    }
}
