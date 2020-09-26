﻿using UnityEngine;
using System.Collections;

[CreateAssetMenu()]
public class MeshSettings : UpdatableData
{

    public const int numSupportedLODs = 5;
    public const int numSupportedChunkSizes = 10;
    public const int numSupportedFlatshadedChunkSizes = 3;
    public static readonly int[] supportedChunkSizes = {24 ,48, 72, 96, 120, 144, 168, 192, 216, 240 };

    public float meshScale = 1f;
    public bool useFlatShading;

    [Range(0, numSupportedChunkSizes - 1)]
    public int chunkSizeIndex;
    [Range(0, numSupportedFlatshadedChunkSizes - 1)]
    public int flatshadedChunkSizeIndex;
    [SerializeField]
    private int sizeMesh;


    // num verts per line of mesh rendered at LOD = 0. Includes the 2 extra verts that are excluded from final mesh, but used for calculating normals
    
    public int numVertsPerLine
    {
        get
        {
            sizeMesh= supportedChunkSizes[(useFlatShading) ? flatshadedChunkSizeIndex : chunkSizeIndex] + 5;
            return sizeMesh;
        }
    }

    public float meshWorldSize
    {
        get
        {
            return (numVertsPerLine - 3) * meshScale;
        }
    }


}