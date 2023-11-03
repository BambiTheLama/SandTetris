using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisBlock
{
    public int Type { get; private set; }
    public int X { get; private set; }
    public int Y { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }

    private float yPos = 0;
    private float speed = 1.0f;
    private int[,] blockGrid;

    /// <summary>
    /// Konstruktor kopiuj¹cy bloku.
    /// </summary>
    /// <param name="block">Blok do skopiowania.</param>
    public TetrisBlock(TetrisBlock block)
    {
        Type = block.Type;
        Y = block.Y;
        X = block.X;
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

    /// <summary>
    /// Konstruktor bloku.
    /// </summary>
    /// <param name="type">Typ komórki bloku.</param>
    /// <param name="blockType">Typ bloku.</param>
    public TetrisBlock(int type, BlockType blockType)
    {
        Y = 0;
        Type = type;
        switch (blockType)
        {
            case BlockType.Box:
                Width = 2;
                Height = 2;
                break;
            case BlockType.JBlock:
            case BlockType.LBlock:
            case BlockType.TBlock:
            case BlockType.SBlock:
            case BlockType.ZBlock:
                Width = 2;
                Height = 3;
                break;
            case BlockType.IBlock:
                Width = 1;
                Height = 4;
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
                        blockGrid[j, i] = Type;
                    }
                }
                break;
            case BlockType.JBlock:
                for (int i = Width / 2; i < Width; i++)
                {
                    for (int j = 0; j < Height; j++)
                    {
                        blockGrid[j, i] = Type;
                    }
                }
                for (int i = 0; i < Width / 2; i++)
                {
                    for (int j = (2 * Height) / 3; j < Height; j++)
                    {
                        blockGrid[j, i] = Type;
                    }
                }
                break;
            case BlockType.LBlock:
                for (int i = 0; i < Width / 2; i++)
                {
                    for (int j = 0; j < Height; j++)
                    {
                        blockGrid[j, i] = Type;
                    }
                }
                for (int i = Width / 2; i < Width; i++)
                {
                    for (int j = (2 * Height) / 3; j < Height; j++)
                    {
                        blockGrid[j, i] = Type;
                    }
                }
                break;
            case BlockType.TBlock:
                for (int i = 0; i < Width / 2; i++)
                {
                    for (int j = 0; j < Height; j++)
                    {
                        blockGrid[j, i] = Type;
                    }
                }
                for (int i = Width / 2; i < Width; i++)
                {
                    for (int j = (Height) / 3; j < (2 * Height) / 3; j++)
                    {
                        blockGrid[j, i] = Type;
                    }
                }
                break;
            case BlockType.SBlock:
                for (int i = 0; i < Width / 2; i++)
                {
                    for (int j = 0; j < (Height * 2) / 3; j++)
                    {
                        blockGrid[j, i] = Type;
                    }
                }
                for (int i = Width / 2; i < Width; i++)
                {
                    for (int j = Height / 3; j < Height; j++)
                    {
                        blockGrid[j, i] = Type;
                    }
                }
                break;
            case BlockType.ZBlock:
                for (int i = Width / 2; i < Width; i++)
                {
                    for (int j = 0; j < (Height * 2) / 3; j++)
                    {
                        blockGrid[j, i] = Type;
                    }
                }
                for (int i = 0; i < Width / 2; i++)
                {
                    for (int j = Height / 3; j < Height; j++)
                    {
                        blockGrid[j, i] = Type;
                    }
                }
                break;


        }


        X = (10 - Width) / 2;
    }

    /// <summary>
    /// Obraca blok o 90 stopni w prawo.
    /// </summary>
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
        (Width, Height) = (Height, Width);
        if (X + Width >= 10)
            X = 10- Width;
    }
    /// <summary>
    /// Sprawdza, czy blok ma komórkê na podanej pozycji.
    /// </summary>
    /// <param name="x">Wspó³rzêdna X komórki.</param>
    /// <param name="y">Wspó³rzêdna Y komórki.</param>
    /// <returns>Prawda, jeœli blok ma komórkê na danej pozycji, w przeciwnym razie fa³sz.</returns>
    public bool HasBlock(int x, int y)
    {
        if (x < 0 || y < 0)
            return false;
        if (x >= Width || y >= Height)
            return false;
        return blockGrid[y, x] > 0;
    }
    public void MoveLeft()
    {
        if (X > 0)
        {
            X--;
            if (X < 0)
                X = 0;
        }
    }

    /// <summary>
    /// Przesuwa blok w prawo.
    /// </summary>
    public void MoveRight()
    {
        if (X + Width < 10)
        {
            X++;
            if (X + Width >= 10)
                X = 10 - Width;
        }
    }
    /// <summary>
    /// Rozpoczyna ruch bloku w dó³.
    /// </summary>
    public void MoveDown()
    {
        speed = 6.9f;
    }

    /// <summary>
    /// Realizuje ruch bloku w dó³ z uwzglêdnieniem prêdkoœci.
    /// </summary>
    /// <param name="deltaTime">Czas od ostatniej aktualizacji.</param>
    public void GoDown(float deltaTime)
    {
        yPos += deltaTime * speed * 21.37f;
        Y = (int)(yPos/8);
        if (Y + Height > 21)
            Y = 21 - Height;
        speed = 1;
    }
}
