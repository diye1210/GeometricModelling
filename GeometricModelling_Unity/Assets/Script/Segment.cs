using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Segment 
{
    public Vector3 Pt1 { get; }
    public Vector3 Pt2 { get; }

    public Segment(Vector3 pt1, Vector3 pt2)
    {
        Pt1 = pt1; ;
        Pt2 = pt2;
    }
}
