using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextBlockNormalTetrisScript : MonoBehaviour
{
    private static readonly int gridHeight = 4;
    private static readonly int gridWidth = 2;
    public CellNormalTetrisScript cellScript;
    readonly float ratio = 0.64f;
    readonly CellNormalTetrisScript[,] cells = new CellNormalTetrisScript[gridHeight, gridWidth];
    
    void Start()
    {
        GenerateGrid();
    }

    /// <summary>
    /// Czyści kolory wszystkich komórek w siatce.
    /// </summary>
    void ClearColor()
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                CellNormalTetrisScript cell = cells[y, x];
                cell.DeactivateCellClear();
            }
        }
    }

    /// <summary>
    /// Ustawia blok na siatce komórek.
    /// </summary>
    /// <param name="block">Blok do wyświetlenia na siatce.</param>
    public void SetBlockAtGrid(TetrisBlock block)
    {
        ClearColor();
        for (int x = 0; x < block.Width; x++)
        {
            for (int y = 0; y < block.Height; y++)
            {
                if (block.HasBlock(x, y))
                {
                    cells[y, x].SetCellValue(block.Type);
                }
            }
        }
    }

    /// <summary>
    /// Generuje siatkę komórek na podstawie ustawień początkowych.
    /// </summary>
    void GenerateGrid()
    {

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                CellNormalTetrisScript cell = Instantiate(cellScript,
                    new Vector2(transform.position.x + (x) * ratio, transform.position.y - (y) * ratio),
                    Quaternion.identity,
                    transform) as CellNormalTetrisScript;
                cells[y, x] = cell;
            }
        }

    }
}
