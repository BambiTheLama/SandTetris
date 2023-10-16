using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockType
{
    Box, LBlock, JBlock, TBlock, IBlock, SBlock, ZBlock
}
public class Block
{
    public CellType CellType { get; private set; }
    public int X { get; private set; }
    public int Y { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }

    private float yPos = 0;
    private float speed = 1.0f;
    private int[,] blockGrid;
    private Color blockColor;

    public Block(Block block)
    {
        CellType = block.CellType;
        Y = block.Y;
        X = block.X;
        blockColor = block.blockColor;
        Height = block.Height;
        Width = block.Width;
        speed = block.speed;
        yPos = block.yPos;
        blockGrid = new int[Height, Width];
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                blockGrid[i, j] = block.blockGrid[i, j];
            }
        }
    }
    public Block(CellType type, BlockType blockType)
    {
        Y = 0;
        switch (blockType)
        {
            case BlockType.Box:
                Width = 20;
                Height = 20;
                break;
            case BlockType.JBlock:
            case BlockType.LBlock:
            case BlockType.TBlock:
            case BlockType.SBlock:
            case BlockType.ZBlock:
                Width = 20;
                Height = 30;
                break;
            case BlockType.IBlock:
                Width = 40;
                Height = 10;
                break;
        }

        blockGrid = new int[Height, Width];
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                blockGrid[j, i] = 0;
            }
        }
        switch (blockType)
        {
            case BlockType.IBlock:
            case BlockType.Box:
                for (int i = 0; i < Width; i++)
                {
                    for (int j = 0; j < Height; j++)
                    {
                        blockGrid[j, i] = 1;
                    }
                }
                break;
            case BlockType.JBlock:
                for (int i = Width /2; i < Width; i++)
                {
                    for (int j = 0; j < Height; j++)
                    {
                        blockGrid[j, i] = 1;
                    }
                }
                for (int i = 0; i < Width / 2; i++) 
                {
                    for (int j = (2 * Height) / 3; j < Height; j++)
                    {
                        blockGrid[j, i] = 1;
                    }
                }
                break;
            case BlockType.LBlock:
                for (int i = 0; i < Width /2; i++)
                {
                    for (int j = 0; j < Height; j++)
                    {
                        blockGrid[j, i] = 1;
                    }
                }
                for (int i = Width /2; i < Width; i++)
                {
                    for (int j = (2*Height)/3; j < Height; j++)
                    {
                        blockGrid[j, i] = 1;
                    }
                }
                break;
            case BlockType.TBlock:
                for (int i = 0; i < Width / 2; i++)
                {
                    for (int j = 0; j < Height; j++)
                    {
                        blockGrid[j, i] = 1;
                    }
                }
                for (int i = Width / 2; i < Width; i++)
                {
                    for (int j = (Height) / 3; j < (2*Height)/3; j++)
                    {
                        blockGrid[j, i] = 1;
                    }
                }
                break;
            case BlockType.SBlock:
                for (int i = 0; i < Width / 2; i++)
                {
                    for (int j = 0; j < (Height * 2) / 3; j++)
                    {
                        blockGrid[j, i] = 1;
                    }
                }
                for (int i = Width / 2; i < Width; i++)
                {
                    for (int j = Height / 3; j < Height; j++)
                    {
                        blockGrid[j, i] = 1;
                    }
                }
                break;
            case BlockType.ZBlock:
                for (int i = Width / 2; i < Width; i++)
                {
                    for (int j = 0; j < (Height * 2) / 3; j++)
                    {
                        blockGrid[j, i] = 1;
                    }
                }
                for (int i = 0; i < Width / 2; i++)
                {
                    for (int j = Height / 3; j < Height; j++)
                    {
                        blockGrid[j, i] = 1;
                    }
                }
                break;


        }
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                if (blockGrid[j, i] == 1)
                {
                    blockGrid[j, i] = Random.Range(1, 4);
                }
            }
        }

        CellType = type;
        X = 50 - Width / 2;
        switch (CellType)
        {
            case CellType.SandYellow:
                blockColor = Color.yellow;
                break;
            case CellType.SandRed:
                blockColor = Color.red;
                break;
            case CellType.SandBlue:
                blockColor = Color.blue;
                break;
            case CellType.SandGreen:
                blockColor = Color.green;
                break;
            default:
                blockColor = Color.white;
                break;
        }
    }


    public void RotateBlock()
    {
        int[,] blockGrid = new int[Width, Height];
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                blockGrid[j, i] = this.blockGrid[Height - 1 - i, j];
            }
        }

        this.blockGrid = blockGrid;
        int t = Height;
        Height = Width;
        Width = t;
        if (X + Width >= 100)
            X = 99 - Width;
    }


    public void MoveLeft()
    {
        if (X > 0)
        {
            X -= 10;
            if (X < 0)
                X = 0;
        }
    }

    public void MoveRight()
    {
        if (X + Width < 100)
        {
            X += 10;
            if (X + Width >= 100)
                X = 100 - Width;
        }
    }

    public void MoveDown()
    {
        speed = 6.9f;
    }

    public void GoDown(float deltaTime)
    {
        yPos += deltaTime * speed * 21.37f;
        Y = (int)yPos;
        if (Y + Height >= 200)
            Y = 200 - Height;
        speed = 1;
    }

    public bool HasBlock(int x, int y)
    {
        if (x < 0 || y < 0)
            return false;
        if (x >= Width || y >= Height)
            return false;
        return blockGrid[y, x] > 0;
    }
    public Color GetColor(int x, int y)
    {
        Color color = blockColor;
        if (blockGrid[y, x] == 0)
            return new Color(0, 0, 0, 0);
        else if (blockGrid[y, x] == 1)
            return color;
        else if (blockGrid[y, x] == 2)
            return new Color(color.r * 0.9f, color.g * 0.9f, color.b * 0.9f, 1);
        else if (blockGrid[y, x] == 3)
            return new Color(color.r * 0.7f, color.g * 0.7f, color.b * 0.7f, 1);
        else if (blockGrid[y, x] == 4)
            return new Color(color.r * 0.5f, color.g * 0.5f, color.b * 0.5f, 1);
        return new Color(1, 1, 1, 1);
    }
}
