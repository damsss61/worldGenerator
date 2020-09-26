using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class BiomeGenerator
{
    public static BiomeMask GenerateBiomesMask(int mapSize, List<Biome> biomes, float blend, bool[] maskToUse)
    {
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
        
        return new BiomeMask(masks);
    }

    public static List<Biome> InitializeBiome(int mapSize, BiomeSettings[] biomesSettings)
    {
        List<Biome> biomes = new List<Biome>(9);
        float dstFromChunkEdgePercent = 0.2f;
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                int centerX = Mathf.RoundToInt(Random.Range((i + dstFromChunkEdgePercent) * mapSize, (i + 1- dstFromChunkEdgePercent) * mapSize));
                int centerY = Mathf.RoundToInt(Random.Range((j + dstFromChunkEdgePercent) * mapSize, (j + 1- dstFromChunkEdgePercent) * mapSize));
                Vector2Int biomeCenter = new Vector2Int(centerX, centerY);
                Biome biome = new Biome(biomeCenter, biomesSettings[Random.Range(0, biomesSettings.Length)]);
                biomes.Add(biome);
            }
        }

        return biomes;
    }


    public static BiomeMask GenerateBiomeMaskFromMesh(Mesh mesh, List<Chunk> neighbourChunks, float blend)
    {
        List<HeightMap> masks = new List<HeightMap>(9);
        int numVerticesPerLine = (int)Mathf.Sqrt(mesh.vertexCount);
        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < 9; i++)
        {
            masks.Add(new HeightMap(new float[numVerticesPerLine, numVerticesPerLine], 0, 1));
        }

        for (int y = 0; y < numVerticesPerLine; y++)
        {
            for (int x = 0; x < numVerticesPerLine; x++)
            {
                int VertexIncrement = y * numVerticesPerLine + x;
                Vector3 pos = vertices[VertexIncrement];
                float sum = 0;
                float[] influence = new float[9];

                for (int i = 0; i < neighbourChunks.Count; i++)
                {
                    float minVal = float.MaxValue;
                    influence[i] = float.MaxValue;

                    for (int j = 0; j < neighbourChunks.Count; j++)
                    {
                        if (i != j)
                        {
                            
                            float val = 1f - Mathf.Clamp01(Vector3.Dot(neighbourChunks[i].biomeProjectedCenter - neighbourChunks[j].biomeProjectedCenter, neighbourChunks[i].biomeProjectedCenter - pos) / Vector3.SqrMagnitude(neighbourChunks[i].biomeProjectedCenter - neighbourChunks[j].biomeProjectedCenter));
                            float val2 = Mathf.Pow(val, blend) / (Mathf.Pow(val, blend) + Mathf.Pow(1 - val, blend));
                            if (val < minVal)
                            {
                                minVal = val;
                                influence[i] = val2;
                            }
                        }
                    }
                    sum += influence[i];
                }
                for (int k = 0; k < neighbourChunks.Count; k++)
                {
                    float weight = influence[k];
                    masks[k].values[x, y] = weight/sum;
                }
            }
        }

        return new BiomeMask(masks);
    }
}

public struct BiomeMask
{

    public List<HeightMap> mask;


    public BiomeMask(List<HeightMap> mask)
    {
        this.mask = mask;
    }
}




