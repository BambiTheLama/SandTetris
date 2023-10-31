using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Typy komórek piasku.
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
/// Klasa reprezentuj¹ca pojedyncz¹ komórkê piasku.
/// </summary>
public class CellScript : MonoBehaviour
{
    /// <summary>
    /// Flaga informuj¹ca, czy komórka jest pusta.
    /// </summary>
    public bool IsEmpty { get; private set; } = true;

    /// <summary>
    /// Typ komórki.
    /// </summary>
    public CellType Type { get; private set; }

    /// <summary>
    /// Kolor komórki.
    /// </summary>
    public Color Color;

    private SpriteRenderer _sprite;

    /// <summary>
    /// Inicjalizacja komórki.
    /// </summary>
    private void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _sprite.color = Color;
    }

    /// <summary>
    /// Deaktywuj komórkê i ustaw jej kolor na ciemny.
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
    /// Deaktywuj komórkê i wyczyœæ jej kolor (ustaw na przezroczysty).
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
    /// Ustaw wartoœci komórki na podstawie podanego typu i koloru.
    /// </summary>
    /// <param name="cellType">Typ komórki piasku.</param>
    /// <param name="cellColor">Kolor komórki.</param>
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
    /// Ustaw kolor komórki na bia³y.
    /// </summary>
    public void SetWhite()
    {
        _sprite.color = Color.white;
    }
}
