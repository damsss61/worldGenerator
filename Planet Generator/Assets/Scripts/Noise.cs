﻿using UnityEngine;
using System.Collections;

public static class Noise
{

    public enum NormalizeMode { Local, Global };

    public static float[,] GenerateNoiseMap(int mapWidth, NoiseSettings settings, Vector2 sampleCentre)
    {
        float[,] noiseMap = new float[mapWidth, mapWidth];

        System.Random prng = new System.Random(settings.seed);
        Vector2[] octaveOffsets = new Vector2[settings.octaves];

        float maxPossibleHeight = 0f;
        float amplitude = 1f;
        float frequency = 1f;

        for (int i = 0; i < settings.octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + settings.offset.x + sampleCentre.x;
            float offsetY = prng.Next(-100000, 100000) - settings.offset.y - sampleCentre.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= settings.persistance;
        }

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;


        for (int y = 0; y < mapWidth; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {

                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < settings.octaves; i++)
                {
                    float sampleX = (x - halfWidth + octaveOffsets[i].x) / settings.scale * frequency;
                    float sampleY = (y - halfWidth + octaveOffsets[i].y) / settings.scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= settings.persistance;
                    frequency *= settings.lacunarity;
                }

                if (noiseHeight > maxLocalNoiseHeight)
                {
                    maxLocalNoiseHeight = noiseHeight;
                }
                if (noiseHeight < minLocalNoiseHeight)
                {
                    minLocalNoiseHeight = noiseHeight;
                }
                noiseMap[x, y] = noiseHeight;

                if (settings.normalizeMode == NormalizeMode.Global)
                {
                    float normalizedHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight / 0.9f);
                    noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                }
            }
        }

        if (settings.normalizeMode == NormalizeMode.Local)
        {
            for (int y = 0; y < mapWidth; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
                }
            }
        }

        return noiseMap;
    }
    public static float GenerateNoisePoint(Vector2 worldPos, NoiseSettings settings)
    {
        float noiseValue=0;

        System.Random prng = new System.Random(settings.seed);
        Vector2 octaveOffsets;

        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;

        for (int i = 0; i < settings.octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + settings.offset.x;
            float offsetY = prng.Next(-100000, 100000) - settings.offset.y;
            octaveOffsets = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= settings.persistance;

            float sampleX = (worldPos.x + octaveOffsets.x) / settings.scale * frequency;
            float sampleY = (worldPos.y + octaveOffsets.y) / settings.scale * frequency;

            float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
            noiseValue += perlinValue * amplitude;

            amplitude *= settings.persistance;
            frequency *= settings.lacunarity;
        }


        float normalizedHeight = (noiseValue) / (maxPossibleHeight / 0.8f);
        noiseValue = Mathf.Clamp(normalizedHeight, 0, int.MaxValue)*settings.multiplier;

        return noiseValue;
    }

    public static float ShowCoordinates(Vector2 worldPos)
    {

        return (worldPos.x/360 +worldPos.y/180f);
    }

}