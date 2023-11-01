using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CellNormalTetrisScript : MonoBehaviour
{
    private SpriteRenderer _sprite;
    public Color Color;
    public bool IsEmpty { get; private set; } = true;
    public int Type { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        Color = new Color(0.1f, 0.1f, 0.1f, 1f);
        _sprite.color = Color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Deaktywuj komórkê i ustaw jej kolor na ciemny.
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
    /// Deaktywuj komórkê i wyczyœæ jej kolor (ustaw na przezroczysty).
    /// </summary>
    public void DeactivateCellClear()
    {
        Type = 0;
        IsEmpty = true;
        if (!_sprite)
            _sprite = GetComponent<SpriteRenderer>();
        Color = new Color(0.1f, 0.1f, 0.1f, 1f);
        //Color = new Color(0f, 0f, 0f, 0f); // Ustaw przezroczysty kolor
        _sprite.color = Color;
    }
    /// <summary>
    /// Ustaw wartoœci komórki na podstawie podanego typu i koloru.
    /// </summary>
    /// <param name="cellType">Typ bloku</param>
    public void SetCellValue(int type)
    {
        if (!IsEmpty)
            return;
        if(type==0)
        {
            DeactivateCell();
            return;
        }
        
        Type = type;
        switch(type)
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
