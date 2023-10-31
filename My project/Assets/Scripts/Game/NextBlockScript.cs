using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Klasa kontroluj�ca wy�wietlanie nast�pnego bloku w grze.
/// </summary>
public class NextBlockScript : MonoBehaviour
{
    private static int gridHeight = 40, gridWidth = 40;
    readonly CellScript[,] cells = new CellScript[gridHeight, gridWidth];
    public CellScript cellScript;
    readonly float ratio = 0.16f;

    /// <summary>
    /// Inicjalizacja siatki kom�rek i czyszczenie kolor�w.
    /// </summary>
    void Start()
    {
        GenerateGrid();
        ClearColor();
    }

    /// <summary>
    /// Ustawia blok na siatce kom�rek.
    /// </summary>
    /// <param name="block">Blok do wy�wietlenia na siatce.</param>
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
    /// Generuje siatk� kom�rek na podstawie ustawie� pocz�tkowych.
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
    /// Czy�ci kolory wszystkich kom�rek w siatce.
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
