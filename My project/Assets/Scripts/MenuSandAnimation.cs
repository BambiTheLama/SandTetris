using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Klasa odpowiedzialna za animacjê piasku w menu.
/// </summary>
public class MenuSandAnimation : MonoBehaviour
{
    /// <summary>
    /// Wysokoœæ siatki komórek piasku.
    /// </summary>
    private static readonly int gridHeight = 173;

    /// <summary>
    /// Szerokoœæ siatki komórek piasku.
    /// </summary>
    private static readonly int gridWidth = 320;

    /// <summary>
    /// Prefabrykat komórki piasku.
    /// </summary>
    public CellScript prefab;

    /// <summary>
    /// Tablica komórek piasku.
    /// </summary>
    readonly CellScript[,] grid = new CellScript[gridHeight, gridWidth];

    /// <summary>
    /// Stosunek rozmiaru komórki do odstêpu miêdzy nimi.
    /// </summary>
    readonly float ratio = 0.16f;

    /// <summary>
    /// Lista komórek do aktualizacji.
    /// </summary>
    List<Vector2Int> toUpdate = new();

    /// <summary>
    /// Aktualny kolor.
    /// </summary>
    Color color;

    /// <summary>
    /// Numer aktualnego koloru.
    /// </summary>
    int nColor;

    /// <summary>
    /// Timer do zmiany koloru.
    /// </summary>
    float time = 0;

    /// <summary>
    /// Timer do spawnu piasku.
    /// </summary>
    float spawnSandTimer = 0;

    /// <summary>
    /// Inicjalizacja.
    /// </summary>
    void Start()
    {
        GenerateGrid();
        ClearColor();
        time = 1;
        spawnSandTimer = 0;
        color.a = 1;
        NextColor();
        nColor = Random.Range(0, 6);
        Time.timeScale = 1;
    }

    /// <summary>
    /// Wybierz nastêpny kolor.
    /// </summary>
    void NextColor()
    {
        nColor++;
        nColor %= 7; // Zawijanie do zakresu od 0 do 6
        // Przypisanie koloru na podstawie numeru koloru
        switch (nColor)
        {
            case 0:
                color = Color.red;
                break;
            case 1:
                color = new Color(1f, 0.5f, 0.2f);
                break;
            case 2:
                color = Color.yellow;
                break;
            case 3:
                color = Color.green;
                break;
            case 4:
                color = new Color(0, 1, 1);
                break;
            case 5:
                color = Color.blue;
                break;
            case 6:
                color = new Color(0.69f, 0f, 1f);
                break;
        }
    }

    /// <summary>
    /// Aktualizacja.
    /// </summary>
    void Update()
    {
        UpdateSand();
        SpawnSand();
        time -= Time.deltaTime;
        if (time <= 0)
        {
            time = 5;
            NextColor();
        }
    }

    /// <summary>
    /// SprawdŸ, czy dana komórka jest wolna.
    /// </summary>
    /// <param name="x">Wspó³rzêdna x komórki.</param>
    /// <param name="y">Wspó³rzêdna y komórki.</param>
    /// <returns>Zwraca true, jeœli komórka jest pusta, w przeciwnym razie false.</returns>
    bool IsFreeSpace(int x, int y)
    {
        return grid[y, x].IsEmpty;
    }

    /// <summary>
    /// Generowanie koloru na podstawie aktualnego.
    /// </summary>
    /// <returns>Nowy kolor na podstawie aktualnego z przyciemnieniem.</returns>
    public Color GetColor()
    {
        int v = Random.Range(1, 5);
        float alpha;
        switch (v)
        {
            case 1:
                return color;
            case 2:
                alpha = 0.9f;
                break;
            case 3:
                alpha = 0.7f;
                break;
            case 4:
                alpha = 0.5f;
                break;
            default:
                return new Color(1, 1, 1, 1);
        }
        return new Color(color.r * alpha, color.g * alpha, color.b * alpha, 1);
    }

