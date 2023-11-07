using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Klasa kontroluj¹ca wyœwietlanie nastêpnego bloku w grze.
/// </summary>
public class NextBlockScript : MonoBehaviour
{
    private static readonly int gridHeight = 40;
    private static int gridWidth = 40;
    readonly CellScript[,] cells = new CellScript[gridHeight, gridWidth];
    public CellScript cellScript;
    readonly float ratio = 0.16f;

    /// <summary>
    /// Inicjalizacja siatki komórek i czyszczenie kolorów.
    /// </summary>
    void Start()
    {
        GenerateGrid();
        ClearColor();
    }

    /// <summary>
    /// Ustawia blok na siatce komórek.
    /// </summary>
    /// <param name="block">Blok do wyœwietlenia na siatce.</param>
    public void SetBlockAtGrid(Block block)
    {
        ClearColor();
        for (int x = 0; x < block.Width; x++)
        {
            for (int y = 0; y < block.Height; y++)
            {
                if (block.HasBlock(x, y))
                {
                    cells[y, x].SetCellValue(block.CellType, block.GetColor(x, y));
                }
            }
        }
    }

    /// <summary>
    /// Generuje siatkê komórek na podstawie ustawieñ pocz¹tkowych.
    /// </summary>
    void GenerateGrid()
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                CellScript cell = Instantiate(cellScript,
                    new Vector2(transform.position.x + (x) * ratio, transform.position.y - (y) * ratio),
                    Quaternion.identity,
                    transform) as CellScript;
                cells[y, x] = cell;
            }
        }
    }

    /// <summary>
    /// Czyœci kolory wszystkich komórek w siatce.
    /// </summary>
    void ClearColor()
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                CellScript cell = cells[y, x];
                cell.DeactivateCellClear();
            }
        }
    }
}
