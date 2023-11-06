using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Typy kom�rek piasku.
/// </summary>
public enum CellType
{

    SandRed,
    SandBlue,
    SandGreen,
    SandYellow,
    Fire,
    Wood,
    Water,
    Glass,
    Steam,
    NON
}

/// <summary>
/// Klasa reprezentuj�ca pojedyncz� kom�rk� piasku.
/// </summary>
public class CellScript : MonoBehaviour
{
    public int timer { get; private set; } = 0;
    const int timerMax = 200;
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
    public CellType Type { get; private set; } = CellType.NON;

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
        timer = 0;
        Type = CellType.NON;
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
        timer = 0;
        Type=CellType.NON;
    }

    /// <summary>
    /// Ustaw warto�ci kom�rki na podstawie podanego typu i koloru.
    /// </summary>
    /// <param name="cellType">Typ kom�rki piasku.</param>
    /// <param name="cellColor">Kolor kom�rki.</param>
    public void SetCellValue(CellType cellType, Color cellColor,int time=timerMax)
    {
        if (!IsEmpty)
            return;

        Color = cellColor;
        Type = cellType;
        _sprite.color = cellColor;
        IsEmpty = false;
        timer = time;
    }

    /// <summary>
    /// Ustaw kolor kom�rki na bia�y.
    /// </summary>
    public void SetWhite()
    {
        _sprite.color = Color.white;
    }

    public void updateTimer()
    {
        if (Type == CellType.Fire) 
        {

            Color = Color.Lerp(Color.yellow, Color.red, (float)timer / (float)timerMax);
            _sprite.color = Color;
            timer--;
            if (timer<0)
            {
                DeactivateCell();
            }    
        }
    }
}