    /// <summary>
    /// Spawn piasku.
    /// </summary>
    void SpawnSand()
    {
        spawnSandTimer -= Time.deltaTime;
        if (spawnSandTimer <= 0)
        {
            spawnSandTimer = Random.Range(0.21f, 0.37f);
        }
        else
        {
            return;
        }

        int n = Random.Range(1, 7);
        int x;
        for (int i = -n / 2; i < n / 2 + 1; i++)
        {
            if (Random.Range(0, 3) == 1)
                continue;
            x = gridWidth / 8 + (i * 3);
            if (IsFreeSpace(x, 0))
            {
                grid[0, x].SetCellValue(CellType.SandYellow, GetColor());
                toUpdate.Add(new Vector2Int(x, 0));
            }
        }

        n = Random.Range(1, 7);
        for (int i = -n / 2; i < n / 2 + 1; i++)
        {
            if (Random.Range(0, 3) == 1)
                continue;
            x = 7 * gridWidth / 8 + (i * 3);
            if (IsFreeSpace(x, 0))
            {
                grid[0, x].SetCellValue(CellType.SandYellow, GetColor());
                toUpdate.Add(new Vector2Int(x, 0));
            }
        }
    }

    /// <summary>
    /// Przesuwaie piasku z jednej komórki do drugiej.
    /// </summary>
    /// <param name="active">Aktywna komórka.</param>
    /// <param name="nonactive">Nieaktywna komórka.</param>
    void MoveSand(Vector2Int active, Vector2Int nonactive)
    {
        CellScript c = grid[active.y, active.x];
        CellScript c2 = grid[nonactive.y, nonactive.x];
        c2.SetCellValue(c.Type, c.Color);
        c.DeactivateCellClear();
    }

    /// <summary>
    /// Dodawanie komórki do listy do aktualizacji.
    /// </summary>
    /// <param name="p">Pozycja komórki.</param>
    void AddToUpdate(Vector2Int p)
    {
        if (!toUpdate.Contains(p))
            toUpdate.Add(p);
    }

    /// <summary>
    /// Aktualizacja animacji opadania piasku.
    /// </summary>
    void UpdateSand()
    {
        List<Vector2Int> toUpdate = this.toUpdate;
        this.toUpdate = new List<Vector2Int>();

        foreach (var c in toUpdate)
        {
            if (c.y + 1 >= gridHeight)
                continue;
            if (grid[c.y + 1, c.x].IsEmpty)
            {
                Vector2Int c2 = new(c.x, c.y + 1);
                MoveSand(c, c2);
                AddToUpdate(c2);
            }
            else if (c.x - 1 >= 0 && c.x + 1 < gridWidth
                && grid[c.y + 1, c.x - 1].IsEmpty && grid[c.y + 1, c.x + 1].IsEmpty)
            {
                Vector2Int c2;
                if (Random.Range(0, 2) == 1)
                {
                    c2 = new(c.x - 1, c.y + 1);
                }
                else
                {
                    c2 = new(c.x + 1, c.y + 1);
                }
                MoveSand(c, c2);
                AddToUpdate(c2);
            }
            else if (c.x - 1 >= 0 && grid[c.y + 1, c.x - 1].IsEmpty)
            {
                Vector2Int c2 = new(c.x - 1, c.y + 1);
                MoveSand(c, c2);
                AddToUpdate(c2);
            }
            else if (c.x + 1 < gridWidth && grid[c.y + 1, c.x + 1].IsEmpty)
            {
                Vector2Int c2 = new(c.x + 1, c.y + 1);
                MoveSand(c, c2);
                AddToUpdate(c2);
            }
        }
    }

    /// <summary>
    /// Wygenerowanie siatki komórek piasku.
    /// </summary>
    void GenerateGrid()
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                CellScript cell = Instantiate(prefab,
                    new Vector2(transform.position.x + (x) * ratio, transform.position.y - (y) * ratio),
                    Quaternion.identity,
                    transform) as CellScript;
                grid[y, x] = cell;
            }
        }
    }

    /// <summary>
    /// Czyszczenie kolorów w komórkach.
    /// </summary>
    void ClearColor()
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                CellScript cell = grid[y, x];
                cell.DeactivateCellClear();
            }
        }
    }
}
