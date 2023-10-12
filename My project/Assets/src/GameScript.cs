using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class GameScript : MonoBehaviour
{
    private static int gridW = 100, gridH = 200;
    public CellScript cellScript;
    public float ratio = 0.9f;
    CellScript[,] cells = new CellScript[gridH, gridW];
    Block b = null;
    List<Vector2Int> cellsToUpdate= new List<Vector2Int>();
    float timer=0.0f;
    List<Vector2Int> cellsToCheck = new List<Vector2Int>();
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
        int sandSpeed = 36;
        if ((int)(timer * sandSpeed) > 0) 
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
            while(cellsToCheck.Count>0)
                checkBlockToRemove();
            activateBlockCells();
        }
    }
    CellType getTypeAt(int x, int y)
    {
        return cells[y, x].type;
    }

    bool checkBlockTypeIfNotAtList(Vector2Int p,CellType type,List<Vector2Int> l)
    {
        return !cells[p.y, p.x].isEmpty && getTypeAt(p.x, p.y) == type && !l.Contains(p);
    }

    void checkBlockToRemove()
    {
        Vector2Int first = this.cellsToCheck.First();
        this.cellsToCheck.Remove(first);
        if (cells[first.y, first.x].isEmpty)
        {
            return;
        }

        CellType type = cells[first.y, first.x].type;
        List<Vector2Int> cellsToRemove = new List<Vector2Int>();
        List<Vector2Int> cellsToCheck = new List<Vector2Int> { first };
        int minX = first.x;
        int maxX = first.x;
        while(cellsToCheck.Count>0)
        {
            Vector2Int p=cellsToCheck.First();
            Vector2Int tmp;
            cellsToCheck.RemoveAt(0);
            if (p.x - 1 >= 0) 
            {
                tmp = new Vector2Int(p.x - 1, p.y);
                if (checkBlockTypeIfNotAtList(tmp, type, cellsToRemove)) 
                {
                    cellsToRemove.Add(tmp);
                    cellsToCheck.Add(tmp);
                    this.cellsToCheck.Remove(tmp);
                    if (minX > tmp.x) 
                        minX = tmp.x;
                }
            }
            if (p.y - 1 >= 0)
            {
                tmp = new Vector2Int(p.x, p.y - 1);
                if (checkBlockTypeIfNotAtList(tmp, type, cellsToRemove))
                {
                    cellsToRemove.Add(tmp);
                    cellsToCheck.Add(tmp);
                    this.cellsToCheck.Remove(tmp);
                }
            }
            if (p.x + 1 < gridW)
            {
                tmp = new Vector2Int(p.x + 1, p.y);
                if (checkBlockTypeIfNotAtList(tmp, type, cellsToRemove))
                {
                    cellsToRemove.Add(tmp);
                    cellsToCheck.Add(tmp);
                    this.cellsToCheck.Remove(tmp);
                    if (maxX < tmp.x)
                        maxX = tmp.x;
                }
            }
            if (p.y + 1 < gridH)
            {
                tmp = new Vector2Int(p.x, p.y + 1);
                if (checkBlockTypeIfNotAtList(tmp, type, cellsToRemove))
                {
                    cellsToRemove.Add(tmp);
                    cellsToCheck.Add(tmp);
                    this.cellsToCheck.Remove(tmp);
                }
            }

        }

        if (minX<=0 && maxX>=gridW-1)
        {
            foreach (var cell in cellsToRemove)
            {
                cells[cell.y, cell.x].disactivateCell();
                addBlocksToUpdate(cell);
            }

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

    void addToUpdate(Vector2Int p)
    {
        if (!cellsToUpdate.Contains(p))
            cellsToUpdate.Add(p);
    }
    void addBlocksToUpdate(Vector2Int pos)
    {
        if (pos.y - 1 >= 0 && pos.y - 1 < gridH && pos.x>=0 && pos.x<gridH) 
        {
            if (pos.x - 1 >= 0 && !cells[pos.y - 1, pos.x - 1].isEmpty)
            {
                addToUpdate(new Vector2Int(pos.x - 1, pos.y - 1));
            }
            if (pos.x + 1 < gridW && !cells[pos.y - 1, pos.x + 1].isEmpty)
            {
                addToUpdate(new Vector2Int(pos.x + 1, pos.y - 1));
            }
            if (!cells[pos.y - 1, pos.x].isEmpty)
            {
                addToUpdate(new Vector2Int(pos.x, pos.y - 1));
            }

        }

    }

    void sandUpdate(Vector2Int c)
    {
        if (moveCellBlock(c, new Vector2Int(c.x, c.y + 1)))
        {
            addBlocksToUpdate(c);
            Vector2Int p = new Vector2Int(c.x, c.y + 1);
            addToUpdate(p);
            cellsToCheck.Add(p);
            return;
        }
        if (isFreeSpaceIn(new Vector2Int(c.x - 1, c.y + 1)) &&
            isFreeSpaceIn(new Vector2Int(c.x + 1, c.y + 1)))
        {
            if (Random.Range(0, 1) == 0)
            {
                Vector2Int p = new Vector2Int(c.x + 1, c.y + 1);
                addToUpdate(p);
                moveCellBlock(c, p);
                cellsToCheck.Add(p);

            }
            else
            {
                Vector2Int p = new Vector2Int(c.x - 1, c.y + 1);
                addToUpdate(p);
                moveCellBlock(c, p);
                cellsToCheck.Add(p);
            }
            addBlocksToUpdate(c);
            return;
        }
        if (isFreeSpaceIn(new Vector2Int(c.x - 1, c.y + 1)))
        {
            Vector2Int p = new Vector2Int(c.x - 1, c.y + 1);
            addToUpdate(p);
            moveCellBlock(c, p);
            cellsToCheck.Add(p);
            addBlocksToUpdate(c);
            return;
        }
        if (isFreeSpaceIn(new Vector2Int(c.x + 1, c.y + 1)))
        {
            Vector2Int p = new Vector2Int(c.x + 1, c.y + 1);
            addToUpdate(p);
            moveCellBlock(c, p);
            cellsToCheck.Add(p);
            addBlocksToUpdate(c);
            return;
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
        if (y+b.h>=200)
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
