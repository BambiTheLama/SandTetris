using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MenuSandAnimation : MonoBehaviour
{
    private static readonly int gridHeight = 173;
    private static readonly int gridWidth = 320;
    public CellScript prefab;
    readonly CellScript[,] grid = new CellScript[gridHeight, gridWidth];
    readonly float ratio = 0.16f;
    List<Vector2Int> toUpdate = new();
    Color color;
    int nColor;
    float time = 0;
    float spawnSandTimer = 0;

    void Start()
    {
        GenerateGrid();
        ClearColor();
        time = 1;
        spawnSandTimer = 0;
        color.a = 1;
        NextColor();
        nColor=Random.Range(0,6);
        Time.timeScale = 1;
    }
    void NextColor()
    {
        nColor++;
        nColor %= 7;
        switch(nColor)
        {
            case 0:
                color = Color.red;
                break;
            case 1:
                color = new Color(1f,0.5f,0.2f);
                break;
            case 2:
                color = Color.yellow;
                break;
            case 3:
                color = Color.green;
                break;
            case 4:
                color = new Color(0,1,1);
                break;
            case 5:
                color = Color.blue;
                break;
            case 6:
                color = new Color(0.69f, 0f, 1f);
                break;
        }
    }
    void Update()
    {
        UpdateSand();
        SpawnSand();
        time-=Time.deltaTime;
        if(time <= 0) 
        {
            time = 5;
            NextColor();
        }
    }

    bool IsFreeSpace(int x,int y)
    {
        return grid[y, x].IsEmpty;
    }
    public Color GetColor()
    {

        int v=Random.Range(1, 4);

        if (v == 1)
            return color;
        else if (v == 2)
            return new Color(color.r * 0.9f, color.g * 0.9f, color.b * 0.9f, 1);
        else if (v == 3)
            return new Color(color.r * 0.7f, color.g * 0.7f, color.b * 0.7f, 1);
        else if (v == 4)
            return new Color(color.r * 0.5f, color.g * 0.5f, color.b * 0.5f, 1);
        return new Color(1, 1, 1, 1);
    }
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
        int n=Random.Range(1, 7);
        int x;
        for (int i = -n/2; i < n/2+1; i++)
        {
            if (Random.Range(0, 3) == 1)
                continue;
            x = gridWidth / 8 + (i*3);
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
            x = 7*gridWidth / 8 + (i * 3);
            if (IsFreeSpace(x, 0))
            {
                grid[0, x].SetCellValue(CellType.SandYellow, GetColor());
                toUpdate.Add(new Vector2Int(x, 0));
            }
        }

    }
    void MoveSand(Vector2Int active, Vector2Int nonactive)
    {
        CellScript c = grid[active.y, active.x];
        CellScript c2 = grid[nonactive.y, nonactive.x];
        c2.SetCellValue(c.Type, c.Color);
        c.DeactivateCellClear();
    }

    void AddToUpdate(Vector2Int p)
    {
        if (!toUpdate.Contains(p))
            toUpdate.Add(p);
    }

    void UpdateSand()
    {
        List<Vector2Int> toUpdate = this.toUpdate;
        this.toUpdate=new List<Vector2Int>();

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
            else if(c.x - 1 >= 0 && c.x + 1 < gridWidth 
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
