using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiffusionReaction : MonoBehaviour
{
    Cell[,] grid;
    Cell[,] next;
    public int height, width;
    SpriteRenderer sr;
    public float diffusionA = 1.0f;
    public float diffusionB = 0.5f;
    public float feed = 0.055f;
    public float killrate = 0.062f;
    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        InitGrid();
        RunAlgorithm();
        sr.sprite = GridToImage();
    }
    private void Update()
    {
        for(int i = 0; i < 10; i++)
        {
            Swap();
            RunAlgorithm();
        }
        
        sr.sprite = GridToImage();
    }
    void RunAlgorithm()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                next[x, y].a = NewA(grid[x,y].a, grid[x, y].b, x, y);
                next[x, y].b = NewB(grid[x, y].a, grid[x, y].b, x, y);
            }
        }
    }
    float NewA(float oldA, float oldB, int gx, int gy)
    {
        float newA = oldA + 
                    (diffusionA * WeightedKernel("a", gx, gy)) -
                    (oldA * oldB * oldB) +
                    (feed * (1 - oldA));
        return newA;
    }
    float NewB(float oldA, float oldB, int gx, int gy)
    {
        float newB = oldB +
                    (diffusionB * WeightedKernel("b", gx, gy)) +
                    (oldA * oldB * oldB) -
                    ((killrate + feed) * oldB);
        return newB;
    }
    public float diagonal = 0.05f;
    public float normal = 0.2f;
    float WeightedKernel(string mode, int gridX, int gridY)
    {
        float sum = 0;
        for(int x = gridX - 1; x <= gridX + 1; x++)
        {
           for(int y = gridY - 1; y <= gridY + 1; y++)
           {
                if(x > -1 && y > -1 && y < height && x < width)
                {
                    float mult = -1;
                    if (x != gridX && y != gridY)
                    {
                        mult = diagonal;
                    }
                    if (x == gridX || y == gridY)
                    {
                        if (x != gridX || y != gridY)
                        {
                            mult = normal;
                        }
                    }
                    if (mode == "a") sum += grid[x, y].a * mult;
                    else if (mode == "b") sum += grid[x, y].b * mult;
                    else Debug.LogError("invalid mode");
                }
                else
                {
                   /* if (x != gridX && y != gridY)
                    {
                        sum += diagonal;
                    }
                    if (x == gridX || y == gridY)
                    {
                        if (x != gridX || y != gridY)
                        {
                            sum += normal;
                        }
                    }*/
                }
          

            }
        }
        return sum;
    }
    void InitGrid()
    {

        grid = new Cell[width, height];
        next = new Cell[width, height];
        for(int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = new Cell(1.0f, 0.0f);
                next[x, y] = new Cell(0.0f, 1.0f);
            }
        }
        for(int x = 30; x < 45; x++)
        {
            for (int y = 30; y < 45; y++)
            {
                grid[x, y].b = 1.0f;
            }
        }
        for (int x = 80; x < 90; x++)
        {
            for (int y = 60; y < 70; y++)
            {
                grid[x, y].b = 1.0f;
            }
        }
        for (int x = 140; x < 150; x++)
        {
            for (int y = 165; y < 185; y++)
            {
                grid[x, y].b = 1.0f;
            }
        }
        for (int x = 90; x < 110; x++)
        {
            for (int y = 2; y < 10; y++)
            {
                grid[x, y].b = 1.0f;
            }
        }
        for (int x = 111; x < 145; x++)
        {
            for (int y = 45; y < 89; y++)
            {
                grid[x, y].b = 1.0f;
            }
        }
    }
    Sprite GridToImage()
    {
        Texture2D newTex = new Texture2D(width, height);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                newTex.SetPixel(x, y, CellToColor(next[x,y]));
            }
        }
        newTex.filterMode = FilterMode.Point;
        newTex.Apply();

        Sprite newSprite = Sprite.Create(newTex, new Rect(0, 0, newTex.width, newTex.height), Vector2.one * 0.5f);
        
        return newSprite;
    }
    Color CellToColor(Cell cellToObserve)
    {
        Color c = Color.black;
        c.r = cellToObserve.a;
        c.b = cellToObserve.b;
        return c;
    }
    void Swap()
    {
        Cell[,] temp = grid;
        grid = next;
        next = temp;
    }


    public class Cell
    {
        public float a;
        public float b;
        public Cell(float _A, float _B)
        {
            a = _A;
            b = _B;
        }
    }	
}
