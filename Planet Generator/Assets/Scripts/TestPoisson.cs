using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPoisson : MonoBehaviour
{

    public float radius = 1;
    public float mapLength = 1;
    public float mapWidth = 1;
    public int rejectionSamples = 30;
    public float displayRadius = 1;

    List<Vector2> points;

    void OnValidate()
    {
        points = PoissonSampler.GenerateSamplePositions(radius, new Vector2(mapLength, mapWidth), rejectionSamples);
    }

    void OnDrawGizmos()
    {
        
        Gizmos.DrawWireCube(new Vector2(mapLength,mapWidth) / 2, new Vector2(mapLength, mapWidth));
        if (points != null)
        {
            foreach (Vector2 point in points)
            {
                Gizmos.DrawSphere(point, displayRadius);
            }
        }
    }
}
