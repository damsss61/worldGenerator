using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Chunk))]
public class ChunkEditor : Editor
{

    public override void OnInspectorGUI()
    {
        Chunk chunck = (Chunk)target;

       
    }
}