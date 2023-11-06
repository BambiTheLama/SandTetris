using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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

    // Update is called once per frame
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
    /// Dodaje kom�rki do aktualizacji na podstawie aktywowanego bloku.
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
    /// Umieszcza aktywowany blok na planszy i przygotowuje do aktualizacji kom�rek.
    /// </summary>
    void SpawnBlockAtMap()
    {
        ActivateBlockCells();
        AddCellStoUpdateFromBlock();
        block = null;
    }
    /// <summary>
    /// Umieszcza blok na planszy, aktywuj�c kom�rki w jego obszarze.
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
    /// Deaktywuje kom�rki bloku na planszy.
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
    /// Aktualizuje stan kom�rek na planszy, np. spadaj�cy piasek.
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
            else if (cell.Type == CellType.Steam)
                WoodUpdate(c);

        }
        cellsToUpdate.Clear();
    }

    /// <summary>
    /// Usuwa kom�rki ognia.
    /// </summary>
    /// <param name="p">Pozycja kom�rki ognia do usuni�cia.</param>
    void ClearFire(Vector2Int p)
    {
        if (cells[p.y, p.x].Type != CellType.Fire)
            return;
        cells[p.y, p.x].DeactivateCell();
    }

    /// <summary>
    /// Aktualizuje kom�rki wody.
    /// </summary>
    /// <param name="c">Pozycja kom�rki wody do aktualizacji.</param>
    void WaterUpdate(Vector2Int c)
    {
        Vector2Int p;
        AddToUpdate(c);
        int dir = 0;
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
    /// Aktualizuje kom�rki drewna.
    /// </summary>
    /// <param name="c">Pozycja kom�rki drewna do aktualizacji.</param>
    void WoodUpdate(Vector2Int c)
    {
        if (c.y + 1 >= gridHeight)
            return;
        if (cells[c.y + 1, c.x].IsEmpty || cells[c.y + 1, c.x].Type == CellType.SandYellow)
        {
            CellScript c1 = cells[c.y, c.x];
            CellScript c2 = cells[c.y + 1, c.x];
            c2.DeactivateCell();
            c2.SetCellValue(c1.Type, c1.Color, c1.timer);
            c1.DeactivateCell();
            AddToUpdate(new Vector2Int(c.x, c.y + 1));
        }
        else if(cells[c.y + 1, c.x].Type != CellType.Wood)
        {
            CellScript c1 = cells[c.y, c.x];
            CellScript c2 = cells[c.y + 1, c.x];
            CellType type = c1.Type;
            Color color = c1.Color;
            int timer = c1.timer;
            c1.DeactivateCell();
            c1.SetCellValue(c2.Type, c2.Color, c2.timer);
            c2.DeactivateCell();
            c2.SetCellValue(type, color, timer);
            if (!cells[c.y, c.x].IsEmpty)
                AddToUpdate(new Vector2Int(c.x, c.y));
            AddToUpdate(new Vector2Int(c.x, c.y + 1));
            AddBlocksToUpdate(c);
        }
    }

    /// <summary>
    /// Aktywuje kom�rki ognia.
    /// </summary>
    /// <param name="p">Pozycja kom�rki ognia.</param>
    void FireBlock(Vector2Int p)
    {
        if (cells[p.y, p.x].Type != CellType.Wood)
            return;
        cells[p.y, p.x].DeactivateCell();
        cells[p.y, p.x].SetCellValue(CellType.Fire, Color.red);
        AddToUpdate(p);
    }

    /// <summary>
    /// Aktualizuje kom�rki ognia.
    /// </summary>
    /// <param name="c">Pozycja kom�rki ognia do aktualizacji.</param>
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
            cells[c.y, c.x].updateTimer();
            if (cells[c.y, c.x].IsEmpty)
                AddBlocksToUpdate(c);
            else
                AddToUpdate(c);
        }




    }

    /// <summary>
    /// Usuwa cz�steczki wody.
    /// </summary>
    /// <param name="p">Pozycja kom�rki wody do usuni�cia.</param>
    bool DisapearWater(Vector2Int p)
    {
        if (cells[p.y, p.x].Type != CellType.Water)
            return false;
        cells[p.y, p.x].DeactivateCell();
        return true;
    }

    /// <summary>
    /// Aktualizuje kom�rki piasku (funkcja zwi�zana z mechanik� piasku).
    /// </summary>
    /// <param name="c">Pozycja kom�rki piasku do aktualizacji.</param>
    void SandUpdate(Vector2Int c)
    {
        Vector2Int p;
        int dir = 0;
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
    /// Przenosi kom�rk� z pozycji aktywnej na pozycj� nieaktywn�, je�li jest to mo�liwe.
    /// </summary>
    /// <param name="active">Pozycja aktywnej kom�rki.</param>
    /// <param name="nonactive">Pozycja nieaktywnej kom�rki.</param>
    /// <returns>Zwraca true, je�li przeniesienie by�o mo�liwe, w przeciwnym razie false.</returns>
    bool MoveCellBlock(Vector2Int active, Vector2Int nonactive)
    {
        if (cells[active.y, active.x].IsEmpty)
            return false;
        if (!cells[nonactive.y, nonactive.x].IsEmpty)
            return false;
        CellScript a = cells[active.y, active.x];
        cells[nonactive.y, nonactive.x].SetCellValue(a.Type, a.Color, a.timer);
        a.DeactivateCell();
        return true;
    }
    /// <summary>
    /// Dodaje kom�rki powy�ej danej pozycji do listy kom�rek do aktualizacji (wersja uproszczona).
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
    /// Dodaje kom�rki po lewej stronie danej pozycji do listy kom�rek do aktualizacji.
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
    /// Dodaje kom�rki po prawej stronie danej pozycji do listy kom�rek do aktualizacji.
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
    /// <returns>Zwraca true, je�li pozycja jest wolna, w przeciwnym razie false.</returns>
    bool IsFreeSpaceIn(Vector2Int p1)
    {
        if (p1.y < 0 || p1.y >= gridHeight || p1.x < 0 || p1.x >= gridWidth)
            return false;
        return cells[p1.y, p1.x].IsEmpty;
    }
    /// <summary>
    /// Dodaje kom�rki powy�ej danej pozycji do listy kom�rek do aktualizacji.
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
    /// Dodaje dan� pozycj� do listy kom�rek do aktualizacji.
    /// </summary>
    /// <param name="p">Pozycja kom�rki do aktualizacji.</param>
    void AddToUpdate(Vector2Int p)
    {
        if (!cellsToUpdate.Contains(p))
            cellsToUpdate.Add(p);
    }
    /// <summary>
    /// Sprawdza kolizje aktywowanego bloku z istniej�cymi kom�rkami na planszy.
    /// </summary>
    /// <returns>Zwraca true, je�li wyst�pi�a kolizja, w przeciwnym razie false.</returns>
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
    /// Generuje plansz� gry w postaci kom�rek.
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
    /// Ustawia kolor wszystkich kom�rek na planszy.
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
