using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CellNormalTetrisScript : MonoBehaviour
{
    private SpriteRenderer _sprite;

    /// <summary>
    /// Kolor bloku.
    /// </summary>
    public Color Color;

    /// <summary>
    /// Flaga informuj�ca, czy kom�rka jest pusta.
    /// </summary>
    public bool IsEmpty { get; private set; } = true;
    
    /// <summary>
    /// Typ kom�rki.
    /// </summary>
    public int Type { get; private set; }
    
    /// <summary>
    /// Inicjalizacja kom�rki.
    /// </summary>
    void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        Color = new Color(0.1f, 0.1f, 0.1f, 1f);
        _sprite.color = Color;
    }
    
    /// <summary>
    /// Deaktywuj kom�rk� i ustaw jej kolor na ciemny.
    /// </summary>
    public void DeactivateCell()
    {
        Type = 0;
        IsEmpty = true;
        if (!_sprite)
            _sprite = GetComponent<SpriteRenderer>();
        Color = new Color(0.1f, 0.1f, 0.1f, 1f);
        _sprite.color = Color;
    }

    /// <summary>
    /// Deaktywuj kom�rk� i wyczy�� jej kolor (ustaw na przezroczysty).
    /// </summary>
    public void DeactivateCellClear()
    {
        Type = 0;
        IsEmpty = true;
        if (!_sprite)
            _sprite = GetComponent<SpriteRenderer>();
        Color = new Color(0f, 0f, 0f, 0f); // Ustaw przezroczysty kolor
        _sprite.color = Color;
    }
    /// <summary>
    /// Ustaw warto�ci kom�rki na podstawie podanego typu i koloru.
    /// </summary>
    /// <param name="cellType">Typ bloku</param>
    public void SetCellValue(int cellType)
    {
        if (!IsEmpty)
            return;
        if(cellType == 0)
        {
            DeactivateCell();
            return;
        }
        
        Type = cellType;
        switch(cellType)
        {
            case 1:
                Color = new Color(1, 0, 0,1);
            break;
            case 2:
                Color = new Color(1, 1, 0,1);
                break;
            case 3:
                Color = new Color(0, 1, 0,1);
                break;
            case 4:
                Color = new Color(0, 0, 1,1);
                break;
            default:
                Color = new Color(1, 1, 1,1);
                break;

        }
        _sprite.color = Color;
        IsEmpty = false;
    }
}
