using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk :MonoBehaviour
{

    [Range(0, 8)]
    public int maskIndex = 0;
    Vector2Int coordinate;
    Vector2 chunkCenter;
    public Biome biome;
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
    Vector3 projectedcenter;
    public Vector3 biomeProjectedCenter;
    public float latitude;
    public float longitude;
    List<Chunk> neighbourChunks;
    public MissingCorner missingCorner = MissingCorner.None;
    bool[] maskToUse;
    public bool showNeighbours;
    HeightMap heightMap;
    

    public void InitializeChunck(Vector2Int coordinate)
    {
        parentFace = GetComponentInParent<PlanetFace>();
        parentWorld = parentFace.parentWorld;
        Material material = Resources.Load("materialPlanet", typeof(Material)) as Material;
        this.coordinate = coordinate;
        neighbourChunks = new List<Chunk>();
        heightMap = new HeightMap();


        //Temp test
        texture = TextureGenerator.TextureFromHeightMap(parentWorld.heightMap);
        texture = Resources.Load("sky", typeof(Texture2D)) as Texture2D;


        meshSettings = parentWorld.meshSettings;
        chunkCenter = new Vector2(1f / (2f * parentWorld.chunksPerFaces) + coordinate.x * (1f / (float)parentWorld.chunksPerFaces), 1f / (2f * (float)parentWorld.chunksPerFaces) + coordinate.y * (1f / (float)parentWorld.chunksPerFaces));
        possibleBiomeSettings = parentWorld.biomesSettings;
        
        localUp = parentFace.localUp;
        axisA = parentFace.axisA;
        axisB = parentFace.axisB;

        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = material;

        projectedcenter = MeshGenerator.ProjectPointSphere(new Vector2Int((meshSettings.numVertsPerLine - 3) / 2, (meshSettings.numVertsPerLine - 3) / 2), chunkCenter, meshSettings.numVertsPerLine, localUp, axisA, axisB, parentWorld.planetRadius, parentWorld.chunksPerFaces);

        Vector2 sphericalCoordinate = CoordinateHelper.MapToSphericalCoordinate(new Vector2Int((meshSettings.numVertsPerLine - 3) / 2, (meshSettings.numVertsPerLine - 3) / 2), chunkCenter, meshSettings.numVertsPerLine, localUp, axisA, axisB, parentWorld.chunksPerFaces);

        longitude = sphericalCoordinate.x;
        latitude = sphericalCoordinate.y;
        AttachRandomBiome();

    }

    public void CreateMesh()
    {
        HeightMap heightMap = new HeightMap(new float[meshSettings.numVertsPerLine, meshSettings.numVertsPerLine], 0, 1);
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(heightMap, meshSettings, 0, localUp, axisA, axisB, chunkCenter, parentWorld.planetRadius, parentWorld.chunksPerFaces, parentWorld.terrainShape);
        meshFilter.sharedMesh = meshData.CreateMesh();
        meshRenderer.sharedMaterial.mainTexture = texture;
    }
   

    public float CalculateNoiseAtPoint(int x, int y)
    {
        Vector2 sphericalCoordinate = CoordinateHelper.MapToSphericalCoordinate(new Vector2Int(x, y), chunkCenter, meshSettings.numVertsPerLine, localUp, axisA, axisB, parentWorld.chunksPerFaces);
        Vector2 seemlessCoordinate = new Vector2(Mathf.Abs(sphericalCoordinate.x), Mathf.Abs(sphericalCoordinate.y));
        


        return Noise.GenerateNoisePoint(seemlessCoordinate, biome.biomeSettings.heightMapSettings.noiseSettings);
        //return Noise.ShowCoordinates(seemlessCoordinate);
    }


    public void RegisterNeighbours()
    {
        //enregistrement 0,0
        neighbourChunks.Add(this);

        //enregistrement voisin x-1 y-1
        if (coordinate.y != 0 || coordinate.x != 0)
        {
            if (coordinate.x == 0)
            {
                if (parentFace.isInvertedFace)
                {
                    neighbourChunks.Add(parentFace.neighbourFaces[1].chunks[parentWorld.chunksPerFaces - coordinate.y, parentWorld.chunksPerFaces - 1]);
                }
                else
                {
                    neighbourChunks.Add(parentFace.neighbourFaces[1].chunks[coordinate.y - 1, 0]);
                }

            }
            else if (coordinate.y == 0)
            {
                if (parentFace.isInvertedFace)
                {
                    neighbourChunks.Add(parentFace.neighbourFaces[3].chunks[0, coordinate.x - 1]);
                }
                else
                {
                    neighbourChunks.Add(parentFace.neighbourFaces[3].chunks[parentWorld.chunksPerFaces - 1, parentWorld.chunksPerFaces - coordinate.x]);
                }
            }
            else
            {
                neighbourChunks.Add(parentFace.chunks[coordinate.x - 1, coordinate.y - 1]);
            }
        }
        else
        {
            
            missingCorner = MissingCorner.BottomLeft;
        }


        //enregistrement voisin y-1
        if (coordinate.y == 0)
        {
            if (parentFace.isInvertedFace)
            {
                neighbourChunks.Add(parentFace.neighbourFaces[3].chunks[0, coordinate.x]);
            }
            else
            {
                neighbourChunks.Add(parentFace.neighbourFaces[3].chunks[parentWorld.chunksPerFaces - 1, parentWorld.chunksPerFaces - 1 - coordinate.x]);
            }
        }
        else
        {
            neighbourChunks.Add(parentFace.chunks[coordinate.x, coordinate.y - 1]);
        }


        //enregistrement x+1 y-1
        if (coordinate.y != 0 || coordinate.x != parentWorld.chunksPerFaces - 1)
        {
            if (coordinate.x == parentWorld.chunksPerFaces - 1)
            {
                if (parentFace.isInvertedFace)
                {
                    neighbourChunks.Add(parentFace.neighbourFaces[0].chunks[coordinate.y - 1, parentWorld.chunksPerFaces - 1]);
                }
                else
                {
                    neighbourChunks.Add(parentFace.neighbourFaces[0].chunks[parentWorld.chunksPerFaces - coordinate.y, 0]);
                }
            }
            else if (coordinate.y == 0)
            {
                if (parentFace.isInvertedFace)
                {
                    neighbourChunks.Add(parentFace.neighbourFaces[3].chunks[0, coordinate.x + 1]);
                }
                else
                {
                    neighbourChunks.Add(parentFace.neighbourFaces[3].chunks[parentWorld.chunksPerFaces - 1, parentWorld.chunksPerFaces - 2 - coordinate.x]);
                }
            }
            else
            {
                neighbourChunks.Add(parentFace.chunks[coordinate.x + 1, coordinate.y - 1]);
            }
        }
        else
        {
            
            missingCorner = MissingCorner.TopLeft;
        }


        //enregistrement voisin x-1
        if (coordinate.x == 0)
        {
            if (parentFace.isInvertedFace)
            {
                neighbourChunks.Add(parentFace.neighbourFaces[1].chunks[parentWorld.chunksPerFaces - 1 - coordinate.y, parentWorld.chunksPerFaces - 1]);
            }
            else
            {
                neighbourChunks.Add(parentFace.neighbourFaces[1].chunks[coordinate.y, 0]);
            }
        }
        else
        {
            neighbourChunks.Add(parentFace.chunks[coordinate.x - 1, coordinate.y]);
        }



        //enregistrement voisin x+1
        if (coordinate.x == parentWorld.chunksPerFaces - 1)
        {
            if (parentFace.isInvertedFace)
            {
                neighbourChunks.Add(parentFace.neighbourFaces[0].chunks[coordinate.y, parentWorld.chunksPerFaces - 1]);
            }
            else
            {
                neighbourChunks.Add(parentFace.neighbourFaces[0].chunks[parentWorld.chunksPerFaces - 1 - coordinate.y, 0]);
            }
        }
        else
        {
            neighbourChunks.Add(parentFace.chunks[coordinate.x + 1, coordinate.y]);
        }


        //enregistrement voisin x-1 y+1
        if (coordinate.y != parentWorld.chunksPerFaces - 1 || coordinate.x != 0)
        {
            if (coordinate.x == 0)
            {
                if (parentFace.isInvertedFace)
                {
                    neighbourChunks.Add(parentFace.neighbourFaces[1].chunks[parentWorld.chunksPerFaces - 2 - coordinate.y, parentWorld.chunksPerFaces - 1]);
                }
                else
                {
                    neighbourChunks.Add(parentFace.neighbourFaces[1].chunks[coordinate.y + 1, 0]);
                }

            }
            else if (coordinate.y == parentWorld.chunksPerFaces - 1)
            {
                if (parentFace.isInvertedFace)
                {
                    neighbourChunks.Add(parentFace.neighbourFaces[2].chunks[parentWorld.chunksPerFaces - 1, coordinate.x - 1]);
                }
                else
                {
                    neighbourChunks.Add(parentFace.neighbourFaces[2].chunks[0, parentWorld.chunksPerFaces - coordinate.x]);
                }
            }
            else
            {
                neighbourChunks.Add(parentFace.chunks[coordinate.x - 1, coordinate.y + 1]);
            }
        }
        else
        {
            
            missingCorner = MissingCorner.BottomRight;
        }
       

        //enregistrement voisin y+1
        if (coordinate.y == parentWorld.chunksPerFaces - 1)
        {
            if (parentFace.isInvertedFace)
            {
                neighbourChunks.Add(parentFace.neighbourFaces[2].chunks[parentWorld.chunksPerFaces - 1, coordinate.x]);
            }
            else
            {
                neighbourChunks.Add(parentFace.neighbourFaces[2].chunks[0, parentWorld.chunksPerFaces - 1 - coordinate.x]);
            }
        }
        else
        {
            neighbourChunks.Add(parentFace.chunks[coordinate.x, coordinate.y + 1]);
        }


        //enregistrement voisin x+1 y+1
        if (coordinate.y != parentWorld.chunksPerFaces - 1 || coordinate.x != parentWorld.chunksPerFaces - 1)
        {
            if (coordinate.x == parentWorld.chunksPerFaces - 1)
            {
                if (parentFace.isInvertedFace)
                {
                    neighbourChunks.Add(parentFace.neighbourFaces[0].chunks[coordinate.y+1, parentWorld.chunksPerFaces - 1]);
                }
                else
                {
                    neighbourChunks.Add(parentFace.neighbourFaces[0].chunks[parentWorld.chunksPerFaces - 2 - coordinate.y, 0]);
                }
            }
            else if (coordinate.y == parentWorld.chunksPerFaces - 1)
            {
                if (parentFace.isInvertedFace)
                {
                    neighbourChunks.Add(parentFace.neighbourFaces[2].chunks[parentWorld.chunksPerFaces - 1, coordinate.x+1]);
                }
                else
                {
                    neighbourChunks.Add(parentFace.neighbourFaces[2].chunks[0, parentWorld.chunksPerFaces - 2 - coordinate.x]);
                }
            }
            else
            {
                neighbourChunks.Add(parentFace.chunks[coordinate.x + 1, coordinate.y + 1]);
            }
        }
        else
        {
            
            missingCorner = MissingCorner.TopRight;
        }



        CreateMaskToUse(missingCorner);

    }


    public void AddBiomeMask()
    {
        
        biome.CreateBiomeMask(meshFilter.sharedMesh, neighbourChunks, parentWorld.biomeBlend);
    }

    public void AddHeightMap()
    {
        heightMap = HeightMapGenerator.GenerateHeightMapFromMesh(meshFilter.sharedMesh, neighbourChunks, parentWorld.useFalloff);
    }

    [ContextMenu("Draw noise map")]
    public void DrawNoiseMap()
    {
        if (parentWorld.drawMode == DrawMode.Mask)
        {
            meshRenderer.sharedMaterial = new Material(Shader.Find("Unlit/Texture"));
            texture = TextureGenerator.TextureFromHeightMap(biome.biomeMask.mask[maskIndex]);
            meshRenderer.sharedMaterial.mainTexture = texture;
            
        }

        if (parentWorld.drawMode == DrawMode.NoiseMap)
        {
            meshRenderer.sharedMaterial = new Material(Shader.Find("Unlit/Texture"));
            texture = TextureGenerator.TextureFromHeightMap(heightMap);
            meshRenderer.sharedMaterial.mainTexture = texture;
        }

    }

    public void CreateMaskToUse(MissingCorner missingCorner)
    {
        maskToUse = new bool[9] { true, true, true, true, true, true, true, true, true };

        switch (missingCorner)
        {
            case MissingCorner.TopLeft:
                maskToUse[2] = false;
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
    }

    public void AttachRandomBiome()
    {
        biome = new Biome(meshSettings.numVertsPerLine, possibleBiomeSettings[Random.Range(0, possibleBiomeSettings.Length)]);
        biomeProjectedCenter = MeshGenerator.ProjectPointSphere(biome.biomeCenter, chunkCenter, meshSettings.numVertsPerLine, localUp, axisA, axisB, parentWorld.planetRadius, parentWorld.chunksPerFaces);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0);
        if (biome!=null)
        {
            Gizmos.DrawSphere(biomeProjectedCenter, 1f);

            Gizmos.color = new Color(0, 0, 1);
            Gizmos.DrawSphere(projectedcenter, 1f);

            if (showNeighbours)
            {
                Gizmos.color = new Color(0, 1, 0);
                foreach (Chunk chunk in neighbourChunks)
                {
                    Gizmos.DrawSphere(chunk.projectedcenter, 1f);
                }
                
                
            }
        }
    }
}





