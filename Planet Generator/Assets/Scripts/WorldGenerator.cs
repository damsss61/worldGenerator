using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [Range(1, 3)]
    public int chunksPerFaces = 1;


    [SerializeField, HideInInspector]
    public List<PlanetFace> terrainFaces;

    List<Biome> biomes;
    public bool useFalloff;
    public BiomeSettings[] biomesSettings;
    public float blend;
    public float planetRadius=10f;

    public MeshSettings meshSettings;
    public HeightMapSettings heightMapSettings;
    public TextureData textureData;

    public bool displayNoiseMap;
    [Range(1,6)]
    public int nbFaces;
    public HeightMap heightMap;


    

    private void Start()
    {
        DeleteFaces();
        Initialize();
        GenerateMesh();
    }


    public void GenerateWorld()
    {
        DeleteFaces();
        Initialize();
        GenerateMesh();
    }


    void Initialize()
    {

        terrainFaces =  new List<PlanetFace>(6);
        bool[] maskToUse = new bool[9] { true, true, true, true, true, true, true, true, true };
        biomes = BiomeGenerator.InitializeBiome(meshSettings.numVertsPerLine, biomesSettings);
        BiomeMask biomeMask = BiomeGenerator.GenerateBiomes(meshSettings.numVertsPerLine, biomes, blend, maskToUse);
        heightMap = HeightMapGenerator.GenerateHeightMap(meshSettings.numVertsPerLine, biomes, biomeMask, Vector2.zero, useFalloff, maskToUse);

        

        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        for (int i = 0; i < nbFaces; i++)
        {
            GameObject faceObj = new GameObject("Face");
            faceObj.transform.parent = this.transform;
            PlanetFace face = faceObj.AddComponent<PlanetFace>();
            terrainFaces.Add(face);
            terrainFaces[i].InitializeFace(directions[i], heightMap);
        }
        for (int i = 0; i < nbFaces; i++)
        {
            
            terrainFaces[i].RegisterNeighbours();
        }

    }

    void GenerateMesh()
    {
       
        foreach(PlanetFace face in terrainFaces)
        {
            if (!displayNoiseMap)
            {
                face.ConstructAllMeshes();
            }
            else
            {

                face.DisplayHeightMaps();
            }

            
        }
    }

    void DeleteFaces()
    {
        
        int nbChildren = this.transform.childCount;
        for (int i = 0; i < nbChildren; i++)
        {
            DestroyImmediate(this.transform.GetChild(0).gameObject);
        }
    }
}


