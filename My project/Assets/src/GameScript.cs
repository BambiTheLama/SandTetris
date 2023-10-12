using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameScript : MonoBehaviour
{
    private static int gridW = 100, gridH = 200;
    public CellScript cellScript;
    public float ratio = 0.9f;
    CellScript[,] cells = new CellScript[gridH, gridW];
    Block b = null;
    List<Vector2Int> cellsToUpdate= new List<Vector2Int>();
    float timer=0.0f;
    // Start is called before the first frame update
    void Start()
    {
        generateGrid();
        setColor();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        int sandSpeed = 21;
        if((int)(timer* sandSpeed) >0)
        {
            for (int i = 0; i < (int)(timer * sandSpeed); i++)
                updateCells();
            timer -= ((int)(timer * sandSpeed))/(float) sandSpeed;
        }

        if (b == null)
            b = new Block(CellType.sand,(BlockType)Random.Range(0,(int)BlockType.size));
        else
        {
            Block tmp = b;
            b = new Block(tmp);
            deactivateBlockCells();
            if (Input.GetKeyDown(KeyCode.A))
                b.moveLeft();
            if (Input.GetKeyDown(KeyCode.D))
                b.moveRight();
            if(Input.GetKeyDown(KeyCode.W))
                b.rotateLeft();
            if (Input.GetKey(KeyCode.S))
                b.speedFalling();
            b.goDown(Time.deltaTime);
            if (checkCollisionBlock())
            {
                b = tmp;
                spawnBlockAtMap();
                return;
            }   
            
            activateBlockCells();
        }
    }

    bool moveCellBlock(Vector2Int active, Vector2Int nonactive)
    {
        if (cells[active.y, active.x].isEmpty)
            return false;
        if (!cells[nonactive.y, nonactive.x].isEmpty)
            return false;
        CellScript a = cells[active.y, active.x];
        cells[nonactive.y, nonactive.x].setCellValue(a.type, a.color);
        a.disactivateCell();
        return true;
    }
    void addBlocksToUpdate(Vector2Int pos, List<Vector2Int> list)
    {
        if (pos.y - 1 >= 0 && pos.y - 1 < gridH) 
        {
            if (pos.x - 1 >= 0)
            {
                if (!cells[pos.y - 1, pos.x - 1].isEmpty)
                    list.Add(new Vector2Int(pos.x - 1, pos.y - 1));
            }
            if (pos.x + 1 < gridW)
            {
                if (!cells[pos.y - 1, pos.x + 1].isEmpty)
                    list.Add(new Vector2Int(pos.x + 1, pos.y - 1));
            }
            if (!cells[pos.y - 1, pos.x].isEmpty)
                list.Add(new Vector2Int(pos.x, pos.y - 1));
        }

    }
    void updateCells()
    {
        List<Vector2Int> cellsToUpdateLater = new List<Vector2Int>();
        foreach (var c in cellsToUpdate)
        {
            CellScript cell = cells[c.y, c.x];
            if (c.y + 1 >= gridH)
                continue;
            switch (cell.type)
            {
                case CellType.sand:
                    if(moveCellBlock(c, new Vector2Int(c.x, c.y + 1)))
                    {
                        addBlocksToUpdate(c, cellsToUpdateLater);
                        cellsToUpdateLater.Add(new Vector2Int(c.x, c.y + 1));

                    }
                    else if (c.x +1<gridW && moveCellBlock(c, new Vector2Int(c.x + 1, c.y + 1)))
                    {
                        addBlocksToUpdate(c, cellsToUpdateLater);
                        cellsToUpdateLater.Add(new Vector2Int(c.x + 1, c.y + 1));
                    }
                    else if (c.x - 1>=0 && moveCellBlock(c, new Vector2Int(c.x - 1, c.y + 1)))
                    {
                        addBlocksToUpdate(c, cellsToUpdateLater);
                        cellsToUpdateLater.Add(new Vector2Int(c.x - 1, c.y + 1));
                    }
                    break;
            }
        }
        cellsToUpdate.Clear();
        cellsToUpdate = null;
        cellsToUpdate = cellsToUpdateLater;
    }
    void spawnBlockAtMap()
    {
        activateBlockCells();
        addCellStoUpdateFromBlock();
        b = null;
    }
    bool checkCollisionBlock()
    {

        int x = b.x;
        int y = b.y;
        if (y+b.h>=199)
        {
            return true;
        }
        for (int i = 0; i < b.h; i++)
            for (int j = 0; j < b.w; j++)
            {
                if (!b.hasBlock(j, i))
                    continue;
                if (cells[y + i, x + j].isEmpty)
                    continue;
                return true;
                
            }
        return false;
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

    void addCellStoUpdateFromBlock()
    {
        for (int i = 0; i < b.h; i++)
            for (int j = 0; j < b.w; j++)
            {
                if (!b.hasBlock(j, i))
                    continue;
                int x = j + b.x;
                int y = i + b.y;
                cellsToUpdate.Add(new Vector2Int(x, y));
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

        for (int y = 0; y < gridH; y++)
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
            }
        }
        
    }
}
