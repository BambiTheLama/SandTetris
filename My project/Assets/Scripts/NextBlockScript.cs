using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextBlockScript : MonoBehaviour
{
    private static int gridHeight = 40, gridWidth = 40;
    CellScript[,] cells = new CellScript[gridHeight, gridWidth];
    public CellScript cellScript;
    float ratio = 0.16f;
    // Start is called before the first frame update
    void Start()
    {
        GenerateGrid();
        ClearColor();
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void SetBlockAtGrid(Block block)
    {
        ClearColor();
        for (int x = 0; x < block.Width; x++) 
        {
            for (int y = 0; y < block.Height; y++) 
            { 
                if(block.HasBlock(x, y))
                {
                    cells[y,x].SetCellValue(block.CellType,block.GetColor(x, y));
                }
            }
        }
    }

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
