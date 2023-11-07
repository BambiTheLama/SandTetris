using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Główny skrypt zarządzający grą i planszą.
/// </summary>
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
    float timer = 0.0f;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Time.timeScale = 1;
        GenerateGrid();
        SetColor();
        statsController.ResetTimer();
        statsController.StartTimer();

    }

    void Update()
    {
        if(EndGame)
        {
            audioSource.clip = loseAudio;
            if (!lostGameMusic)
            {
                MainTheme.Stop();
                audioSource.Play();
                lostGameMusic = true;
            }

            statsController.StopTimer();
            return;
        }
        timer += Time.deltaTime;
        if(timer>=0.03f)
        {
            timer-=0.03f;
            UpdateCells();
        }

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
                    audioSource.clip = moveAudio;
                    audioSource.Play();
                }
            }

            if (Input.GetKeyDown(KeyCode.D))// && !pauseController.paused)
            {
                block.MoveRight();
                if (block.X + block.Width < 80)
                {
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
    /// Umieszcza blok na planszy, aktywując komórki w jego obszarze.
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
    /// Aktualizuje stan komórek na planszy, np. spadający piasek.
    /// </summary>
    void UpdateCells()
    {
        List<Vector2Int> cellsToUpdate = this.cellsToUpdate.OrderBy(cell => -cell.y).ToList();
        this.cellsToUpdate = new List<Vector2Int>();
        foreach (var c in cellsToUpdate)
        {
            CellScript cell = cells[c.y, c.x];
            if (c.y >= gridHeight || c.y < 0 || c.x < 0 || c.x >= gridWidth) 
                continue;
            if(cell.IsEmpty) 
                continue;
            if (cell.Type <= CellType.SandYellow)
                SandUpdate(c);
            else if (cell.Type == CellType.Fire)
                FireUpdate(c);
            else if (cell.Type == CellType.Water)
                WaterUpdate(c);
            else if (cell.Type == CellType.Wood)
                WoodUpdate(c);
            //else if (cell.Type == CellType.Steam) 
            //    ;


        }
        cellsToUpdate.Clear();
    }

    /// <summary>
    /// Usuwa komórki ognia.
    /// </summary>
    /// <param name="p">Pozycja komórki ognia do usunięcia.</param>
    void ClearFire(Vector2Int p)
    {
        if (cells[p.y, p.x].Type != CellType.Fire)
            return;
        cells[p.y, p.x].DeactivateCell();
    }

    /// <summary>
    /// Aktualizuje komórki wody.
    /// </summary>
    /// <param name="c">Pozycja komórki wody do aktualizacji.</param>
    void WaterUpdate(Vector2Int c)
    {
        Vector2Int p;
        AddToUpdate(c);
        int dir;
        if (Random.Range(0, 2) == 0) 
            dir = -1;
        else
            dir = 1;
        if (c.y + 1 < gridHeight)
            ClearFire(new Vector2Int(c.x, c.y + 1));
        if (c.y - 1 >= 0)
            ClearFire(new Vector2Int(c.x, c.y - 1));
        if (c.x + 1 < gridWidth) 
            ClearFire(new Vector2Int(c.x + 1, c.y));
        if (c.x - 1 >= 0)
            ClearFire(new Vector2Int(c.x - 1, c.y));
        if (c.y + 1 < gridHeight && MoveCellBlock(c, p = new Vector2Int(c.x, c.y + 1)))
        {
            AddBlocksToUpdate(c);
        }
        else if (c.x - dir < gridWidth && c.x - dir >= 0 && c.y + 1<gridHeight &&
            IsFreeSpaceIn(p = new Vector2Int(c.x - dir, c.y + 1)) && MoveCellBlock(c, p))
        {
            AddBlocksToUpdateLeft(c);
        }
        else if (c.x - dir < gridWidth && c.x - dir >= 0 && 
            IsFreeSpaceIn(p = new Vector2Int(c.x - dir, c.y)) && MoveCellBlock(c, p))
        {
            AddBlocksToUpdateLeft(c);
        }
        else if (c.x + dir < gridWidth && c.x + dir >= 0 && c.y + 1 < gridHeight &&
            IsFreeSpaceIn(p = new Vector2Int(c.x + dir, c.y + 1)) && MoveCellBlock(c, p))
        {
            AddBlocksToUpdateRight(c);
        }
        else if (c.x + dir < gridWidth && c.x + dir >= 0 &&
            IsFreeSpaceIn(p = new Vector2Int(c.x + dir, c.y)) && MoveCellBlock(c, p))
        {
            AddBlocksToUpdateRight(c);
        }
        else
            return;
        AddToUpdate(p);
        cellsToCheck.Add(p);
    }


    /// <summary>
    /// Aktualizuje komórki drewna.
    /// </summary>
    /// <param name="c">Pozycja komórki drewna do aktualizacji.</param>
    void WoodUpdate(Vector2Int c)
    {
        if (c.y + 1 >= gridHeight)
            return;
        if (cells[c.y + 1, c.x].IsEmpty || cells[c.y + 1, c.x].Type <= CellType.SandYellow || cells[c.y + 1, c.x].Type == CellType.Glass)
        {
            CellScript c1 = cells[c.y, c.x];
            CellScript c2 = cells[c.y + 1, c.x];
            c2.DeactivateCell();
            c2.SetCellValue(c1.Type, c1.Color, c1.Timer);
            c1.DeactivateCell();
            AddToUpdate(new Vector2Int(c.x, c.y + 1));
        }
        else if(cells[c.y + 1, c.x].Type != CellType.Wood)
        {
            CellScript c1 = cells[c.y, c.x];
            CellScript c2 = cells[c.y + 1, c.x];
            CellType type = c1.Type;
            Color color = c1.Color;
            int timer = c1.Timer;
            c1.DeactivateCell();
            c1.SetCellValue(c2.Type, c2.Color, c2.Timer);
            c2.DeactivateCell();
            c2.SetCellValue(type, color, timer);
            if (!cells[c.y, c.x].IsEmpty)
                AddToUpdate(new Vector2Int(c.x, c.y));
            AddToUpdate(new Vector2Int(c.x, c.y + 1));
            AddBlocksToUpdate(c);
        }
    }

    /// <summary>
    /// Aktywuje komórki ognia.
    /// </summary>
    /// <param name="p">Pozycja komórki ognia.</param>
    void FireBlock(Vector2Int p)
    {
        if (cells[p.y, p.x].Type == CellType.Wood)
        {
            cells[p.y, p.x].DeactivateCell();
            cells[p.y, p.x].SetCellValue(CellType.Fire, Color.red);

        }
        else if (cells[p.y, p.x].Type <= CellType.SandYellow)
        {
            cells[p.y, p.x].DeactivateCell();
            cells[p.y, p.x].SetCellValue(CellType.Glass, Color.white);
        }
        else 
            return;
        AddToUpdate(p);

    }

    /// <summary>
    /// Aktualizuje komórki ognia.
    /// </summary>
    /// <param name="c">Pozycja komórki ognia do aktualizacji.</param>
    void FireUpdate(Vector2Int c)
    {
        Vector2Int p;
        p = new Vector2Int(c.x, c.y - 1);
        if (p.y >= 0) 
        {
            FireBlock(p);
        }
        p = new Vector2Int(c.x, c.y + 1);
        if (p.y < gridHeight)
        {
            FireBlock(p);
        }
        p = new Vector2Int(c.x - 1, c.y);
        if (p.x >= 0)
        {
            FireBlock(p);
        }
        p = new Vector2Int(c.x + 1, c.y);
        if (p.x < gridWidth)
        {
            FireBlock(p);
        }

        if (c.y + 1 < gridHeight && MoveCellBlock(c, new Vector2Int(c.x, c.y + 1))) 
        {
            AddToUpdate(new Vector2Int(c.x, c.y + 1));
            AddBlocksToUpdate(c);
        }
        else if (!cells[c.y, c.x].IsEmpty)
        {
            cells[c.y, c.x].UpdateTimer();
            if (cells[c.y, c.x].IsEmpty)
                AddBlocksToUpdate(c);
            else
                AddToUpdate(c);
        }




    }

    /// <summary>
    /// Usuwa cząsteczki wody.
    /// </summary>
    /// <param name="p">Pozycja komórki wody do usunięcia.</param>
    bool DisapearWater(Vector2Int p)
    {
        if (cells[p.y, p.x].Type != CellType.Water)
            return false;
        cells[p.y, p.x].DeactivateCell();
        return true;
    }

    /// <summary>
    /// Aktualizuje komórki piasku (funkcja związana z mechaniką piasku).
    /// </summary>
    /// <param name="c">Pozycja komórki piasku do aktualizacji.</param>
    void SandUpdate(Vector2Int c)
    {
        Vector2Int p;
        int dir;
        if (Random.Range(0, 2) == 0)
            dir = -1;
        else
            dir = 1;
        if (c.y + 1 >= gridHeight)
            return;

        if (DisapearWater(new Vector2Int(c.x, c.y + 1)))
        {

        }
        else if (c.x - dir >= 0 && c.x - dir < gridHeight && DisapearWater(new Vector2Int(c.x - dir, c.y + 1)))
        {

        }
        else if (c.x + dir >= 0 && c.x + dir < gridHeight && DisapearWater(new Vector2Int(c.x + dir, c.y + 1)))
        {

        }

        if (MoveCellBlock(c, p = new Vector2Int(c.x, c.y + 1)))
        {
            AddBlocksToUpdate(c);
        }
        else if (c.x - dir >= 0 && c.x - dir < gridHeight && 
            IsFreeSpaceIn(p = new Vector2Int(c.x - dir, c.y + 1)) && MoveCellBlock(c, p))
        {
            AddBlocksToUpdateLeft(c);
        }
        else if (c.x + dir >= 0 && c.x + dir < gridHeight && 
            IsFreeSpaceIn(p = new Vector2Int(c.x + dir, c.y + 1)) && MoveCellBlock(c, p))
        {
            AddBlocksToUpdateRight(c);
        }
        else
            return;
        AddToUpdate(p);
        cellsToCheck.Add(p);
    }
    /// <summary>
    /// Przenosi komórkę z pozycji aktywnej na pozycję nieaktywną, jeśli jest to możliwe.
    /// </summary>
    /// <param name="active">Pozycja aktywnej komórki.</param>
    /// <param name="nonactive">Pozycja nieaktywnej komórki.</param>
    /// <returns>Zwraca true, jeśli przeniesienie było możliwe, w przeciwnym razie false.</returns>
    bool MoveCellBlock(Vector2Int active, Vector2Int nonactive)
    {
        if (cells[active.y, active.x].IsEmpty)
            return false;
        if (!cells[nonactive.y, nonactive.x].IsEmpty)
            return false;
        CellScript a = cells[active.y, active.x];
        cells[nonactive.y, nonactive.x].SetCellValue(a.Type, a.Color, a.Timer);
        a.DeactivateCell();
        return true;
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
            if (pos.x - 1 >= 0 && !cells[pos.y, pos.x - 1].IsEmpty)
            {
                AddToUpdate(new Vector2Int(pos.x - 1, pos.y));
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
            if (pos.x + 1 < gridWidth && !cells[pos.y, pos.x + 1].IsEmpty)
            {
                AddToUpdate(new Vector2Int(pos.x + 1, pos.y));
            }
        }

    }
    /// <summary>
    /// Sprawdza, czy dana pozycja jest wolna na planszy.
    /// </summary>
    /// <param name="p1">Pozycja do sprawdzenia.</param>
    /// <returns>Zwraca true, jeśli pozycja jest wolna, w przeciwnym razie false.</returns>
    bool IsFreeSpaceIn(Vector2Int p1)
    {
        if (p1.y < 0 || p1.y >= gridHeight || p1.x < 0 || p1.x >= gridWidth)
            return false;
        return cells[p1.y, p1.x].IsEmpty;
    }
    /// <summary>
    /// Dodaje komórki powyżej danej pozycji do listy komórek do aktualizacji.
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
    /// Dodaje daną pozycję do listy komórek do aktualizacji.
    /// </summary>
    /// <param name="p">Pozycja komórki do aktualizacji.</param>
    void AddToUpdate(Vector2Int p)
    {
        if (!cellsToUpdate.Contains(p))
            cellsToUpdate.Add(p);
    }
    /// <summary>
    /// Sprawdza kolizje aktywowanego bloku z istniejącymi komórkami na planszy.
    /// </summary>
    /// <returns>Zwraca true, jeśli wystąpiła kolizja, w przeciwnym razie false.</returns>
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
    /// Generuje planszę gry w postaci komórek.
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

    /// <summary>
    /// Rozpoczyna grę od nowa.
    /// </summary>
    public void RestartGame()
    {
        EndGame = false;
        MainTheme.Play();
        statsController.ResetTimer();
        statsController.StartTimer();

        SetColor();
        NewBlock();
    }
}
