using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MenuSandAnimation : MonoBehaviour
{
    private static int gridHeight=100, gridWidth=177;
    public CellScript prefab;
    CellScript[,] grid=new CellScript[gridHeight,gridWidth];
    float ratio = 0.16f;
    List<Vector2Int> toUpdate = new List<Vector2Int>();
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
        nextColor();
        nColor=Random.Range(0,6);
        Time.timeScale = 1;
    }
    void nextColor()
    {
        nColor++;
        nColor = nColor % 7;
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
            nextColor();
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
            spawnSandTimer = 0.07f;
        }
        else
        {
            return;
        }
        int x = 10;
        if (IsFreeSpace(x, 0))
        {
            grid[0, x].SetCellValue(CellType.SandYellow, GetColor());
            toUpdate.Add(new Vector2Int(x, 0));
        }
        
        x = gridWidth - 10;
        if (IsFreeSpace(x, 0))
        {
            grid[0, x].SetCellValue(CellType.SandYellow, GetColor());
            toUpdate.Add(new Vector2Int(x, 0));
        }
    }
    void moveSand(Vector2Int active, Vector2Int nonactive)
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
    void AddBlocksOnTopToUpdate(Vector2Int pos)
    {
        if (pos.y - 1 >= 0 && pos.y - 1 < gridHeight && pos.x >= 0 && pos.x < gridHeight)
        {
            if (pos.x - 1 >= 0 && !grid[pos.y - 1, pos.x - 1].IsEmpty)
            {
                AddToUpdate(new Vector2Int(pos.x - 1, pos.y - 1));
            }
            if (pos.x + 1 < gridWidth && !grid[pos.y - 1, pos.x + 1].IsEmpty)
            {
                AddToUpdate(new Vector2Int(pos.x + 1, pos.y - 1));
            }
            if (!grid[pos.y - 1, pos.x].IsEmpty)
            {
                AddToUpdate(new Vector2Int(pos.x, pos.y - 1));
            }

        }

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
                Vector2Int c2 = new Vector2Int(c.x, c.y + 1);
                moveSand(c, c2);
                AddToUpdate(c2);
                AddBlocksOnTopToUpdate(c);
            }
            else if(c.x - 1 >= 0 && c.x + 1 < gridWidth 
                && grid[c.y + 1, c.x - 1].IsEmpty && grid[c.y + 1, c.x + 1].IsEmpty)
            {
                Vector2Int c2;
                if (Random.Range(0, 2) == 1) 
                {
                    c2 = new Vector2Int(c.x - 1, c.y + 1);
                }
                else
                {
                    c2 = new Vector2Int(c.x + 1, c.y + 1);
                }
                moveSand(c, c2);
                AddToUpdate(c2);
                AddBlocksOnTopToUpdate(c);
            }
            else if (c.x - 1 >= 0 && grid[c.y + 1, c.x - 1].IsEmpty) 
            {
                Vector2Int c2 = new Vector2Int(c.x - 1, c.y + 1);
                moveSand(c, c2);
                AddToUpdate(c2);
                AddBlocksOnTopToUpdate(c);
            }
            else if (c.x + 1 < gridWidth && grid[c.y + 1, c.x + 1].IsEmpty)
            {
                Vector2Int c2 = new Vector2Int(c.x + 1, c.y + 1);
                moveSand(c, c2);
                AddToUpdate(c2);
                AddBlocksOnTopToUpdate(c);
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
