using UnityEngine;

public class CellNormalTetrisScript : MonoBehaviour
{
    private SpriteRenderer _sprite;

    /// <summary>
    /// Kolor bloku.
    /// </summary>
    public Color Color;

    /// <summary>
    /// Flaga informująca, czy komórka jest pusta.
    /// </summary>
    public bool IsEmpty { get; private set; } = true;
    
    /// <summary>
    /// Typ komórki.
    /// </summary>
    public int Type { get; private set; }
    
    /// <summary>
    /// Inicjalizacja komórki.
    /// </summary>
    void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        Color = new Color(0.1f, 0.1f, 0.1f, 1f);
        _sprite.color = Color;
    }
    
    /// <summary>
    /// Deaktywuj komórkę i ustaw jej kolor na ciemny.
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
    /// Deaktywuj komórkę i wyczyść jej kolor (ustaw na przezroczysty).
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
    /// Ustaw wartości komórki na podstawie podanego typu i koloru.
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
        Color = cellType switch
        {
            1 => new Color(1, 0, 0, 1),
            2 => new Color(1, 1, 0, 1),
            3 => new Color(0, 1, 0, 1),
            4 => new Color(0, 0, 1, 1),
            _ => new Color(1, 1, 1, 1),
        };
        _sprite.color = Color;
        IsEmpty = false;
    }
}
