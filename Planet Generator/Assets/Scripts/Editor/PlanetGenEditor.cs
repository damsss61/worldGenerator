using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(WorldGenerator))]
public class PlanetGenEditor : Editor
{

    public override void OnInspectorGUI()
    {
        WorldGenerator worldGen = (WorldGenerator)target;

        if (DrawDefaultInspector())
        {
            /*if (worldGen.autoUpdate)
            {
                worldGen.DrawMapInEditor();
            }*/
        }

        if (GUILayout.Button("Generate"))
        {
            worldGen.GenerateWorld();
        }
    }


}
   
        
   