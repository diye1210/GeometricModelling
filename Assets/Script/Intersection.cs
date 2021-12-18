using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GeometricClass;


public class Intersection 
{
    public static bool InterSegmentPlane(Segment seg, GeometricClass.Plane plane, out Vector3 interPt, out Vector3 interNormal)
    {
        //déclaration des variables
        Vector3 AB = seg.pt2 - seg.pt1;
        interPt = new Vector3();
        interNormal = new Vector3();
        float dotABn = Vector3.Dot(AB, plane.Normal);

        //algo segment plan 
        if (Mathf.Approximately(dotABn, 0))
            return false;

        float t = (plane.d - Vector3.Dot(seg.pt1, plane.Normal)) / dotABn;

        if (t < 0 || t > 1)
            return false;

        interPt = seg.pt1 + t * AB;

        if(dotABn <0)
            interNormal = plane.Normal;
        else
            interNormal = - plane.Normal;

        interNormal.Normalize();
        return true;
    } 

    public static bool InterSegmentSphere( Segment seg, GeometricClass.Sphere sphere, out Vector3 interPt, out Vector3 interNormal)
    {
        //déclaration des variables 
        Vector3 AB = seg.pt2 - seg.pt1;
        Vector3 OA = seg.pt1 - sphere.center;
        interPt = new Vector3();
        interNormal = new Vector3();
        float x, y, z, determinant; //variables eq second degré 
        float x1, x2; //solutions

        
        x = Vector3.Dot(AB, AB);
        y = 2f * Vector3.Dot(OA, AB);
        z = Vector3.Dot(OA, OA) - (sphere.radius * sphere.radius);
        determinant = y * y - 4 * x * z;

        //algo segment sphere
        if (determinant < 0)
            return false;

        x1 = (-y - Mathf.Sqrt(determinant)) / (2f * x);
        x2 = (-y + Mathf.Sqrt(determinant)) / (2f * x);

        if(x1 >= 0 && x1 <= 1)
        {
            interPt = seg.pt1 + x1 * AB;
            interNormal = interPt - sphere.center;
            interNormal.Normalize();
            return true;
        }

        if (x2 >= 0 && x2 <= 1)
        {
            interPt = seg.pt1 + x2 * AB;
            interNormal = -(interPt - sphere.center);
            interNormal.Normalize();
            return true;
        }

        return false;


    }

    public static bool InterSegmentCylinder(Segment seg, Cylinder cylinder, out Vector3 interPt, out Vector3 interNormal)
    {
        //déclaration des variables 
        Vector3 AB = seg.pt1 - seg.pt2;
        Vector3 PA = seg.pt1 - cylinder.pt1;
        Vector3 PQ = cylinder.pt2 - cylinder.pt1;
        Vector3 u = PQ / PQ.magnitude;
        interPt = new Vector3();
        interNormal = new Vector3();
        float x, y, z, determinant; //variables eq second degré 
        float x1, x2; //solutions


        x = Vector3.Dot(AB, AB) - 2 * Vector3.Dot(AB, Vector3.Dot(AB, PQ) / PQ.magnitude * u) + Mathf.Pow(Vector3.Dot(AB, PQ) / PQ.magnitude, 2) * Vector3.Dot(u, u);
        y = 2 * Vector3.Dot(AB, PA) - 4 * Vector3.Dot(AB, Vector3.Dot(PA, PQ) / PQ.magnitude * u) + 2 * Vector3.Dot(AB, PQ) * Vector3.Dot(PA, PQ) / Mathf.Pow(PQ.magnitude, 2) * Vector3.Dot(u, u);
        z = Vector3.Dot(PA, PA) - 2 * Vector3.Dot(PA, Vector3.Dot(PA, PQ) / PQ.magnitude * u) + Mathf.Pow(Vector3.Dot(PA, PQ) / PQ.magnitude, 2) * Vector3.Dot(u, u) - Mathf.Pow(cylinder.radius, 2);
        determinant = y * y - 4 * x * z;

        //algo segment cylindre
        if (determinant < 0)
            return false;

        x1 = (-y - Mathf.Sqrt(determinant)) / (2f * x);
        x2 = (-y + Mathf.Sqrt(determinant)) / (2f * x);

        if (x1 >= 0 && x1 <= 1)
        {
            interPt = seg.pt1 + x1 * AB;
            Vector3 haut = cylinder.pt1 + Vector3.Dot(interPt - cylinder.pt1, u) * u;
            interNormal = interPt - haut;
            interNormal.Normalize();
            return true;
        }

        if (x2 >= 0 && x2 <= 1)
        {
            interPt = seg.pt1 + x2 * AB;
            Vector3 haut = cylinder.pt1 + Vector3.Dot(interPt - cylinder.pt1, u) * u;
            interNormal = -(interPt - haut);
            interNormal.Normalize();
            return true;
        }

        return false;


    }
}
