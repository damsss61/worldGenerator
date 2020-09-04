using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WorleyNoise
{
    public static float cellSize;
    public static float[,] GenerateWorleyNoisePoints(Vector2 mapSize, float radius)
    {
        cellSize = radius / Mathf.Sqrt(2);
        Vector2 gridSize = new Vector2(Mathf.FloorToInt(mapSize.x / cellSize), Mathf.FloorToInt(mapSize.y / cellSize));
        int[,] grid = new int[(int)gridSize.x,(int)gridSize.y];

        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < gridSize.y; j++)
            {

            }

        }

        return new float[0, 0];
    }
}
