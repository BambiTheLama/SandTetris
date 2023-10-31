using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Typy bloków.
/// </summary>
public enum BlockType
{
    Box, LBlock, JBlock, TBlock, IBlock, SBlock, ZBlock
}

/// <summary>
/// Klasa reprezentuj¹ca blok w grze.
/// </summary>
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


    /// <summary>
    /// Konstruktor kopiuj¹cy bloku.
    /// </summary>
    /// <param name="block">Blok do skopiowania.</param>
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

    /// <summary>
    /// Konstruktor bloku.
    /// </summary>
    /// <param name="type">Typ komórki bloku.</param>
    /// <param name="blockType">Typ bloku.</param>
    public Block(CellType type, BlockType blockType)
    {
        Y = 0;
        switch (blockType)
        {
            case BlockType.Box:
                Width = 16;
                Height = 16;
                break;
            case BlockType.JBlock:
            case BlockType.LBlock:
            case BlockType.TBlock:
            case BlockType.SBlock:
            case BlockType.ZBlock:
                Width = 16;
                Height = 24;
                break;
            case BlockType.IBlock:
                Width = 32;
                Height = 8;
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
        blockColor = CellType switch
        {
            CellType.SandYellow => Color.yellow,
            CellType.SandRed => Color.red,
            CellType.SandBlue => Color.blue,
            CellType.SandGreen => Color.green,
            _ => Color.white,
        };
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
        if (X + Width >= 80)
            X = 79 - Width;
    }

    /// <summary>
    /// Przesuwa blok w lewo.
    /// </summary>
    public void MoveLeft()
    {
        if (X > 0)
        {
            X -= 10;
            if (X < 0)
                X = 0;
        }
    }

    /// <summary>
    /// Przesuwa blok w prawo.
    /// </summary>
    public void MoveRight()
    {
        if (X + Width < 80)
        {
            X += 10;
            if (X + Width >= 80)
                X = 80 - Width;
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
        Y = (int)yPos;
        if (Y + Height >= 160)
            Y = 160 - Height;
        speed = 1;
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


    /// <summary>
    /// Zwraca kolor komórki bloku na podanej pozycji.
    /// </summary>
    /// <param name="x">Wspó³rzêdna X komórki.</param>
    /// <param name="y">Wspó³rzêdna Y komórki.</param>
    /// <returns>Kolor komórki bloku na danej pozycji.</returns>
    public Color GetColor(int x, int y)
    {
        Color color = blockColor;
        float alpha = 1.0f;

        switch (blockGrid[y, x])
        {
            case 0:
                return new Color(0, 0, 0, 0);
            case 1:
                break;
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

}
