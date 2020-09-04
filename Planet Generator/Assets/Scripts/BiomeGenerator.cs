using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class BiomeGenerator
{
    public static BiomeMask GenerateBiomes(int mapSize, List<Biome> biomes, float blend, bool[] maskToUse)
    {

        int[,] biomeIndex = new int[mapSize, mapSize];
        List<HeightMap> masks = new List<HeightMap>(9);

        for (int i = 0; i < 9; i++)
        {
            masks.Add(new HeightMap(new float[mapSize, mapSize], 0, 1));
        }


        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                Vector2 pos = new Vector2(x, y);

                float[] influence = new float[9];
                float sum = 0;
                for (int k = 0; k < 9; k++)
                {
                    float minVal = float.MaxValue;
                    if (maskToUse[k])
                    {
                        influence[k] = float.MaxValue;
                        
                        for (int l = 0; l < 9; l++)
                        {
                            if (l != k && maskToUse[l])
                            {
                                
                                float val = 1f-Mathf.Clamp01(Vector2.Dot(biomes[k].biomeCenter - biomes[l].biomeCenter, biomes[k].biomeCenter - pos) / Vector2.SqrMagnitude(biomes[k].biomeCenter - biomes[l].biomeCenter));
                                float val2 = Mathf.Pow(val, blend) / (Mathf.Pow(val, blend) + Mathf.Pow(1 - val, blend));
                                if (val <minVal)
                                {
                                    minVal = val;
                                    influence[k] = val2;
                                }
                                
                            }
                        }
                        sum += influence[k];
                        
                    }
                    else
                    {
                        influence[k] = 0f;
                    }
                    
                }

                for (int k = 0; k < 9; k++)
                {
                    float weight = influence[k] / sum;
                    masks[k].values[x, y] = weight;
                    
                }


            }
        }

        return new BiomeMask(masks, biomeIndex);
    }




    public static List<Biome> InitializeBiome(int mapSize, BiomeSettings[] biomesSettings)
    {
        List<Biome> biomes = new List<Biome>(9);

        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                int centerX = Mathf.RoundToInt(Random.Range((i + 0.2f) * mapSize, (i + 0.8f) * mapSize));
                int centerY = Mathf.RoundToInt(Random.Range((j + 0.2f) * mapSize, (j + 0.8f) * mapSize));
                Vector2 biomeCenter = new Vector2(centerX, centerY);
                Biome biome = new Biome(biomeCenter, biomesSettings[Random.Range(0, biomesSettings.Length)]);
                biomes.Add(biome);
            }
        }

        return biomes;
    }

}

public struct BiomeMask
{

    public List<HeightMap> mask;
    public int[,] biomeIndex;


    public BiomeMask(List<HeightMap> mask, int[,] biomeIndex)
    {
        this.biomeIndex = biomeIndex;
        this.mask = mask;
    }
}




