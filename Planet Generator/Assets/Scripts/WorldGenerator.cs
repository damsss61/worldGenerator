using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [Range(1, 5)]
    public int chunksPerFaces = 1;


    [SerializeField, HideInInspector]
    public List<PlanetFace> terrainFaces;


    public bool useFalloff;
    public BiomeSettings[] biomesSettings;

    public float planetRadius = 10f;

    public MeshSettings meshSettings;
    public TextureData textureData;

    [Range(1, 6)]
    public int nbFaces;
    public HeightMap heightMap;
    public DrawMode drawMode = DrawMode.Mask;
    public MeshGenerator.TerrainShape terrainShape = MeshGenerator.TerrainShape.sphere;
    public float biomeBlend = 3f;

    public Material worldMaterial;

    public bool autoUpdate;




    private void Start()
    {
       /* DeleteFaces();
        Initialize();
        ComputeAllSphereVertexPos();*/
    }


    public void GenerateWorld()
    {
        DeleteFaces();
        Initialize();
        ComputeAllSphereVertexPos();
        RegisterChunksNeighbours();
        GenerateBiomeMask();
        GenerateHeightMap();
        DrawWorld();
    }

    public void UpdateTerrain()
    {
        GenerateHeightMap();
        DrawWorld();
    }

    void Initialize()
    {

        terrainFaces = new List<PlanetFace>(6);


        Vector3[] directions = { Vector3.up, Vector3.left, Vector3.forward, Vector3.down, Vector3.right, Vector3.back };

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

    void GenerateBiomeMask()
    {
        foreach (PlanetFace face in terrainFaces)
        {
            face.ConstructAllBiomeMask();
        }
    }

    void GenerateHeightMap()
    {
        foreach (PlanetFace face in terrainFaces)
        {
            face.ConstructAllHeightMap();
        }
    }

    void DrawWorld()
    {
        foreach (PlanetFace face in terrainFaces)
        {
            face.DrawAllMap();
        }
    }

    void ComputeAllSphereVertexPos()
    {

        foreach (PlanetFace face in terrainFaces)
        {
            face.ComputeAllSphereVertexPos();
        }
    }

    void RegisterChunksNeighbours()
    {
        foreach (PlanetFace face in terrainFaces)
        {
            face.RegisterChunksNeighBours();
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

    void OnValuesUpdated()
    {
        if (!Application.isPlaying)
        {
            if (terrainFaces != null)
            {
                UpdateTerrain();
            }
            
        }
    }

    void OnValidate()
    {
        if (meshSettings != null)
        {
            meshSettings.OnValuesUpdated -= OnValuesUpdated;
            meshSettings.OnValuesUpdated += OnValuesUpdated;
        }
        if (biomesSettings != null)
        {
            foreach (BiomeSettings biomeSettings in biomesSettings)
            {
                biomeSettings.heightMapSettings.OnValuesUpdated -= OnValuesUpdated;
                biomeSettings.heightMapSettings.OnValuesUpdated += OnValuesUpdated;
            }
            
        }

    }
}


