using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Plane
{
    public Vector3 Normal { get; }
    public float D { get; }

    public Plane(Vector3 normal, float d)
    {
        Normal = normal;
        D = d;
    }

}
