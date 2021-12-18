using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static GeometricClass;
using static GeometricServices;
public class TestManager : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] GameObject Plane;
    [SerializeField] GameObject Sphere;
    [SerializeField] GameObject Cylinder;
    [SerializeField] GameObject PointA;
    [SerializeField] GameObject PointB;
    [SerializeField] GameObject PointC;

    [Header("Segment (A,B)")]
    [SerializeField] float Size_Point_AB;
    [SerializeField] Color Color_Point_A;
    [SerializeField] Color Color_Point_B;
    [SerializeField] Color Color_Segment_AB;
    [SerializeField] Color Color_Line_AB;

    [Header("Point C")]
    [SerializeField] float Size_Point_C;
    [SerializeField] Color Color_Point_C;
    [SerializeField] Color Color_Segment_C_To_Plane;
    [SerializeField] Color Color_Segment_C_To_Segment_AB;

    [Header("Normals")]
    [SerializeField] float Size_normals;
    [SerializeField] Color Color_Normals;

    [Header("Intersections")]
    [SerializeField] float Size_Intersect_Points;
    [SerializeField] Color Color_Intersect_Sphere;
    [SerializeField] Color Color_Intersect_Cylinder;
    [SerializeField] Color Color_Intersect_Plane;

    void OnDrawGizmos()
    {
        #region Segment
        {


            GUIStyle myStyle = new GUIStyle();
            myStyle.fontSize = 16;

            Vector3 posA = PointA.transform.position;
            Vector3 posB = PointB.transform.position;
            
            
            
            Gizmos.color = Color_Point_A;
            myStyle.normal.textColor = Color_Point_A;
            Gizmos.DrawSphere(posA, Size_Point_AB);
            Handles.Label(posA, "A", myStyle);

            Gizmos.color = Color_Point_B;
            myStyle.normal.textColor = Color_Point_B;
            Gizmos.DrawSphere(posB, Size_Point_AB);
            Handles.Label(posB, "B", myStyle);

            Gizmos.color = Color_Segment_AB;
            Gizmos.DrawLine(posA, posB);

            Gizmos.color = Color_Line_AB;
            Gizmos.DrawRay(posA, posB - posA);
            Gizmos.DrawRay(posA, posA - posB);

        }
        #endregion

        #region Point C
        {
            GUIStyle myStyle = new GUIStyle();
            myStyle.fontSize = 16;

            Vector3 posC = PointC.transform.position;

            Gizmos.color = Color_Point_C;
            myStyle.normal.textColor = Color_Point_A;
            Gizmos.DrawSphere(posC, Size_Point_C);
            Handles.Label(posC, "C", myStyle);

            #region Distance Plane
            {
                Gizmos.color = Color_Segment_C_To_Plane;
                myStyle.normal.textColor = Color_Segment_C_To_Plane;
                Vector3 normalPlane = Plane.transform.up;
                Vector3 OC = posC - Plane.transform.position;

                float distance = Vector3.Dot(normalPlane, OC);

                Vector3 u = Plane.transform.right / Plane.transform.right.magnitude;
                Vector3 v = Plane.transform.forward / Plane.transform.forward.magnitude;

                Vector3 OH = Vector3.Dot(OC, u) * u + Vector3.Dot(OC, v) * v + Plane.transform.position;

                Gizmos.DrawSphere(OH, Size_Point_C);
                Handles.Label(OH, "H", myStyle);

                Gizmos.DrawLine(OH, posC);
                Handles.Label((OH + posC) / 2, distance.ToString(), myStyle);
            }
            #endregion

            #region Distance Segment AB
            {
                Gizmos.color = Color_Segment_C_To_Segment_AB;
                myStyle.normal.textColor = Color_Segment_C_To_Segment_AB;

                Vector3 AB = PointB.transform.position - PointA.transform.position;
                Vector3 AC = PointC.transform.position - PointA.transform.position;                
                
                Vector3 u = AB / AB.magnitude;

                Vector3 H = Vector3.Dot(AC, u) * u + PointA.transform.position;

                float distance = Vector3.Cross(AC, u).magnitude;

                Gizmos.DrawSphere(H, Size_Point_C);
                Handles.Label(H, "H", myStyle);

                Gizmos.DrawLine(H, posC);
                Handles.Label((H + posC) / 2, distance.ToString(), myStyle);
            }
            #endregion

        }
        #endregion

        #region Plane
        {
            Vector3 interpt;
            Vector3 interNormal;
            Segment segment = new Segment { pt1 = PointA.transform.position, pt2 = PointB.transform.position };
            Vector3 norm = Plane.transform.up;
            GeometricClass.Plane plane = new GeometricClass.Plane { Normal = norm, d = Vector3.Dot(norm, Plane.transform.position) };
            if (InterSegmentPlane(segment, plane, out interpt, out interNormal))
            {
                Gizmos.color = Color_Intersect_Plane;
                Gizmos.DrawSphere(interpt, Size_Intersect_Points);
                Gizmos.color = Color_Normals;
                Gizmos.DrawLine(interpt, interpt + interNormal * Size_normals);
            }
        }
        #endregion

        #region Cylinder
        {
            Vector3 interpt;
            Vector3 interNormal;
            Segment segment = new Segment { pt1 = PointA.transform.position, pt2 = PointB.transform.position };
            GeometricClass.Cylinder cylinder = new GeometricClass.Cylinder { pt1 = Cylinder.transform.position, pt2 = Cylinder.transform.position + Cylinder.transform.up, radius = Cylinder.transform.localScale.x / 2 };
            if (InterSegmentCylinder(segment, cylinder, out interpt, out interNormal))
            {
                Gizmos.color = Color_Intersect_Cylinder;
                Gizmos.DrawSphere(interpt, Size_Intersect_Points);
                Gizmos.color = Color_Normals;
                Gizmos.DrawLine(interpt, interpt + interNormal * Size_normals);
            }
        }
        #endregion

        #region Sphere
        {
            Vector3 interpt;
            Vector3 interNormal;
            Segment segment = new Segment { pt1 = PointA.transform.position, pt2 = PointB.transform.position };
            GeometricClass.Sphere sphere = new GeometricClass.Sphere { center = Sphere.transform.position, radius = Sphere.transform.localScale.x / 2 };
            if (InterSegmentSphere(segment, sphere, out interpt, out interNormal))
            {
                Gizmos.color = Color_Intersect_Sphere;
                Gizmos.DrawSphere(interpt, Size_Intersect_Points);
                Gizmos.color = Color_Normals;
                Gizmos.DrawLine(interpt, interpt + interNormal * Size_normals);
            }
        }
        #endregion
    }
}
