using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScript : MonoBehaviour
{
    private static int gridW = 80, gridH = 120;
    public CellScript cellScript;
    public float ratio = 0.9f;
    CellScript[,] cells = new CellScript[gridH, gridW];

    // Start is called before the first frame update
    void Start()
    {
        generateGrid();
    }

    // Update is called once per frame
    void Update()
    {
        setColor();
    }

    void generateGrid()
    {
        for(int y = 0; y < gridH; y++)
        {
            for (int x = 0; x < gridW; x++)
            {
                CellScript cell = Instantiate(cellScript,
                    new Vector2(transform.position.x + (x) * ratio, transform.position.y - (y) * ratio),
                    Quaternion.identity,
                    transform) as CellScript;
                cells[y, x] = cell;
            }
        }

    }
    void setColor()
    {
        for (int y = 0; y < gridH; y++)
        {
            for (int x = 0; x < gridW; x++)
            {
                CellScript cell = cells[y, x];
                cell.setCellValue(CellType.sand,
                    new Color((float)x / gridW, (float)y / gridW, 1, 1));
                cell.disactivateCell();
            }
        }
        
    }
}
