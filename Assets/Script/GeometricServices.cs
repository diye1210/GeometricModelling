using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GeometricClass;

public static class GeometricServices
{

    public static bool InterSegmentPlane(Segment segment, GeometricClass.Plane plane, out Vector3 interpt, out Vector3 interNormal)
    {
        interpt = new Vector3();
        interNormal = new Vector3();

        Vector3 AB = segment.pt2 - segment.pt1;

        float dotABn = Vector3.Dot(AB, plane.Normal);

        if (Mathf.Approximately(dotABn, 0))
        {
            return false;
        }

        float t = (plane.d - Vector3.Dot(segment.pt1, plane.Normal)) / dotABn;

        if (t < 0 || t > 1)
        {
            return false;
        }

        interpt = segment.pt1 + t * AB;

        if (dotABn < 0)
        {
            interNormal = plane.Normal;
        }
        else
        {
            interNormal = -plane.Normal;
        }
        interNormal.Normalize();
        return true;
    }

    public static bool isValid(float t)
    {
        return t >= 0 && t <= 1;

    }

    public static bool InterSegmentSphere(Segment segment, GeometricClass.Sphere sphere, out Vector3 interpt, out Vector3 interNormal)
    {
        interpt = new Vector3();
        interNormal = new Vector3();

        Vector3 AB = segment.pt2 - segment.pt1;
        Vector3 OA = segment.pt1 - sphere.center;

        float a = Vector3.Dot(AB, AB);
        float b = 2f * Vector3.Dot(OA, AB);
        float c = Vector3.Dot(OA, OA) - sphere.radius * sphere.radius;

        float det = b * b - 4f * a * c;

        if (det < 0)
        {
            return false;
        }

        float x1 = (-b - Mathf.Sqrt(det)) / (2f * a);
        float x2 = (-b + Mathf.Sqrt(det)) / (2f * a);

        if (isValid(x1))
        {
            interpt = segment.pt1 + x1 * AB;
            interNormal = (interpt - sphere.center);
            interNormal.Normalize();
            return true;
        }
        if (isValid(x2))
        {
            interpt = segment.pt1 + x2 * AB;
            interNormal = -(interpt - sphere.center);
            interNormal.Normalize();
            return true;
        }

        return false;
    }

    public static bool InterSegmentCylinder(Segment segment, Cylinder cylinder, out Vector3 interpt, out Vector3 interNormal)
    {
        interpt = new Vector3();
        interNormal = new Vector3();

        Vector3 AB = segment.pt2 - segment.pt1;
        Vector3 PA = segment.pt1 - cylinder.pt1;
        Vector3 PQ = cylinder.pt2 - cylinder.pt1;
        Vector3 u = PQ / PQ.magnitude;

        float a = Vector3.Dot(AB, AB) - 2 * Vector3.Dot(AB, Vector3.Dot(AB, PQ) / PQ.magnitude * u) + Mathf.Pow(Vector3.Dot(AB, PQ) / PQ.magnitude, 2) * Vector3.Dot(u, u);
        float b = 2 * Vector3.Dot(AB, PA) - 4 * Vector3.Dot(AB, Vector3.Dot(PA, PQ) / PQ.magnitude * u) + 2 * Vector3.Dot(AB, PQ) * Vector3.Dot(PA, PQ) / Mathf.Pow(PQ.magnitude, 2) * Vector3.Dot(u, u);
        float c = Vector3.Dot(PA, PA) - 2 * Vector3.Dot(PA, Vector3.Dot(PA, PQ) / PQ.magnitude * u) + Mathf.Pow(Vector3.Dot(PA, PQ) / PQ.magnitude, 2) * Vector3.Dot(u, u) - Mathf.Pow(cylinder.radius, 2);

        float det = b * b - 4f * a * c;

        if (det < 0)
        {
            return false;
        }

        float x1 = (-b - Mathf.Sqrt(det)) / (2f * a);
        float x2 = (-b + Mathf.Sqrt(det)) / (2f * a);

        if (isValid(x1))
        {
            interpt = segment.pt1 + x1 * AB;
            Vector3 H = cylinder.pt1 + Vector3.Dot(interpt - cylinder.pt1, u) * u;
            interNormal = interpt - H;
            interNormal.Normalize();
            return true;
        }
        if (isValid(x2))
        {
            interpt = segment.pt1 + x2 * AB;
            Vector3 H = cylinder.pt1 + Vector3.Dot(interpt - cylinder.pt1, u) * u;
            interNormal = -(interpt - H);
            interNormal.Normalize();
            return true;
        }

        return false;
    }
}
