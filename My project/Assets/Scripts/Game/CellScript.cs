using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Typy kom�rek piasku.
/// </summary>
public enum CellType
{
    SandYellow,
    SandRed,
    SandBlue,
    SandGreen,
    size
    // Fire,
    // Wood
}

/// <summary>
/// Klasa reprezentuj�ca pojedyncz� kom�rk� piasku.
/// </summary>
public class CellScript : MonoBehaviour
{
    private SpriteRenderer _sprite;

    /// <summary>
    /// Kolor kom�rki.
    /// </summary>
    public Color Color;

    /// <summary>
    /// Flaga informuj�ca, czy kom�rka jest pusta.
    /// </summary>
    public bool IsEmpty { get; private set; } = true;

    /// <summary>
    /// Typ kom�rki.
    /// </summary>
    public CellType Type { get; private set; }

    /// <summary>
    /// Inicjalizacja kom�rki.
    /// </summary>
    private void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _sprite.color = Color;
    }

    /// <summary>
    /// Deaktywuj kom�rk� i ustaw jej kolor na ciemny.
    /// </summary>
    public void DeactivateCell()
    {
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
        IsEmpty = true;
        if (!_sprite)
            _sprite = GetComponent<SpriteRenderer>();
        Color = new Color(0f, 0f, 0f, 0f); // Ustaw przezroczysty kolor
        _sprite.color = Color;
    }

    /// <summary>
    /// Ustaw warto�ci kom�rki na podstawie podanego typu i koloru.
    /// </summary>
    /// <param name="cellType">Typ kom�rki piasku.</param>
    /// <param name="cellColor">Kolor kom�rki.</param>
    public void SetCellValue(CellType cellType, Color cellColor)
    {
        if (!IsEmpty)
            return;

        Color = cellColor;
        Type = cellType;
        _sprite.color = cellColor;
        IsEmpty = false;
    }

    /// <summary>
    /// Ustaw kolor kom�rki na bia�y.
    /// </summary>
    public void SetWhite()
    {
        _sprite.color = Color.white;
    }
}
