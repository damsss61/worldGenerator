using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HeightMapGenerator
{

    static float[,] falloffMap;
    public static HeightMap GenerateHeightMap(int width, List<Biome> biomes, BiomeMask biomeMask, Vector2 sampleCentre, bool useFalloff, bool[] maskToUse)
    {
        float[,] values = new float[width, width];
        List<float[,]> noiseMaps = new List<float[,]>(9);
        List<AnimationCurve> heightCurves = new List<AnimationCurve>(9);
        for (int i = 0; i < 9; i++)
        {
            if (maskToUse[i])
            {
                noiseMaps.Add(Noise.GenerateNoiseMap(width, biomes[i].biomeSettings.heightMapSettings.noiseSettings, sampleCentre));
                heightCurves.Add(new AnimationCurve(biomes[i].biomeSettings.heightMapSettings.heightCurve.keys));
            }
            else
            {
                noiseMaps.Add(new float[width, width]);
                heightCurves.Add(new AnimationCurve());
            }
            
        }
        
        float minValue = float.MaxValue;
        float maxValue = float.MinValue;

        if (useFalloff)
        {
            if (falloffMap == null)
            {
                falloffMap = FalloffGenerator.GenerateFalloffMap(width);
            }
        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < width; j++)
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
}