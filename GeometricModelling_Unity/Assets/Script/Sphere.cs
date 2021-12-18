using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Sphere 
{
    public Vector3 Center { get; }
    public float Radius { get; }

    public Sphere(Vector3 center, float radius)
    {
        Center = center;
        Radius = radius;
    }

}
