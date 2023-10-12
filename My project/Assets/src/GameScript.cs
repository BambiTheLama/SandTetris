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
    void newBlock()
    {
        b = new Block((CellType)Random.Range(0, (int)CellType.size), (BlockType)Random.Range(0, (int)BlockType.size));
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
            newBlock();
        else
        {
            Block tmp = b;
            b = new Block(tmp);
            deactivateBlockCells();
            if (Input.GetKeyDown(KeyCode.A))
                b.moveLeft();
            if (Input.GetKeyDown(KeyCode.D))
                b.moveRight();
            if (Input.GetKeyDown(KeyCode.W))
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
    bool isFreeSpaceIn(Vector2Int p1)
    {
        if (p1.y < 0 || p1.y >= gridH || p1.x < 0 || p1.x >= gridW)
            return false;
        return cells[p1.y, p1.x].isEmpty;
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
    void addBlocksToUpdate(Vector2Int pos)
    {
        if (pos.y - 1 >= 0 && pos.y - 1 < gridH && pos.x>=0 && pos.x<gridH) 
        {
            if (pos.x - 1 >= 0)
            {
                if (!cells[pos.y - 1, pos.x - 1].isEmpty)
                    cellsToUpdate.Add(new Vector2Int(pos.x - 1, pos.y - 1));
            }
            if (pos.x + 1 < gridW)
            {
                if (!cells[pos.y - 1, pos.x + 1].isEmpty)
                    cellsToUpdate.Add(new Vector2Int(pos.x + 1, pos.y - 1));
            }
            if (!cells[pos.y - 1, pos.x].isEmpty)
                cellsToUpdate.Add(new Vector2Int(pos.x, pos.y - 1));
        }

    }
    void sandUpdate(Vector2Int c)
    {
        if (moveCellBlock(c, new Vector2Int(c.x, c.y + 1)))
        {
            addBlocksToUpdate(c);
            this.cellsToUpdate.Add(new Vector2Int(c.x, c.y + 1));

        }
        else if (isFreeSpaceIn(new Vector2Int(c.x - 1, c.y + 1)) &&
            isFreeSpaceIn(new Vector2Int(c.x + 1, c.y + 1)))
        {
            int right = Random.Range(0, 1);
            if (right == 0)
            {
                moveCellBlock(c, new Vector2Int(c.x + 1, c.y + 1));
                this.cellsToUpdate.Add(new Vector2Int(c.x + 1, c.y + 1));
            }
            else
            {
                moveCellBlock(c, new Vector2Int(c.x - 1, c.y + 1));
                this.cellsToUpdate.Add(new Vector2Int(c.x - 1, c.y + 1));
            }
            addBlocksToUpdate(c);
        }
        else if (isFreeSpaceIn(new Vector2Int(c.x - 1, c.y + 1)))
        {
            moveCellBlock(c, new Vector2Int(c.x - 1, c.y + 1));
            this.cellsToUpdate.Add(new Vector2Int(c.x - 1, c.y + 1));
            addBlocksToUpdate(c);

        }
        else if (isFreeSpaceIn(new Vector2Int(c.x + 1, c.y + 1)))
        {
            moveCellBlock(c, new Vector2Int(c.x + 1, c.y + 1));
            this.cellsToUpdate.Add(new Vector2Int(c.x + 1, c.y + 1));
            addBlocksToUpdate(c);

        }
    }
    void updateCells()
    {
        List<Vector2Int> cellsToUpdate = this.cellsToUpdate;
        this.cellsToUpdate = new List<Vector2Int>();
        foreach (var c in cellsToUpdate)
        {
            CellScript cell = cells[c.y, c.x];
            if (c.y + 1 >= gridH)
                continue;
            switch (cell.type)
            {
                case CellType.sand:
                case CellType.sand2:
                case CellType.sand3:
                case CellType.sand4:
                    sandUpdate(c);
                    break;
            }
        }
        cellsToUpdate.Clear();
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
