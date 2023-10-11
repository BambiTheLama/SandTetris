using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScript : MonoBehaviour
{
    private static int gridW = 100, gridH = 200;
    public CellScript cellScript;
    public float ratio = 0.9f;
    CellScript[,] cells = new CellScript[gridH, gridW];
    Block b = null;
    // Start is called before the first frame update
    void Start()
    {
        generateGrid();
        setColor();
    }

    // Update is called once per frame
    void Update()
    {
        if (b == null)
            b = new Block(CellType.sand,(BlockType)Random.Range(0,(int)BlockType.size));
        else
        {
            deactivateBlockCells();
            if (Input.GetKeyDown(KeyCode.A))
                b.moveLeft();
            if (Input.GetKeyDown(KeyCode.D))
                b.moveRight();
            if(Input.GetKeyDown(KeyCode.W))
                b.rotateLeft();
            if (Input.GetKeyDown(KeyCode.R))
            {
                b = null;
                return;
            }

                
            activateBlockCells();
        }
    }

    void activateBlockCells()
    {
        for (int i = 0; i < b.h; i++)
            for (int j = 0; j < b.w; j++) 
            {
                if (!b.hasBlock(j, i))
                    continue;
                int x = j + b.x;
                int y = i + b.y;
                cells[y,x].setCellValue(b.cellType,b.GetColor(j,i));
            }
    }

    void deactivateBlockCells()
    {
        for (int i = 0; i < b.h; i++)
            for (int j = 0; j < b.w; j++)
            {
                if (!b.hasBlock(j, i))
                    continue;
                int x = j + b.x;
                int y = i + b.y;
                cells[y, x].disactivateCell();
            }
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
                cell.disactivateCell();
                cell.setCellValue(CellType.sand, Color.red);
            }
        }
        
    }
}
