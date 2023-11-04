using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ElementalsTetrisScript : MonoBehaviour
{
    private static readonly int gridHeight = 160;
    private static readonly int gridWidth = 80;
    public CellScript cellScript;
    readonly float ratio = 0.16f;
    readonly CellScript[,] cells = new CellScript[gridHeight, gridWidth];
    Block block = null;
    Block block2 = null;
    List<Vector2Int> cellsToUpdate = new();
    readonly List<Vector2Int> cellsToCheck = new();
    public StatsController statsController;
    public PauseController pauseController;
    public AudioSource MainTheme;
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
        //statsController.ResetTimer();
        //statsController.StartTimer();


    }

    // Update is called once per frame
    void Update()
    {
        if(EndGame)
        {
            return;
        }
        UpdateCells();
        if (block==null)
        {
            NewBlock();
        }
        else
        {
            Block tmp = block;
            block = new Block(tmp);
            DeactivateBlockCells();
            if (Input.GetKeyDown(KeyCode.A))// && !pauseController.paused)
            {
                block.MoveLeft();
                if (block.X > 0)
                {
                    //audioSource.clip = moveAudio;
                    //audioSource.Play();
                }
            }

            if (Input.GetKeyDown(KeyCode.D))// && !pauseController.paused)
            {
                block.MoveRight();
                if (block.X + block.Width < 80)
                {
                    //audioSource.clip = moveAudio;
                    //audioSource.Play();
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

            ActivateBlockCells();
        }

    }
    /// <summary>
    /// Dodaje komórki do aktualizacji na podstawie aktywowanego bloku.
    /// </summary>
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
    /// <summary>
    /// Umieszcza aktywowany blok na planszy i przygotowuje do aktualizacji komórek.
    /// </summary>
    void SpawnBlockAtMap()
    {
        ActivateBlockCells();
        AddCellStoUpdateFromBlock();
        block = null;
    }
    /// <summary>
    /// Umieszcza blok na planszy, aktywuj¹c komórki w jego obszarze.
    /// </summary>
    void ActivateBlockCells()
    {
        for (int i = block.Height - 1; i >= 0; i--)
            for (int j = 0; j < block.Width; j++)
            {
                if (!block.HasBlock(j, i))
                    continue;
                int x = j + block.X;
                int y = i + block.Y;
                cells[y, x].SetCellValue(block.CellType, block.GetColor(j, i));
            }
    }
    /// <summary>
    /// Deaktywuje komórki bloku na planszy.
    /// </summary>
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

    /// <summary>
    /// Tworzy nowy blok do gry.
    /// </summary>
    void NewBlock()
    {
        const int startOfCells = 3;
        const int endOfCells = 7;
        const int sizeofBlockType = (int)BlockType.size;
        if (block2 == null)
        {
            block = new Block((CellType)Random.Range(startOfCells, endOfCells), (BlockType)Random.Range(0, sizeofBlockType));
            block2 = new Block((CellType)Random.Range(startOfCells, endOfCells), (BlockType)Random.Range(0, sizeofBlockType));

        }
        else
        {
            block = block2;
            block2 = new Block((CellType)Random.Range(startOfCells, endOfCells), (BlockType)Random.Range(0, sizeofBlockType));

        }
        if (nextBlock)
            nextBlock.SetBlockAtGrid(block2);
        EndGame = CheckCollisionBlock();
    }    
    /// <summary>
    /// Aktualizuje stan komórek na planszy, np. spadaj¹cy piasek.
    /// </summary>
    void UpdateCells()
    {
        List<Vector2Int> cellsToUpdate = this.cellsToUpdate.OrderBy(cell => -cell.y).ToList();
        this.cellsToUpdate = new List<Vector2Int>();
        foreach (var c in cellsToUpdate)
        {
            CellScript cell = cells[c.y, c.x];
            if (c.y + 1 >= gridHeight)
                continue;
            if (cell.Type <= CellType.SandYellow)
                SandUpdate(c);

        }
        cellsToUpdate.Clear();
    }
    /// <summary>
    /// Aktualizuje komórki piasku (funkcja zwi¹zana z mechanik¹ piasku).
    /// </summary>
    /// <param name="c">Pozycja komórki piasku do aktualizacji.</param>
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
    /// <summary>
    /// Przenosi komórkê z pozycji aktywnej na pozycjê nieaktywn¹, jeœli jest to mo¿liwe.
    /// </summary>
    /// <param name="active">Pozycja aktywnej komórki.</param>
    /// <param name="nonactive">Pozycja nieaktywnej komórki.</param>
    /// <returns>Zwraca true, jeœli przeniesienie by³o mo¿liwe, w przeciwnym razie false.</returns>
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
    /// <summary>
    /// Dodaje komórki powy¿ej danej pozycji do listy komórek do aktualizacji (wersja uproszczona).
    /// </summary>
    /// <param name="pos">Pozycja referencyjna.</param>
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

    /// <summary>
    /// Dodaje komórki po lewej stronie danej pozycji do listy komórek do aktualizacji.
    /// </summary>
    /// <param name="pos">Pozycja referencyjna.</param>
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

    /// <summary>
    /// Dodaje komórki po prawej stronie danej pozycji do listy komórek do aktualizacji.
    /// </summary>
    /// <param name="pos">Pozycja referencyjna.</param>
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
    /// <summary>
    /// Sprawdza, czy dana pozycja jest wolna na planszy.
    /// </summary>
    /// <param name="p1">Pozycja do sprawdzenia.</param>
    /// <returns>Zwraca true, jeœli pozycja jest wolna, w przeciwnym razie false.</returns>
    bool IsFreeSpaceIn(Vector2Int p1)
    {
        if (p1.y < 0 || p1.y >= gridHeight || p1.x < 0 || p1.x >= gridWidth)
            return false;
        return cells[p1.y, p1.x].IsEmpty;
    }
    /// <summary>
    /// Dodaje komórki powy¿ej danej pozycji do listy komórek do aktualizacji.
    /// </summary>
    /// <param name="pos">Pozycja referencyjna.</param>
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
    /// <summary>
    /// Dodaje dan¹ pozycjê do listy komórek do aktualizacji.
    /// </summary>
    /// <param name="p">Pozycja komórki do aktualizacji.</param>
    void AddToUpdate(Vector2Int p)
    {
        if (!cellsToUpdate.Contains(p))
            cellsToUpdate.Add(p);
    }
    /// <summary>
    /// Sprawdza kolizje aktywowanego bloku z istniej¹cymi komórkami na planszy.
    /// </summary>
    /// <returns>Zwraca true, jeœli wyst¹pi³a kolizja, w przeciwnym razie false.</returns>
    bool CheckCollisionBlock()
    {

        int x = block.X;
        int y = block.Y;
        if (y + block.Height >= gridHeight)
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
    /// <summary>
    /// Generuje planszê gry w postaci komórek.
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
    /// Ustawia kolor wszystkich komórek na planszy.
    /// </summary>
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
