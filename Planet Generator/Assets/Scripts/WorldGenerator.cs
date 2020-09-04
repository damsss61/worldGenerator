using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [Range(1, 3)]
    public int chunksPerFaces = 1;

    public Material material;

    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    TerrainFace[] terrainFaces;

    List<Biome> biomes;
    public bool useFalloff;
    public BiomeSettings[] biomesSettings;
    public float blend;
    public float planetRadius=10f;

    public MeshSettings meshSettings;
    public HeightMapSettings heightMapSettings;
    public TextureData textureData;

    private void Start()
    {
        Initialize();
        GenerateMesh();
    }

    void Initialize()
    {
        bool[] maskToUse = new bool[9] { true, true, true, true, true, true, true, true, true };
        biomes = BiomeGenerator.InitializeBiome(meshSettings.numVertsPerLine, biomesSettings);
        BiomeMask biomeMask = BiomeGenerator.GenerateBiomes(meshSettings.numVertsPerLine, biomes, blend, maskToUse);
        HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(meshSettings.numVertsPerLine, biomes, biomeMask, Vector2.zero, useFalloff, maskToUse);

        terrainFaces = new TerrainFace[6];

        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        for (int i = 0; i < 6; i++)
        {
            GameObject faceObj = new GameObject("Face");
            faceObj.transform.parent = transform;
            terrainFaces[i] = new TerrainFace(faceObj.transform, meshSettings, directions[i], planetRadius,chunksPerFaces, biomesSettings, heightMap, material);
        }
    }

    void GenerateMesh()
    {
        foreach (TerrainFace face in terrainFaces)
        {
            face.CreateChunks();

        }
        foreach(TerrainFace face in terrainFaces)
        {
            //face.ConstructAllMeshes();

            face.DisplayHeightMaps();
        }
    }
}


