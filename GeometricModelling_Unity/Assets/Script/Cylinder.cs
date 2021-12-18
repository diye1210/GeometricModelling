using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Cylinder 
{
    public Vector3 Pt1 { get; }
    public Vector3 Pt2 { get; }
    float Radius { get; }

    public Cylinder(Vector3 pt1, Vector3 pt2, float radius)
    {
        Pt1 = pt1; ;
        Pt2 = pt2;
        Radius = radius; 
    }
}
