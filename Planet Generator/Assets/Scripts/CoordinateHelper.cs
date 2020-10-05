using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoordinateHelper 
{
    public static Vector2 MapToSphericalCoordinate(Vector2Int mapCoordinate, Vector2 chunkCenter, int mapSize, Vector3 localUp, Vector3 axisA, Vector3 axisB, int chunksPerFaces)
    {
        Vector3 projectedCoordinate = ProjectMapPointOnUnitSphere(mapCoordinate, chunkCenter, mapSize, localUp, axisA, axisB, chunksPerFaces);

        return (CartesianToSphericalCoordinate(projectedCoordinate));
    }

    public static Vector3 ProjectMapPointOnUnitSphere(Vector2Int point, Vector2 chunkCenter, int mapSize, Vector3 localUp, Vector3 axisA, Vector3 axisB, int chunksPerFaces)
    {

        Vector2 percent = chunkCenter + new Vector2((float)point.x+1 - (mapSize - 3f) / 2, (float)point.y+1 - (mapSize - 3f) / 2) / ((mapSize - 5f) * chunksPerFaces);
        Vector3 pointOnUnitCube = localUp + (percent.x - 0.5f) * 2 * axisA + (percent.y - 0.5f) * 2 * axisB;
        
        return (pointOnUnitCube.normalized);
    }


    public static Vector2 CartesianToSphericalCoordinate(Vector3 cartesianCoordinate)
    {
        float radius = cartesianCoordinate.magnitude;
        float longitude = 180f / Mathf.PI * Mathf.Atan2(cartesianCoordinate.z, cartesianCoordinate.x);
        float latitude = 180f / Mathf.PI * Mathf.Asin(cartesianCoordinate.y/radius);

        return new Vector2(longitude, latitude);
    }
}
