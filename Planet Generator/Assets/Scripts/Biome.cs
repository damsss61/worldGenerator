using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Biome
{
    public BiomeSettings biomeSettings;
    public Vector2Int biomeCenter;
    public Vector3 biomeCenterPercent;
    public BiomeMask biomeMask;
    public float[,] heightMap;
    public int[,] biomeIndex;
    public int mapSize;
    public float[,] noiseMap;
    public AnimationCurve heightCurve;

    public Biome(int mapSize, BiomeSettings biomeSettings)
    {
        
        this.biomeSettings = biomeSettings;
        this.mapSize = mapSize;
        AddBiomeCenter(mapSize);
        noiseMap = Noise.GenerateNoiseMap(mapSize, biomeSettings.heightMapSettings.noiseSettings, new Vector2(Random.Range(-10, 10), Random.Range(-10, 10)));
        heightCurve = new AnimationCurve(biomeSettings.heightMapSettings.heightCurve.keys);
        

    }

    public Biome(Vector2Int biomeCenter, BiomeSettings biomeSettings)
    {
        this.biomeSettings = biomeSettings;
        this.biomeCenter = biomeCenter;

    }

    public void AddBiomeCenter(int mapSize)
    {
        float dstFromChunkEdgePercent = 0.1f;
        int centerX = Mathf.RoundToInt(Random.Range(dstFromChunkEdgePercent * (mapSize - 3), (1 - dstFromChunkEdgePercent) * (mapSize - 3)));
        int centerY = Mathf.RoundToInt(Random.Range(dstFromChunkEdgePercent * (mapSize - 3), (1 - dstFromChunkEdgePercent) * (mapSize - 3)));
        biomeCenter = new Vector2Int(centerX, centerY);
        biomeCenterPercent = new Vector3(centerX / (float)mapSize, 0, centerY / (float)mapSize);
        
    }

    public void CreateBiomeMask(Mesh mesh, List<Chunk> neighbourChunks, float blend)
    {
        biomeMask = BiomeGenerator.GenerateBiomeMaskFromMesh(mesh, neighbourChunks, blend);

    }

}
