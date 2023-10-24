using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameScript : MonoBehaviour
{
    private static readonly int gridHeight = 200;
    private static readonly int gridWidth = 100;
    public CellScript cellScript;
    readonly float ratio = 0.16f;
    readonly CellScript[,] cells = new CellScript[gridHeight, gridWidth];
    Block block = null;
    Block block2 = null;
    List<Vector2Int> cellsToUpdate = new();
    readonly List<Vector2Int> cellsToCheck = new();
    List<Vector2Int> cellsToRemove = new();
    public StatsController statsController;
    public PauseController pauseController;
    public bool EndGame = false;
    public NextBlockScript nextBlock;

    private AudioSource audioSource;
    public AudioClip moveAudio, loseAudio, pointsAudio;
    bool lostGameMusic = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
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
        if (block2 == null) 
        {
            block = new Block((CellType)Random.Range(0, sizeofCellType), (BlockType)Random.Range(0, sizeofBlockType));
            block2 = new Block((CellType)Random.Range(0, sizeofCellType), (BlockType)Random.Range(0, sizeofBlockType));

        }
        else
        {
            block = block2;
            block2 = new Block((CellType)Random.Range(0, sizeofCellType), (BlockType)Random.Range(0, sizeofBlockType));

        }
        if (nextBlock)
            nextBlock.SetBlockAtGrid(block2);
        EndGame = CheckCollisionBlock();
    }

    void Update()
    {
        if (EndGame)
        {
            audioSource.clip = loseAudio;
            if(!lostGameMusic)
            {
                audioSource.Play();
                lostGameMusic = true;
            }

            statsController.StopTimer();
            return;
        }

        UpdateCells();
        if (block == null)
            NewBlock();
        else 
        {
            Block tmp = block;
            block = new Block(tmp);
            DeactivateBlockCells();
            if (Input.GetKeyDown(KeyCode.A) && !pauseController.paused)
            {
                block.MoveLeft();
                if (block.X > 0) 
                { 
                    audioSource.clip = moveAudio;
                    audioSource.Play();
                }
            }
                
            if (Input.GetKeyDown(KeyCode.D) && !pauseController.paused)
            {
                block.MoveRight();
                if(block.X + block.Width < 100){ 
                    audioSource.clip = moveAudio;
                    audioSource.Play();
                }
            }

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
                CheckBlockToRemove();
            ActivateBlockCells();
            if (cellsToRemove.Count > 0)
                RemoveCells();
        }
    }
    CellType GetTypeAt(int x, int y)
    {
        return cells[y, x].Type;
    }

    bool CheckBlockTypeIfNotAtList(Vector2Int p, CellType type, List<Vector2Int> toRemove)
    {
        return !cells[p.y, p.x].IsEmpty && GetTypeAt(p.x, p.y) == type && !toRemove.Contains(p);
    }

    void RemoveCells()
    {
        List<Vector2Int> removeFromList= new();
        int n = 32;
        if(cellsToRemove.Count/20>n)
            n= cellsToRemove.Count/20;
        foreach (var cell in cellsToRemove) 
        {
            cells[cell.y, cell.x].DeactivateCell();
            AddBlocksToUpdateUp(cell);
            removeFromList.Add(cell);

            if (n-- <= 0)
                break;
        }
        foreach (var cell in removeFromList)
            cellsToRemove.Remove(cell); 

    }
    void SortRemoveList()
    {
            cellsToRemove = cellsToRemove.OrderBy(cell => cell.x).ToList();
    }
    void CheckBlockToRemove()
    {
        Vector2Int first = this.cellsToCheck.First();
        this.cellsToCheck.Remove(first);

        if (cells[first.y, first.x].IsEmpty)
        {
            return;
        }

        CellType type = cells[first.y, first.x].Type;
        List<Vector2Int> cellsToRemove = new() { first };
        List<Vector2Int> cellsToCheck = new() { first };
        int minX = first.x;
        int maxX = first.x;

        while (cellsToCheck.Count > 0)
        {
            Vector2Int p = cellsToCheck.First();
            cellsToCheck.RemoveAt(0);

            if (p.x - 1 >= 0)
            {
                CheckAndAddBlockType(cellsToRemove, cellsToCheck, ref minX, ref maxX, new Vector2Int(p.x - 1, p.y), type);
                if (p.y - 1 >= 0)
                    CheckAndAddBlockType(cellsToRemove, cellsToCheck, ref minX, ref maxX, new Vector2Int(p.x - 1, p.y - 1), type);
                if (p.y + 1 < gridHeight)
                    CheckAndAddBlockType(cellsToRemove, cellsToCheck, ref minX, ref maxX, new Vector2Int(p.x - 1, p.y + 1), type);
            }
            if (p.y - 1 >= 0)
            {
                CheckAndAddBlockType(cellsToRemove, cellsToCheck, ref minX, ref maxX, new Vector2Int(p.x, p.y - 1), type);
            }
            if (p.x + 1 < gridWidth)
            {
                CheckAndAddBlockType(cellsToRemove, cellsToCheck, ref minX, ref maxX, new Vector2Int(p.x + 1, p.y), type);
                if (p.y - 1 >= 0)
                    CheckAndAddBlockType(cellsToRemove, cellsToCheck, ref minX, ref maxX, new Vector2Int(p.x + 1, p.y - 1), type);
                if (p.y + 1 < gridHeight)
                    CheckAndAddBlockType(cellsToRemove, cellsToCheck, ref minX, ref maxX, new Vector2Int(p.x + 1, p.y + 1), type);
            }
            if (p.y + 1 < gridHeight)
            {
                CheckAndAddBlockType(cellsToRemove, cellsToCheck, ref minX, ref maxX, new Vector2Int(p.x, p.y + 1), type);
            }
        }

        if (minX <= 0 && maxX >= gridWidth - 1)
        {
            statsController.AddPoints(cellsToRemove.Count);
            audioSource.clip = pointsAudio;
            audioSource.Play();

            foreach (var cell in cellsToRemove)
            {
                this.cellsToRemove.Add(cell);
                cells[cell.y, cell.x].SetWhite();
                cellsToUpdate.Remove(cell);
            }

            SortRemoveList();
        }
    }

    void CheckAndAddBlockType(List<Vector2Int> cellsToRemove, List<Vector2Int> cellsToCheck, ref int minX, ref int maxX, Vector2Int tmp, CellType type)
    {
        if (CheckBlockTypeIfNotAtList(tmp, type, cellsToRemove))
        {
            cellsToRemove.Add(tmp);
            cellsToCheck.Add(tmp);
            this.cellsToCheck.Remove(tmp);
            if (minX > tmp.x)
                minX = tmp.x;
            if (maxX < tmp.x)
                maxX = tmp.x;
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
        if (pos.y - 1 >= 0 && pos.y - 1 < gridHeight) 
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
    void AddBlocksToUpdateUp(Vector2Int pos)
    {
        if (pos.y - 1 >= 0 && pos.y - 1 < gridHeight)
        {
            if (!cells[pos.y - 1, pos.x].IsEmpty)
            {
                AddToUpdate(new Vector2Int(pos.x, pos.y - 1));
            }

        }
    }
    void AddBlocksToUpdateLeft(Vector2Int pos)
    {
        if (pos.y - 1 >= 0 && pos.y - 1 < gridHeight)
        {
            if (pos.x - 1 >= 0 && !cells[pos.y - 1, pos.x - 1].IsEmpty)
            {
                AddToUpdate(new Vector2Int(pos.x - 1, pos.y - 1));
            }
            if (!cells[pos.y - 1, pos.x].IsEmpty)
            {
                AddToUpdate(new Vector2Int(pos.x, pos.y - 1));
            }

        }
    }
    void AddBlocksToUpdateRight(Vector2Int pos)
    {
        if (pos.y - 1 >= 0 && pos.y - 1 < gridHeight)
        {
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
    void SandUpdate(Vector2Int c)
    {
        Vector2Int p;

        if (MoveCellBlock(c, p = new Vector2Int(c.x, c.y + 1)))
        {
            AddBlocksToUpdate(c);
            AddToUpdate(p);
            cellsToCheck.Add(p);
        }
        else
        {
            p = new Vector2Int(c.x - 1, c.y + 1);
            if (IsFreeSpaceIn(p))
            {
                AddToUpdate(p);
                MoveCellBlock(c, p);
                cellsToCheck.Add(p);
                AddBlocksToUpdateRight(c);
            }
            else
            {
                p = new Vector2Int(c.x + 1, c.y + 1);
                if (IsFreeSpaceIn(p))
                {
                    AddToUpdate(p);
                    MoveCellBlock(c, p);
                    cellsToCheck.Add(p);
                    AddBlocksToUpdateLeft(c);
                }
            }
        }
    }

    void UpdateCells()
    {
        List<Vector2Int> cellsToUpdate = this.cellsToUpdate.OrderBy(cell=>cell.y).ToList();
        this.cellsToUpdate = new List<Vector2Int>();
        foreach (var c in cellsToUpdate)
        {
            CellScript cell = cells[c.y, c.x];
            if (c.y + 1 >= gridHeight)
                continue;
            if (cell.Type >= CellType.SandYellow && cell.Type <= CellType.SandGreen)
            {
                SandUpdate(c);
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

    public void RestartGame()
    {
        DestroyBlocksAndCells();
        EndGame = false;
        statsController.ResetTimer();
        statsController.StartTimer();

        GenerateGrid();
        SetColor();
        NewBlock();
    }

    void DestroyBlocksAndCells()
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (cells[y, x] != null)
                {
                    Destroy(cells[y, x].gameObject);
                    cells[y, x] = null;
                }
            }
        }

        block = null;
        block2 = null;
        cellsToUpdate.Clear();
        cellsToCheck.Clear();
        cellsToRemove.Clear();
    }

}
