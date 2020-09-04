using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PoissonSampler 
{
    public static float cellSize;

    public static List<Vector2> GenerateSamplePositions(float radius, Vector2 mapSize, int numberOfTryBeforeRejection)
    {
        cellSize = radius / Mathf.Sqrt(2);
        int[,] grid = new int[Mathf.CeilToInt(mapSize.x / cellSize), Mathf.CeilToInt(mapSize.y / cellSize)];

        List<Vector2> samplePos = new List<Vector2>();
        List<Vector2> activeList = new List<Vector2>();



        activeList.Add(new Vector2(Random.Range(0f, mapSize.x), Random.Range(0f, mapSize.y)));

        while (activeList.Count != 0)
        {
            int index = Random.Range(0, activeList.Count);
            Vector2 spawnCenter = activeList[index];
            bool candidateAccepted=false;

            for (int i = 0; i < numberOfTryBeforeRejection; i++)
            {
                float angle = Random.Range(0, 2 * Mathf.PI);
                Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                Vector2 candidate = spawnCenter + direction * Random.Range(radius, 2 * radius);

                if (IsValidCandidate(candidate, mapSize, radius, samplePos, grid))
                {
                    candidateAccepted = true;
                    samplePos.Add(candidate);
                    activeList.Add(candidate);
                    grid[(int)(candidate.x / cellSize), (int)(candidate.y / cellSize)] = samplePos.Count;
                    break;
                }
            }

            if (!candidateAccepted)
            {
                activeList.RemoveAt(index);
            }
        }




        return samplePos;
    }

    static bool IsValidCandidate(Vector2 candidate, Vector2 mapSize, float radius, List<Vector2> points, int[,] grid)
    {
        if (candidate.x >= 0 && candidate.x < mapSize.x && candidate.y >= 0 && candidate.y < mapSize.y)
        {
            int cellX = (int)(candidate.x / cellSize);
            int cellY = (int)(candidate.y / cellSize);
            int searchStartX = Mathf.Max(0, cellX - 2);
            int searchEndX = Mathf.Min(cellX + 2, grid.GetLength(0) - 1);
            int searchStartY = Mathf.Max(0, cellY - 2);
            int searchEndY = Mathf.Min(cellY + 2, grid.GetLength(1) - 1);

            for (int x = searchStartX; x <= searchEndX; x++)
            {
                for (int y = searchStartY; y <= searchEndY; y++)
                {
                    int pointIndex = grid[x, y] - 1;
                    if (pointIndex != -1)
                    {
                        float sqrDst = (candidate - points[pointIndex]).sqrMagnitude;
                        if (sqrDst < radius * radius)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        return false;
    }
}
