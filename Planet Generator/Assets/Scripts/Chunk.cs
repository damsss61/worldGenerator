using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk :MonoBehaviour
{

    Vector2 chunkCenter;
    Biome biome;
    MeshSettings meshSettings;
    BiomeSettings[] possibleBiomeSettings;
    Vector3 localUp;
    Vector3 axisA;
    Vector3 axisB;
    PlanetFace parentFace;
    WorldGenerator parentWorld;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    Texture texture;

    public void InitializeChunck(Vector2 chunkCenter)
    {
        parentFace = GetComponentInParent<PlanetFace>();
        parentWorld = parentFace.parentWorld;
        Material material = Resources.Load("metal_blue_mat", typeof(Material)) as Material;


        //Temp test
        texture = TextureGenerator.TextureFromHeightMap(parentWorld.heightMap);


        meshSettings = parentWorld.meshSettings;
        this.chunkCenter = chunkCenter;
        possibleBiomeSettings = parentWorld.biomesSettings;
        
        localUp = parentFace.localUp;
        axisA = parentFace.axisA;
        axisB = parentFace.axisB;

        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = new Material(Shader.Find("Unlit/Texture"));
        
        AttachRandomBiome();

    }

    public void CreateMesh()
    {
        HeightMap heightMap = new HeightMap(new float[meshSettings.numVertsPerLine, meshSettings.numVertsPerLine], 0, 1);
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(heightMap, meshSettings, 0, localUp, axisA, axisB, chunkCenter, parentWorld.planetRadius, parentWorld.chunksPerFaces, MeshGenerator.TerrainShape.sphere);
        meshFilter.sharedMesh = meshData.CreateMesh();
        meshRenderer.sharedMaterial.mainTexture = texture;
    }
   


    public void AttachRandomBiome()
    {
        biome = new Biome(meshSettings.numVertsPerLine, possibleBiomeSettings[Random.Range(0, possibleBiomeSettings.Length)]);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0);
        if (biome!=null)
        {
            Gizmos.DrawSphere(MeshGenerator.ProjectPointSphere(biome.biomeCenter,chunkCenter,meshSettings.numVertsPerLine,localUp,axisA,axisB,parentWorld.planetRadius,parentWorld.chunksPerFaces), 1f);
        }
    }
}





