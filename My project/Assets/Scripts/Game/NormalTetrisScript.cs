using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Główny skrypt zarządzający grą i planszą.
/// </summary>
public class NormalTetrisScript : MonoBehaviour
{
    private static readonly int gridHeight = 20;
    private static readonly int gridWidth = 10;
    public CellNormalTetrisScript cellScript;
    readonly float ratio = 0.64f;
    readonly CellNormalTetrisScript[,] cells = new CellNormalTetrisScript[gridHeight, gridWidth];
    TetrisBlock block = null;
    TetrisBlock block2 = null;
    public NextBlockNormalTetrisScript nextBlock;
    public StatsController statsController;
    public PauseController pauseController;
    public LoseController loseController;
    public bool EndGame = false;
    public AudioSource MainTheme;
    private AudioSource audioSource;
    public AudioClip moveAudio, loseAudio, pointsAudio;
    bool lostGameMusic = false;
    readonly List<int> checkLines = new();
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Time.timeScale = 1;
        GenerateGrid(); 
        statsController.ResetTimer();
        statsController.StartTimer();
    }

    void Update()
    {
        if (EndGame)
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
        if (block == null)
        {
            NewBlock();
        }
        else
        {
            TetrisBlock tmp=block;
            block = new TetrisBlock(tmp);
            DeactivateBlockCells();
            if (Input.GetKey(KeyCode.S))
                block.MoveDown();
            if (Input.GetKeyDown(KeyCode.W) && CanBlockBeRotated())
                block.RotateBlock();
            if (Input.GetKeyDown(KeyCode.A) && !pauseController.paused && CanBlockBeMove(-1))
            {
                block.MoveLeft();
                audioSource.clip = moveAudio;
                audioSource.Play();
            }


            if (Input.GetKeyDown(KeyCode.D) && !pauseController.paused && CanBlockBeMove(1))
            {
                block.MoveRight();
                audioSource.clip = moveAudio;
                audioSource.Play();
            }

            block.GoDown(Time.deltaTime);
            if (CheckCollisionBlock())
            {
                block = tmp;
                ActivateBlockCells();
                for (int i = 0; i < block.Height; i++)
                    checkLines.Add(block.Y + i);

                block = null;
                return;
            }
            if(checkLines.Count > 0)
            {
                foreach (var line in checkLines)
                {
                    bool allFull = true;
                    for (int i = 0; i < gridWidth; i++)
                        if (cells[line, i].Type == 0) 
                        {
                            allFull = false;
                            break;
                        }
                    if(allFull)
                    {
                        RemoveLine(line);
                        statsController.AddPoints(1000);
                        audioSource.clip = pointsAudio;
                        audioSource.Play();
                    }

                }
                checkLines.Clear();


            }
            ActivateBlockCells();
        }
    }

    /// <summary>
    /// Sprawdza czy blok może zostać poruszony na boki bezkolizyjnie.
    /// </summary>
    bool CanBlockBeMove(int n)
    {
        int x = block.X+n;
        int y = block.Y;
        if (x < 0 || x + block.Width > gridWidth)
            return false;
        if (y < 0 || y + block.Height > gridHeight)
            return false;
        for (int i = 0; i < block.Width; i++)
            for (int j = 0; j < block.Height; j++)
                if (cells[j + y, i + x].Type > 0 && block.HasBlock(i,j))
                    return false;

        return true;
    }

    /// <summary>
    /// Sprawdza czy blok może zostać obrócony bezkolizyjnie.
    /// </summary>
    bool CanBlockBeRotated()
    {
        TetrisBlock b = new(block);
        b.RotateBlock();
        int x = b.X;
        int y = b.Y;
        for (int i = 0; i < b.Width; i++)
            for (int j = 0; j < b.Height; j++)
                if (cells[j + y, i + x].Type > 0 && b.HasBlock(i, j))
                    return false;

        return true;
    }

    /// <summary>
    /// Usuwa daną linię
    /// </summary>
    /// <param name="line">Numer linii</param>
    void RemoveLine(int line)
    {
        for (int i = 0; i < gridWidth; i++)
            cells[line, i].DeactivateCell();
        for(int y=line-1;y>=0;y--)
        {
            for (int i = 0; i < gridWidth; i++)
            {
                cells[y + 1, i].SetCellValue(cells[y, i].Type);
                cells[y, i].DeactivateCell();
            }

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
                cells[y, x].SetCellValue(block.Type);
            }
    }

    /// <summary>
    /// Tworzy nowy blok do gry.
    /// </summary>
    void NewBlock()
    {
        int sizeofCellType = 5;
        int sizeofBlockType = (int)BlockType.size;
        if (block2 == null)
        {
            block = new TetrisBlock(Random.Range(1, sizeofCellType), (BlockType)Random.Range(0, sizeofBlockType));
            block2 = new TetrisBlock(Random.Range(1, sizeofCellType), (BlockType)Random.Range(0, sizeofBlockType));

        }
        else
        {
            block = block2;
            block2 = new TetrisBlock(Random.Range(1, sizeofCellType), (BlockType)Random.Range(0, sizeofBlockType));

        }
        if (nextBlock)
            nextBlock.SetBlockAtGrid(block2);
        EndGame = CheckCollisionBlock();

    }
    /// <summary>
    /// Sprawdza kolizje aktywowanego bloku z istniejącymi komórkami na planszy.
    /// </summary>
    /// <returns>Zwraca true, jeśli wystąpiła kolizja, w przeciwnym razie false.</returns>
    bool CheckCollisionBlock()
    {

        int x = block.X;
        int y = block.Y;
        if (y + block.Height > 20)
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
                CellNormalTetrisScript cell = Instantiate(cellScript,
                    new Vector2(transform.position.x + (x) * ratio, transform.position.y - (y) * ratio),
                    Quaternion.identity,
                    transform) as CellNormalTetrisScript;
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
                cells[y, x].DeactivateCell();
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
        loseController.hasSavedScore = false;

        SetColor();
        NewBlock();
    }
}
