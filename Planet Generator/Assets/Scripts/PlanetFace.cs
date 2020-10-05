using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetFace : MonoBehaviour
{


    public Vector3 localUp;
    public Vector3 axisA;
    public Vector3 axisB;

    int chunksPerFaces;
    public WorldGenerator parentWorld;
    public Chunk[,] chunks;
    public List<PlanetFace> neighbourFaces;
    public bool isInvertedFace;
    

    public void InitializeFace( Vector3 localUp, HeightMap heightMap)
    {
        parentWorld = GetComponentInParent<WorldGenerator>();
        this.localUp = localUp;
        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        axisB = Vector3.Cross(localUp, axisA);
        chunksPerFaces = parentWorld.chunksPerFaces;

        if (localUp.x==-1 || localUp.y == -1 || localUp.z==-1)
        {
            isInvertedFace = true; 
        }
        else
        {
            isInvertedFace = false;
        }

        if (chunks == null || chunks.Length == 0)
        {
            chunks = new Chunk[chunksPerFaces, chunksPerFaces];
        }
        
        for (int i = 0; i < chunksPerFaces; i++)
        {
            for (int j = 0; j < chunksPerFaces; j++)
            {
                
                if (chunks[i, j] == null)
                {
                    CreateChunk(i, j);
                }    
            }
        }

        neighbourFaces = new List<PlanetFace>();
        
    }

    public void RegisterNeighbours()
    {
        neighbourFaces.Add(parentWorld.terrainFaces.Find((x) => x.localUp == axisA));
        neighbourFaces.Add(parentWorld.terrainFaces.Find((x) => x.localUp == -axisA));
        neighbourFaces.Add(parentWorld.terrainFaces.Find((x) => x.localUp == axisB));
        neighbourFaces.Add(parentWorld.terrainFaces.Find((x) => x.localUp == -axisB));
    }
    public void CreateChunk(int i, int j)
    {
        GameObject chunckObj = new GameObject("chunk " + i + " " + j);
        chunckObj.transform.parent = this.transform;
        chunks[i,j] = chunckObj.AddComponent<Chunk>();
        chunks[i, j].InitializeChunck(new Vector2Int(i,j));
    }

    public void ComputeAllSphereVertexPos()
    {
        foreach (Chunk chunk in chunks)
        {
            chunk.ComputeSphereVertexPos();
        }
    }

    public void RegisterChunksNeighBours()
    {
        foreach (Chunk chunk in chunks)
        {
            chunk.RegisterNeighbours();
        }
    }

    public void ConstructAllBiomeMask()
    {
        foreach (Chunk chunk in chunks)
        {
            chunk.AddBiomeMask();
          
        }
    }

    public void ConstructAllHeightMap()
    {
        foreach (Chunk chunk in chunks)
        {
            chunk.AddHeightMap();
            chunk.DrawMap();
        }
    }

    public void DrawAllMap()
    {
        foreach (Chunk chunk in chunks)
        {
            chunk.DrawMap();
        }
    }

}