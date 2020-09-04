using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapPreview : MonoBehaviour
{

    public Renderer textureRender;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    [Range(0, 8)]
    public int maskVisibleIndex = 0;
    public float displayRadius = 1;
    public bool fixePoint = true;

    public enum DrawMode { NoiseMap, Mask, Mesh, FalloffMap };

    public enum MissingCorner {All, TopLeft, TopRight, BottomLeft, BottomRight, None}

    public MissingCorner missingCorner;
    public DrawMode drawMode;

    public MeshSettings meshSettings;
    public HeightMapSettings heightMapSettings;
    public TextureData textureData;

    public Material terrainMaterial;



    [Range(0, MeshSettings.numSupportedLODs - 1)]
    public int editorPreviewLOD;
    public bool autoUpdate;
    public BiomeSettings[] biomesSettings;
    public float blend;
    [Range(0,8)]
    public int maskIndex=0;
    List<Biome> biomes;
    public bool useFalloff;

    bool[] maskToUse = new bool[9];


    public void DrawMapInEditor()
    {
        transform.localScale = new Vector3(1, 1, 1);
        transform.position = new Vector3(0, 0, 0);
        if (!fixePoint || biomes==null)
        {
            biomes = BiomeGenerator.InitializeBiome(meshSettings.numVertsPerLine, biomesSettings);
        }
        
        BiomeMask biomeMask = BiomeGenerator.GenerateBiomes(meshSettings.numVertsPerLine,biomes,blend, maskToUse);
        HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(meshSettings.numVertsPerLine, biomes, biomeMask, Vector2.zero, useFalloff, maskToUse);

        if (drawMode == DrawMode.NoiseMap)
        {
            DrawTexture(TextureGenerator.TextureFromHeightMap(heightMap));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            DrawMesh(MeshGenerator.GenerateTerrainMesh(heightMap, meshSettings, editorPreviewLOD, Vector3.up, Vector2.zero, 1f, 1, MeshGenerator.TerrainShape.cube));

            transform.localScale = Vector3.one * (meshSettings.numVertsPerLine) / 2;
            transform.position = new Vector3(0, -(meshSettings.numVertsPerLine) / 2, 0);

        }
        else if (drawMode == DrawMode.FalloffMap)
        {
           DrawTexture(TextureGenerator.TextureFromHeightMap(new HeightMap(FalloffGenerator.GenerateFalloffMap(meshSettings.numVertsPerLine), 0, 1)));
        }
        else if(drawMode== DrawMode.Mask)
        {
            DrawTexture(TextureGenerator.TextureFromHeightMap(biomeMask.mask[maskIndex]));
        }
    }


    public void DrawTexture(Texture2D texture)
    {
        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height) / 10f;

        textureRender.gameObject.SetActive(true);
        meshFilter.gameObject.SetActive(false);
    }

    public void DrawMesh(MeshData meshData)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();

        textureRender.gameObject.SetActive(false);
        meshFilter.gameObject.SetActive(true);
    }

    void OnDrawGizmos()
    {
        int mapLength = meshSettings.numVertsPerLine;
        

        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(mapLength, 0, mapLength));
        if (biomes != null)
        {
            for (int i = 0; i <9; i++)
            {
                if (maskToUse[i])
                {
                    Biome biome = biomes[i];
                    Gizmos.color = new Color(1, 0, 0, 0.5f);
                    Gizmos.DrawSphere(new Vector3(biome.biomeCenter.x, 0, biome.biomeCenter.y) - new Vector3(mapLength, 0, mapLength) / 2, displayRadius * 2.5f);
                    Gizmos.color = Color.grey;
                    Gizmos.DrawSphere(new Vector3(biome.biomeCenter.x, 0, biome.biomeCenter.y) - new Vector3(mapLength, 0, mapLength) / 2, displayRadius);

                }

            }
           
        }
    }

    void OnValuesUpdated()
    {
        if (!Application.isPlaying)
        {
            DrawMapInEditor();
        }
    }

    void OnTextureValuesUpdated()
    {
        textureData.ApplyToMaterial(terrainMaterial);
    }

    void OnValidate()
    {
        maskToUse = new bool[9] { true,true, true, true, true, true, true, true, true };
       
        switch (missingCorner)
        {
            case MissingCorner.TopLeft: maskToUse[2] = false;
                break;
            case MissingCorner.TopRight:
                maskToUse[8] = false;
                break;
            case MissingCorner.BottomLeft:
                maskToUse[0] = false;
                break;
            case MissingCorner.BottomRight:
                maskToUse[6] = false;
                break;
            case MissingCorner.None:
                maskToUse[0] = false;
                maskToUse[2] = false;
                maskToUse[6] = false;
                maskToUse[8] = false;
                break;
            case MissingCorner.All:
                break;
            default:
                break;
        }

        if (meshSettings != null)
        {
            meshSettings.OnValuesUpdated -= OnValuesUpdated;
            meshSettings.OnValuesUpdated += OnValuesUpdated;
        }
        if (heightMapSettings != null)
        {
            heightMapSettings.OnValuesUpdated -= OnValuesUpdated;
            heightMapSettings.OnValuesUpdated += OnValuesUpdated;
        }
        if (textureData != null)
        {
            textureData.OnValuesUpdated -= OnTextureValuesUpdated;
            textureData.OnValuesUpdated += OnTextureValuesUpdated;
        }

    }

}
