using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridClass
{
    public int width;
    public int height;
    private int[,] gridArray;

    public GridClass(int width, int height)
    {
        this.width = width;
        this.height = height;

        gridArray = new int[width, height];

        for ( int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                if ((x + y) % 2 == 0) { }

                else if ((x + y) % 2 == 1) { }
            }
        }
    }



}
