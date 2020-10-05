using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HeightMapGenerator
{

    static float[,] falloffMap;
    public static HeightMap GenerateHeightMap(int mapSize, List<Biome> biomes, BiomeMask biomeMask, Vector2 sampleCentre, bool useFalloff, bool[] maskToUse)
    {
        float[,] values = new float[mapSize, mapSize];
        List<float[,]> noiseMaps = new List<float[,]>(9);
        List<AnimationCurve> heightCurves = new List<AnimationCurve>(9);
        for (int i = 0; i < 9; i++)
        {
            if (maskToUse[i])
            {
                noiseMaps.Add(Noise.GenerateNoiseMap(mapSize, biomes[i].biomeSettings.heightMapSettings.noiseSettings, sampleCentre));
                heightCurves.Add(new AnimationCurve(biomes[i].biomeSettings.heightMapSettings.heightCurve.keys));
            }
            else
            {
                noiseMaps.Add(new float[mapSize, mapSize]);
                heightCurves.Add(new AnimationCurve());
            }
            
        }
        
        float minValue = float.MaxValue;
        float maxValue = float.MinValue;

        if (useFalloff)
        {
            if (falloffMap == null)
            {
                falloffMap = FalloffGenerator.GenerateFalloffMap(mapSize);
            }
        }

        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                for (int k = 0; k < 9; k++)
                {
                    if (maskToUse[k])
                    {
                        values[i, j] += biomeMask.mask[k].values[i, j] * noiseMaps[k][i, j] * biomes[k].biomeSettings.heightMapSettings.heightMultiplier * heightCurves[k].Evaluate(noiseMaps[k][i, j] - (useFalloff ? falloffMap[i, j] : 0));

                    }
                }

                if (values[i, j] > maxValue)
                {
                    maxValue = values[i, j];
                }
                if (values[i, j] < minValue)
                {
                    minValue = values[i, j];
                }
            }
        }

        return new HeightMap(values, minValue, maxValue);
    }


    public static HeightMap GenerateHeightMapFromVertexPos(Vector3[] vertexPos, List<Chunk> neighbourChunks, bool useFalloff )
    {
        int numVerticesPerLine = (int)Mathf.Sqrt(vertexPos.Length);
        float[,] values = new float[numVerticesPerLine, numVerticesPerLine];

        if (useFalloff)
        {
            if (falloffMap == null)
            {
                falloffMap = FalloffGenerator.GenerateFalloffMap(numVerticesPerLine);
            }
        }

        for (int y = 0; y < numVerticesPerLine; y++)
        {
            for (int x = 0; x < numVerticesPerLine; x++)
            {


                //Vector2 sphericalCoordinate = CoordinateHelper.MapToSphericalCoordinate(new Vector2Int(x - 1, y - 1), neighbourChunks[0].chunkCenter, numVerticesPerLine, neighbourChunks[0].localUp, neighbourChunks[0].axisA, neighbourChunks[0].axisB, neighbourChunks[0].parentWorld.chunksPerFaces);
                Vector2 sphericalCoordinate = CoordinateHelper.CartesianToSphericalCoordinate(vertexPos[y * numVerticesPerLine + x]);
                Vector2 seemlessCoordinate = new Vector2(Mathf.Abs(sphericalCoordinate.x), Mathf.Abs(sphericalCoordinate.y));
                for (int k = 0; k < neighbourChunks.Count; k++)
                {
                    //values[x, y] = Noise.ShowCoordinates(seemlessCoordinate);
                    values[x, y] += Noise.GenerateNoisePoint(seemlessCoordinate, neighbourChunks[k].biome.biomeSettings.heightMapSettings.noiseSettings) * neighbourChunks[0].biome.biomeMask.mask[k].values[x, y];// neighbourChunks[0].biome.biomeMask.mask[k].values[x, y] * neighbourChunks[k].biome.noiseMap[x, y] * neighbourChunks[k].biome.biomeSettings.heightMapSettings.heightMultiplier; // * neighbourChunks[k].biome.heightCurve.Evaluate(neighbourChunks[k].biome.noiseMap[x, y] - (useFalloff ? falloffMap[x,y] : 0));
                }

            }
        }
        
        return new HeightMap(values, 0, 1);
    }
}

public struct HeightMap
{
    public float[,] values;
    public float minValue;
    public float maxValue;

    public HeightMap(float[,] values, float minValue, float maxValue)
    {
        this.values = values;
        this.minValue = minValue;
        this.maxValue = maxValue;
    }

    public HeightMap Zero(int mapSize)
    {
        return (new HeightMap(new float[mapSize, mapSize], 0, 1));
    }

}