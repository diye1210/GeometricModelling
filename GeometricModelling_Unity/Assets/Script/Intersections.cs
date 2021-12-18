using System.Collections;
using System.Collections.Generic;
using System.Windows;
using UnityEngine;

public class Intersections : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		/*Vector3 A = new Vector3(1, 2, 3);
		Vector3 B = new Vector3(2, 3, 4);
		Vector3 norm = new Vector3(0, 1, 0);
		float d = 1;
		Vector3 interPt;
		Vector3 interNormal;

		Segment AB = new Segment(A, B);
		Plane plane = new Plane(norm, d);

		bool test = InterSegmentPlane(AB, plane, out interPt, out interNormal);

		//Gizmos.DrawSegment(AB);
		Gizmos.DrawLine(new Vector3(-10, 1, 10), new Vector3(10, 1, 10));
		//DrawSegment(AB);
		//Debug.DrawLine(new Vector3(-10, 1, 10), new Vector3(10, 1, 10), Color.red, 10);
		Debug.Log(test);*/

	}

	/*bool InterSegmentPlane(Segment seg, Plane plane, out Vector3 interPt, out Vector3 interNormal)
	{
		/*Vector3 AB = seg.Pt2
		//	...- seg.Pt1;

		float dotABn = Vector3.Dot(AB, plane.Normal);
		if (Mathf.Approximately(dotABn, 0))
		{
			return false;
		}
		float t = (plane.D - (Vector.Multiply(seg.Pt1, plane.Normal))) / dotABn;
 
		if (t < 0 || t > 1)
		{
			return false;
		}

		interPt = seg.Pt1 + t * AB;

		if (dotABn < 0)
		{
			interNormal = plane.Normal;
		}
	}*/

	
}
