using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Classe contenant les structures des différentes formes utilisées
/// </summary>
public static class FormClass
{
    public struct Plane
    {
        public Vector3 Normal;
        public float d;
    }

    public struct Segment
    {
        public Vector3 pt1;
        public Vector3 pt2;
    }

    public struct Sphere
    {
        public Vector3 center;
        public float radius;
    }

    public struct Cylinder
    {
        public Vector3 pt1;
        public Vector3 pt2;
        public float radius;
    }
 
}
