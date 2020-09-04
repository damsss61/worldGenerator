using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Biome
{
    public BiomeSettings biomeSettings;
    public Vector2 biomeCenter;
    public float[,] biomeMask;
    public float[,] heightMap;
    public int[,] biomeIndex;

    public Biome(Vector2 biomCenter, BiomeSettings biomeSettings)
    {
        this.biomeCenter = biomCenter;
        this.biomeSettings = biomeSettings;
        
    }

    public void CreateBiomeMask(float[,] heightMap, int[,] biomeIndex)
    {
        this.heightMap = heightMap;
        this.biomeIndex = biomeIndex;
    }

}
