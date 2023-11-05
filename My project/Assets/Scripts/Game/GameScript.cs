using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// G��wny skrypt zarz�dzaj�cy gr� i plansz�.
/// </summary>
public class GameScript : MonoBehaviour
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
    List<Vector2Int> cellsToRemove = new();
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
        statsController.ResetTimer();
        statsController.StartTimer();


    }

    /// <summary>
    /// Tworzy nowy blok do gry.
    /// </summary>
    void NewBlock()
    {
        int sizeofCellType = 4;
        int sizeofBlockType = (int)BlockType.size;
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
                MainTheme.Stop();
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
                if(block.X + block.Width < 80){ 
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

    /// <summary>
    /// Pobiera typ kom�rki na danej pozycji.
    /// </summary>
    /// <param name="x">Wsp�rz�dna X kom�rki.</param>
    /// <param name="y">Wsp�rz�dna Y kom�rki.</param>
    /// <returns>Typ kom�rki na danej pozycji.</returns>
    CellType GetTypeAt(int x, int y)
    {
        return cells[y, x].Type;
    }

    /// <summary>
    /// Sprawdza, czy kom�rka o okre�lonym po�o�eniu i typie nie znajduje si� na li�cie kom�rek do usuni�cia.
    /// </summary>
    /// <param name="p">Po�o�enie kom�rki do sprawdzenia.</param>
    /// <param name="type">Typ kom�rki do sprawdzenia.</param>
    /// <param name="toRemove">Lista kom�rek do usuni�cia.</param>
    /// <returns>Zwraca true, je�li kom�rka nie znajduje si� na li�cie do usuni�cia, w przeciwnym razie false.</returns>
    bool CheckBlockTypeIfNotAtList(Vector2Int p, CellType type, List<Vector2Int> toRemove)
    {
        return !cells[p.y, p.x].IsEmpty && GetTypeAt(p.x, p.y) == type && !toRemove.Contains(p);
    }

    /// <summary>
    /// Usuwa kom�rki z planszy z listy kom�rek do usuni�cia i przygotowuje je do aktualizacji.
    /// </summary>
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
    
    /// <summary>
    /// Sortuje list� kom�rek do usuni�cia na podstawie wsp�rz�dnych x.
    /// </summary>
    void SortRemoveList()
    {
            cellsToRemove = cellsToRemove.OrderBy(cell => cell.x).ToList();
    }

    /// <summary>
    /// Sprawdza, kt�re kom�rki na planszy wymagaj� usuni�cia na podstawie punktu kontrolnego i ich s�siedztwa.
    /// </summary>
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

    /// <summary>
    /// Sprawdza i dodaje kom�rk� do listy kom�rek do usuni�cia oraz aktualizacji, je�li spe�nia okre�lone kryteria.
    /// </summary>
    /// <param name="cellsToRemove">Lista kom�rek do usuni�cia.</param>
    /// <param name="cellsToCheck">Lista kom�rek do sprawdzenia.</param>
    /// <param name="minX">Minimalna wsp�rz�dna x kom�rek do usuni�cia.</param>
    /// <param name="maxX">Maksymalna wsp�rz�dna x kom�rek do usuni�cia.</param>
    /// <param name="tmp">Po�o�enie kom�rki do sprawdzenia.</param>
    /// <param name="type">Oczekiwany typ kom�rki.</param>
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
        cells[nonactive.y, nonactive.x].SetCellValue(a.Type, a.Color);
        a.DeactivateCell();
        return true;
    }
    
    /// <summary>
    /// Dodaje dan� pozycj� do listy kom�rek do aktualizacji.
    /// </summary>
    /// <param name="p">Pozycja kom�rki do aktualizacji.</param>
    void AddToUpdate(Vector2Int p)
    {
        if (!cellsToUpdate.Contains(p) && !cellsToRemove.Contains(p))
            cellsToUpdate.Add(p);
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
        }
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
        if (MoveCellBlock(c, p = new Vector2Int(c.x, c.y + 1)))
        {
            AddBlocksToUpdate(c);
        }
        else if (IsFreeSpaceIn(p = new Vector2Int(c.x - dir, c.y + 1)) && MoveCellBlock(c, p))
        {
            AddBlocksToUpdateLeft(c);
        }
        else if (IsFreeSpaceIn(p = new Vector2Int(c.x + dir, c.y + 1)) && MoveCellBlock(c, p))
        {
            AddBlocksToUpdateRight(c);
        }
        else
            return;
        AddToUpdate(p);
        cellsToCheck.Add(p);
    }

    /// <summary>
    /// Aktualizuje stan kom�rek na planszy, np. spadaj�cy piasek.
    /// </summary>
    void UpdateCells()
    {
        List<Vector2Int> cellsToUpdate = this.cellsToUpdate.OrderBy(cell => - cell.y).ToList();
        this.cellsToUpdate = new List<Vector2Int>();
        foreach (var c in cellsToUpdate)
        {
            CellScript cell = cells[c.y, c.x];
            if (c.y + 1 >= gridHeight)
                continue;

            SandUpdate(c);

        }
        cellsToUpdate.Clear();
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
    /// Sprawdza kolizje aktywowanego bloku z istniej�cymi kom�rkami na planszy.
    /// </summary>
    /// <returns>Zwraca true, je�li wyst�pi�a kolizja, w przeciwnym razie false.</returns>
    bool CheckCollisionBlock()
    {

        int x = block.X;
        int y = block.Y;
        if (y+block.Height >=gridHeight)
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
                cells[y,x].SetCellValue(block.CellType, block.GetColor(j,i));
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


    /// <summary>
    /// Rozpoczyna gr� od nowa.
    /// </summary>
    public void RestartGame()
    {
        DestroyBlocksAndCells();
        EndGame = false;
        MainTheme.Play();
        statsController.ResetTimer();
        statsController.StartTimer();

        GenerateGrid();
        SetColor();
        NewBlock();
    }


    /// <summary>
    /// Usuwa bloki i kom�rki z planszy.
    /// </summary>
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
