using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class GameScript : MonoBehaviour
{
    private static int gridHeight = 200, gridWidth = 100;
    public CellScript cellScript;
    public float ratio = 0.9f;
    CellScript[,] cells = new CellScript[gridHeight, gridWidth];
    Block block = null;
    List<Vector2Int> cellsToUpdate = new List<Vector2Int>();
    float timer = 0.0f;
    List<Vector2Int> cellsToCheck = new List<Vector2Int>();
    List<Vector2Int> cellsToRemove = new List<Vector2Int>();
    public StatsController statsController;
    public PauseController pauseController;

    void Start()
    {
        Time.timeScale = 1;
        GenerateGrid();
        SetColor();
        statsController.ResetTimer();
        statsController.StartTimer();

    }
    void NewBlock()
    {
        int sizeofCellType = sizeof(CellType);
        int sizeofBlockType = sizeof(BlockType);
        block = new Block((CellType)Random.Range(0, sizeofCellType), (BlockType)Random.Range(0, sizeofBlockType));
    }

    void Update()
    {
        timer += Time.deltaTime;
        int sandSpeed = 36;
        if ((int)(timer * sandSpeed) > 0) 
        {
            for (int i = 0; i < (int)(timer * sandSpeed); i++)
                UpdateCells();
            timer -= ((int)(timer * sandSpeed))/(float) sandSpeed;
        }
        if (block == null)
            NewBlock();
        else
        {
            Block tmp = block;
            block = new Block(tmp);
            DeactivateBlockCells();
            if (Input.GetKeyDown(KeyCode.A) && !pauseController.paused)
                block.MoveLeft();
            if (Input.GetKeyDown(KeyCode.D) && !pauseController.paused)
                block.MoveRight();
            if (Input.GetKeyDown(KeyCode.W))
                block.RotateBlock();
            if (Input.GetKey(KeyCode.S))
                block.MoveDown();
            block.GoDown(Time.deltaTime);
            if (CheckCollisionBlock())
            {
                block = tmp;
                SpawnBlockAtMap();
                return;
            }
            while(cellsToCheck.Count>0)
                checkBlockToRemove();
            ActivateBlockCells();
            if (cellsToRemove.Count > 0)
                removeCells();
        }
    }
    CellType getTypeAt(int x, int y)
    {
        return cells[y, x].Type;
    }

    bool checkBlockTypeIfNotAtList(Vector2Int p,CellType type,List<Vector2Int> l)
    {
        return !cells[p.y, p.x].IsEmpty && getTypeAt(p.x, p.y) == type && !l.Contains(p) && !cellsToRemove.Contains(p);
    }

    void removeCells()
    {
        List<Vector2Int> removeFromList= new List<Vector2Int>();
        int n = 16;
        foreach (var cell in cellsToRemove) 
        {
            cells[cell.y, cell.x].DeactivateCell();
            AddBlocksToUpdate(cell);
            removeFromList.Add(cell);

            if (n-- <= 0)
                break;
        }
        foreach (var cell in removeFromList)
            cellsToRemove.Remove(cell); 

    }
    void sortRemoveList()
    {
        for(int j=0; j< cellsToRemove.Count; j++)
            for(int i = 0; i < cellsToRemove.Count-1; i++)
            {
                if (cellsToRemove[i].y > cellsToRemove[i + 1].y)
                {
                    Vector2Int tmp = cellsToRemove[i];
                    cellsToRemove[i] = cellsToRemove[i + 1];
                    cellsToRemove[i+1]= tmp;
                }
            }
    }
    void checkBlockToRemove()
    {
        
        Vector2Int first = this.cellsToCheck.First();
        this.cellsToCheck.Remove(first);
        if (cells[first.y, first.x].IsEmpty)
        {
            return;
        }

        CellType type = cells[first.y, first.x].Type;
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
            if (p.x + 1 < gridWidth)
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
            if (p.y + 1 < gridHeight)
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

        if (minX<=0 && maxX>=gridWidth -1)
        {
            statsController.AddPoints(200);

            foreach (var cell in cellsToRemove)
            {
                this.cellsToRemove.Add(cell);
                cells[cell.y, cell.x].setWhite();
                cellsToUpdate.Remove(cell);
            }
            sortRemoveList();

        }

    }
    bool IsFreeSpaceIn(Vector2Int p1)
    {
        if (p1.y < 0 || p1.y >= gridHeight || p1.x < 0 || p1.x >= gridWidth)
            return false;
        return cells[p1.y, p1.x].IsEmpty;
    }
    bool MoveCellBlock(Vector2Int active, Vector2Int nonactive)
    {
        if (cells[active.y, active.x].IsEmpty)
            return false;
        if (!cells[nonactive.y, nonactive.x].IsEmpty)
            return false;
        CellScript a = cells[active.y, active.x];
        cells[nonactive.y, nonactive.x].SetCellValue(a.Type, a.Color);
        a.DeactivateCell();
        return true;
    }

    void AddToUpdate(Vector2Int p)
    {
        if (!cellsToUpdate.Contains(p) && !cellsToRemove.Contains(p))
            cellsToUpdate.Add(p);
    }
    void AddBlocksToUpdate(Vector2Int pos)
    {
        if (pos.y - 1 >= 0 && pos.y - 1 < gridHeight && pos.x>=0 && pos.x<gridHeight) 
        {
            if (pos.x - 1 >= 0 && !cells[pos.y - 1, pos.x - 1].IsEmpty)
            {
                AddToUpdate(new Vector2Int(pos.x - 1, pos.y - 1));
            }
            if (pos.x + 1 < gridWidth && !cells[pos.y - 1, pos.x + 1].IsEmpty)
            {
                AddToUpdate(new Vector2Int(pos.x + 1, pos.y - 1));
            }
            if (!cells[pos.y - 1, pos.x].IsEmpty)
            {
                AddToUpdate(new Vector2Int(pos.x, pos.y - 1));
            }

        }

    }

    void sandUpdate(Vector2Int c)
    {
        if (MoveCellBlock(c, new Vector2Int(c.x, c.y + 1)))
        {
            AddBlocksToUpdate(c);
            Vector2Int p = new Vector2Int(c.x, c.y + 1);
            AddToUpdate(p);
            cellsToCheck.Add(p);
            return;
        }
        if (IsFreeSpaceIn(new Vector2Int(c.x - 1, c.y + 1)) &&
            IsFreeSpaceIn(new Vector2Int(c.x + 1, c.y + 1)))
        {
            if (Random.Range(0, 1) == 0)
            {
                Vector2Int p = new Vector2Int(c.x + 1, c.y + 1);
                AddToUpdate(p);
                MoveCellBlock(c, p);
                cellsToCheck.Add(p);

            }
            else
            {
                Vector2Int p = new Vector2Int(c.x - 1, c.y + 1);
                AddToUpdate(p);
                MoveCellBlock(c, p);
                cellsToCheck.Add(p);
            }
            AddBlocksToUpdate(c);
            return;
        }
        if (IsFreeSpaceIn(new Vector2Int(c.x - 1, c.y + 1)))
        {
            Vector2Int p = new Vector2Int(c.x - 1, c.y + 1);
            AddToUpdate(p);
            MoveCellBlock(c, p);
            cellsToCheck.Add(p);
            AddBlocksToUpdate(c);
            return;
        }
        if (IsFreeSpaceIn(new Vector2Int(c.x + 1, c.y + 1)))
        {
            Vector2Int p = new Vector2Int(c.x + 1, c.y + 1);
            AddToUpdate(p);
            MoveCellBlock(c, p);
            cellsToCheck.Add(p);
            AddBlocksToUpdate(c);
            return;
        }
    }
    void UpdateCells()
    {
        List<Vector2Int> cellsToUpdate = this.cellsToUpdate;
        this.cellsToUpdate = new List<Vector2Int>();
        foreach (var c in cellsToUpdate)
        {
            CellScript cell = cells[c.y, c.x];
            if (c.y + 1 >= gridHeight)
                continue;
            switch (cell.Type)
            {
                case CellType.SandYellow:
                case CellType.SandRed:
                case CellType.SandBlue:
                case CellType.SandGreen:
                    sandUpdate(c);

                    break;
            }
        }
        cellsToUpdate.Clear();
    }
    void SpawnBlockAtMap()
    {
        ActivateBlockCells();
        AddCellStoUpdateFromBlock();
        block = null;
    }
    bool CheckCollisionBlock()
    {

        int x = block.X;
        int y = block.Y;
        if (y+block.Height >=200)
        {
            return true;
        }
        for (int i = 0; i < block.Height; i++)
            for (int j = 0; j < block.Width; j++)
            {
                if (!block.HasBlock(j, i))
                    continue;
                if (cells[y + i, x + j].IsEmpty)
                    continue;
                return true;
                
            }
        return false;
    }
    void ActivateBlockCells()
    {
        for (int i = block.Height - 1; i >= 0; i--) 
            for (int j = 0; j < block.Width; j++) 
            {
                if (!block.HasBlock(j, i))
                    continue;
                int x = j + block.X;
                int y = i + block.Y;
                cells[y,x].SetCellValue(block.CellType, block.GetColor(j,i));
            }
    }

    void AddCellStoUpdateFromBlock()
    {
        for (int i = block.Height - 1; i >= 0; i--)
            for (int j = 0; j < block.Width; j++)
            {
                if (!block.HasBlock(j, i))
                    continue;
                int x = j + block.X;
                int y = i + block.Y;
                cellsToUpdate.Add(new Vector2Int(x, y));
            }
        
    }

    void DeactivateBlockCells()
    {
        for (int i = 0; i < block.Height; i++)
            for (int j = 0; j < block.Width; j++)
            {
                if (!block.HasBlock(j, i))
                    continue;
                int x = j + block.X;
                int y = i + block.Y;
                cells[y, x].DeactivateCell();
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
    void SetColor()
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                CellScript cell = cells[y, x];
                cell.DeactivateCell();
            }
        }
        
    }
}
