using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainChunk 
{

    public Vector2 coord;
    public int numberBiomes;
    public HeightMap heightMap;
    public BiomeMask biomeMask;

    GameObject meshObject;
    Vector2 biomeCentre;

    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    MeshCollider meshCollider;


}
