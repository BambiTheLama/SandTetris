using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BlockType
{
    Box,LBlock,ReversLBlock,TBlock,IBlock,SBlock,ZBlock,
    size
}

public class Block
{
    public CellType cellType { get; private set; }
    public int y { get; private set; }
    float yPos = 0;
    float speed = 1.0f;
    public int x { get;private set; }
    public int h { get;private set; }
    public int w { get;private set; }
    int[,] blockGred;
    Color color;

    public Block(Block b)
    {
        cellType = b.cellType;  
        y=b.y;
        x=b.x; 
        color=b.color;
        h=b.h;
        w=b.w;
        speed=b.speed;
        yPos = b.yPos;
        blockGred = new int[h, w];
        for(int i = 0; i < h; i++)
            for(int j = 0; j < w; j++)
                blockGred[i, j] = b.blockGred[i,j];
    }
    public Block(CellType type, BlockType b)
    {
        y = 0;
        switch(b)
        {
            case BlockType.Box:
                w = 20;
                h = 20;
                break;
            case BlockType.ReversLBlock:
            case BlockType.LBlock:
            case BlockType.TBlock:
            case BlockType.SBlock:
            case BlockType.ZBlock:
                w = 20;
                h = 30;
                break;
            case BlockType.IBlock:
                w= 10;
                h = 40;
                break;


                
        }
        blockGred = new int[h, w];
        for (int i = 0; i < w; i++) {
            for(int j = 0; j < h; j++)
            {
                blockGred[j, i] = 0;
            }
        }
        switch (b)
        {
            case BlockType.IBlock:
            case BlockType.Box:
                for (int i = 0; i < w; i++)
                {
                    for (int j = 0; j < h; j++)
                    {
                        blockGred[j, i] = 1;
                    }
                }
                break;
            case BlockType.ReversLBlock:
                for (int i = w/2; i < w; i++)
                {
                    for (int j = 0; j < h; j++)
                    {
                        blockGred[j, i] = 1;
                    }
                }
                for (int i = 0; i < w / 2; i++) 
                {
                    for (int j = (2 * h) / 3; j < h; j++)
                    {
                        blockGred[j, i] = 1;
                    }
                }
                break;
            case BlockType.LBlock:
                for (int i = 0; i < w/2; i++)
                {
                    for (int j = 0; j < h; j++)
                    {
                        blockGred[j, i] = 1;
                    }
                }
                for (int i = w/2; i < w; i++)
                {
                    for (int j = (2*h)/3; j < h; j++)
                    {
                        blockGred[j, i] = 1;
                    }
                }
                break;
            case BlockType.TBlock:
                for (int i = 0; i < w / 2; i++)
                {
                    for (int j = 0; j < h; j++)
                    {
                        blockGred[j, i] = 1;
                    }
                }
                for (int i = w / 2; i < w; i++)
                {
                    for (int j = (h) / 3; j < (2*h)/3; j++)
                    {
                        blockGred[j, i] = 1;
                    }
                }
                break;
            case BlockType.SBlock:
                for (int i = 0; i < w / 2; i++)
                {
                    for (int j = 0; j < (h * 2) / 3; j++)
                    {
                        blockGred[j, i] = 1;
                    }
                }
                for (int i = w / 2; i < w; i++)
                {
                    for (int j = h / 3; j < h; j++)
                    {
                        blockGred[j, i] = 1;
                    }
                }
                break;
            case BlockType.ZBlock:
                for (int i = w / 2; i < w; i++)
                {
                    for (int j = 0; j < (h * 2) / 3; j++)
                    {
                        blockGred[j, i] = 1;
                    }
                }
                for (int i = 0; i < w / 2; i++)
                {
                    for (int j = h / 3; j < h; j++)
                    {
                        blockGred[j, i] = 1;
                    }
                }
                break;


        }
        for (int i = 0; i < w; i++)
            for (int j = 0; j < h; j++) 
            {
                if(blockGred[j, i]==1)
                {
                    blockGred[j, i] = Random.Range(1, 4);

                }

            }

        cellType = type;
        x = 50 - w / 2;
        switch(cellType)
        {
            case CellType.sand: 
                color=Color.yellow;
                break;
            case CellType.sand2:
                color=Color.red;
                break;
            case CellType.sand3:
                color = Color.blue;
                break;
            case CellType.sand4:
                color=Color.green;
                break;
            default: color=Color.white;
                break;
        }

    }

    void rotateBlock()
    {
        int[,] blockGred = new int[w, h];
        for (int i = 0; i < h; i++)
        {
            for (int j = 0; j < w; j++)
            {
                blockGred[j, i] = this.blockGred[h - 1 - i, j];
            }
        }

        this.blockGred= blockGred;
        int t = h;
        h = w;
        w = t;
        if (x + w >= 100)
            x = 99 - w;

    }
    public void rotateLeft()
    {
        rotateBlock();
    }

    public void moveLeft()
    {
        if (x > 0)
        {
            x -= 10;
            if(x<0)
                x = 0;
        }


    }
    public void moveRight()
    {
        if(x+w<100)
        {
            x+=10;
            if (x + w >= 100)
                x = 100-w;
        }

    }

    public void speedFalling() {
        speed = 6.9f;
    }

    public void goDown(float deltaTime)
    {
        yPos += deltaTime * speed * 21.37f;
        y = (int)yPos;
        if (y + h >= 200)
            y = 200 - h;
        speed = 1;
    }
    public bool hasBlock(int x,int y)
    {
        if (x < 0 || y < 0)
            return false;
        if (x >= w || y >= h)
            return false;
        return blockGred[y, x] > 0;
    }
    public Color GetColor(int x,int y)
    {
        Color c = color;
        if (blockGred[y, x] == 0)
            return new Color(0, 0, 0, 0);
        else if (blockGred[y, x] == 1)
            return c;
        else if (blockGred[y, x] == 2)
            return new Color(c.r * 0.9f, c.g * 0.9f, c.b * 0.9f, 1);
        else if (blockGred[y, x] == 3)
            return new Color(c.r * 0.7f, c.g * 0.7f, c.b * 0.7f, 1);
        else if (blockGred[y, x] == 4)
            return new Color(c.r * 0.5f, c.g * 0.5f, c.b * 0.5f, 1);
        return new Color(1, 1, 1, 1);
    }
}
