using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Biome 
{
    public BiomeSettings biomeSettings;
    public Vector2Int biomeCenter;
    public Vector3 biomeCenterPercent;
    public float[,] biomeMask;
    public float[,] heightMap;
    public int[,] biomeIndex;

    public Biome(int mapSize, BiomeSettings biomeSettings)
    {
        this.biomeSettings = biomeSettings;
        AddBiomeCenter(mapSize);

    }

    public Biome(Vector2Int biomeCenter, BiomeSettings biomeSettings)
    {
        this.biomeSettings = biomeSettings;
        this.biomeCenter = biomeCenter;

    }

    public void AddBiomeCenter(int mapSize)
    {
        float dstFromChunkEdgePercent = 0.2f;
        int centerX = Mathf.RoundToInt(Random.Range(dstFromChunkEdgePercent * (mapSize-3), (1 - dstFromChunkEdgePercent) * (mapSize-3)));
        int centerY = Mathf.RoundToInt(Random.Range(dstFromChunkEdgePercent * (mapSize-3), (1 - dstFromChunkEdgePercent) * (mapSize-3)));
        biomeCenter =new Vector2Int(centerX, centerY);
        biomeCenterPercent = new Vector3(centerX / (float)mapSize,0, centerY / (float)mapSize);
    }

    public void CreateBiomeMask(float[,] heightMap, int[,] biomeIndex)
    {
        this.heightMap = heightMap;
        this.biomeIndex = biomeIndex;
    }

}
