using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFace
{

    MeshFilter[] mesheFilters;
    int meshSize;
    Vector3 localUp;
    Vector3 axisA;
    Vector3 axisB;
    float planetRadius;
    MeshSettings meshSettings;
    int chunksPerFaces;
    BiomeSettings[] biomesSettings;
    Biome[] biomes;
    int lod = 0;
    float blend = 5;
    HeightMap heightMap;
    Transform face;
    MeshFilter[,] meshFilters;
    Material material;
    

    public TerrainFace(Transform face, MeshSettings meshSettings, Vector3 localUp, float planetRadius, int chunksPerFaces, BiomeSettings[] biomesSettings, HeightMap heightMap, Material material)
    {
        biomes = new Biome[chunksPerFaces* chunksPerFaces];
        this.face = face;
        this.meshSettings = meshSettings;
        this.localUp = localUp;
        this.chunksPerFaces = chunksPerFaces;
        this.planetRadius = planetRadius;
        this.biomesSettings = biomesSettings;
        this.meshSize = meshSettings.numVertsPerLine;
        this.heightMap = heightMap;
        this.material = material;

        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[chunksPerFaces , chunksPerFaces];
        }
        

        for (int i = 0; i < chunksPerFaces; i++)
        {
            for (int j = 0; j < chunksPerFaces; j++)
            {
                if (meshFilters[i,j]==null)
                {
                    GameObject meshObj = new GameObject("mesh" + i + " " + j);
                    meshObj.transform.parent = face;
                    meshObj.AddComponent<MeshRenderer>().sharedMaterial = material;
                    meshFilters[i,j]= meshObj.AddComponent<MeshFilter>();
                }
            }

        }


        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        axisB = Vector3.Cross(localUp, axisA);
    }

    public void ConstructAllMeshes()
    {
        for (int i = 0; i < chunksPerFaces; i++)
        {
            for (int j = 0; j < chunksPerFaces; j++)
            {
                HeightMap _heightMap = RequestHeightMap(i, j);
                MeshData meshData = MeshGenerator.GenerateTerrainMesh(_heightMap, meshSettings, lod, localUp, new Vector2(i * (meshSize - 3), j * (meshSize - 3)), planetRadius, chunksPerFaces, MeshGenerator.TerrainShape.sphere);
                meshFilters[i, j].sharedMesh = meshData.CreateMesh();
            }
            
        }

    }
    public Mesh ConstructMesh(Vector2 offset)
    {
        Vector3[] vertices = new Vector3[meshSize * meshSize];
        int[] triangles = new int[(meshSize - 1) * (meshSize - 1) * 6];
        int triIndex = 0;
        Mesh mesh = new Mesh();

        for (int y = 0; y < meshSize; y++)
        {
            for (int x = 0; x < meshSize; x++)
            {
                int i = x + y * meshSize;
                Vector2 percent = new Vector2(x+offset.x, y+offset.y) / ((meshSize-1)*chunksPerFaces);
                Vector3 pointOnUnitCube = localUp + (percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
                vertices[i] = pointOnUnitSphere* planetRadius * (1 + 0.1f*Mathf.InverseLerp(heightMap.minValue, heightMap.maxValue, heightMap.values[x, y]));

                if (x != meshSize - 1 && y != meshSize - 1)
                {
                    triangles[triIndex] = i;
                    triangles[triIndex + 1] = i + meshSize + 1;
                    triangles[triIndex + 2] = i + meshSize;

                    triangles[triIndex + 3] = i;
                    triangles[triIndex + 4] = i + 1;
                    triangles[triIndex + 5] = i + meshSize + 1;
                    triIndex += 6;
                }
            }
        }
        
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        return(mesh);
    }

    public void CreateChunks()
    {
        int mapSize = meshSettings.numVertsPerLine;
        for (int i = 0; i < chunksPerFaces; i++)
        {
            for (int j = 0; j < chunksPerFaces; j++)
            {
                int centerX = Mathf.RoundToInt(Random.Range((i + 0.2f) * mapSize, (i + 0.8f) * mapSize));
                int centerY = Mathf.RoundToInt(Random.Range((j + 0.2f) * mapSize, (j + 0.8f) * mapSize));

                Vector2 biomeCenter = new Vector2(centerX, centerY);
                
                biomes[i*chunksPerFaces +j] = new Biome(biomeCenter, biomesSettings[Random.Range(0, biomesSettings.Length)]);

            }

        }
    }
    
    public HeightMap RequestHeightMap(int i, int j)
    {
        bool[] maskToUse = new bool[9] { true, true, true, true, true, true, true, true, true };
        bool useFalloff = false;
        List<Biome> suroundingBiomes = BiomeGenerator.InitializeBiome(meshSettings.numVertsPerLine,biomesSettings);
        int biomeIndex = 0;
        if (i==0 && j==0)
        {
            maskToUse[0] = false;
        }
        if (i==chunksPerFaces-1 && j==0)
        {
            maskToUse[6] = false;
        }
        if (i==0 && j==chunksPerFaces-1)
        {
            maskToUse[2] = false;
        }
        if (i==chunksPerFaces-1 && j==chunksPerFaces-1)
        {
            maskToUse[8] = false;
        }

       
        
        for (int l = -1; l < 2; l++)
        {
            for (int k = -1; k < 2; k++)
            {
                int posX = i + l;
                int posY = j + k;
                if (posX>=0 && posY>=0 && posX<chunksPerFaces && posY<chunksPerFaces)
                {
                    suroundingBiomes[biomeIndex] = biomes[posX * chunksPerFaces + posY];
                }
                
                biomeIndex++;
            }

        }
        Vector2 biomePos = new Vector2(i * (meshSize-3), -j * (meshSize-3));
        BiomeMask biomeMask = BiomeGenerator.GenerateBiomes(meshSize, suroundingBiomes, blend, maskToUse);
        HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(meshSize, suroundingBiomes, biomeMask, biomePos, useFalloff, maskToUse);
        // Debug.Log("mask " + maskToUse[0] + maskToUse[1] + maskToUse[2] + maskToUse[3] + maskToUse[4] + maskToUse[5] + maskToUse[6] + maskToUse[7] + maskToUse[8]);
        return heightMap;
    }

    public void DisplayHeightMaps()
    {
        Renderer[,] renderers = new Renderer[chunksPerFaces, chunksPerFaces];
        for (int i = 0; i < chunksPerFaces; i++)
        {
            for (int j = 0; j < chunksPerFaces; j++)
            {

                Texture texture = TextureGenerator.TextureFromHeightMap(RequestHeightMap(i,j));
                GameObject planeObj = GameObject.CreatePrimitive(PrimitiveType.Plane);
                planeObj.transform.up = localUp;
                planeObj.transform.position = axisA * ((i-0.5f) * meshSize / 2) + axisB * ((j-0.5f) * meshSize / 2) + localUp * ((meshSize)/2+0.5f);
                planeObj.transform.localScale = Vector3.one * meshSize / (10f*chunksPerFaces);
                planeObj.transform.parent = face;

                renderers[i,j] = planeObj.GetComponent<Renderer>();
                renderers[i, j].sharedMaterial = new Material(Shader.Find("Unlit/Texture"));
                renderers[i,j].sharedMaterial.mainTexture = texture;


            }

        }
    }
}