using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CatmullClark : MonoBehaviour
{
    /// <summary>
    /// Convert tris to quads 
    /// </summary>
    /// <param name="mesh"></param>
    /// <param name="quads"></param>
    public static void TrianglesToQuads(Mesh mesh, out int[] quads)
    {
        int[] indicesTriangles = mesh.triangles;

        quads = new int[indicesTriangles.Length / 6 * 4];

        int index = 0;
        for (int i = 0; i < indicesTriangles.Length; i++)
        {
            quads[index++] = indicesTriangles[i++];
            quads[index++] = indicesTriangles[i++];
            quads[index++] = indicesTriangles[i++];
            i += 2;
            quads[index++] = indicesTriangles[i];
        }
    }
    /// <summary>
    /// convert vertexface to halfEdge 
    /// </summary>
    /// <param name="mesh"></param>
    /// <returns></returns>
    public static HalfEdgeMesh VertexFaceToHalfEdge(Mesh mesh)
    {
        HalfEdgeMesh halfEdgeMesh = new HalfEdgeMesh();

        int[] quads;
        Vector3[] vertices = mesh.vertices;
        TrianglesToQuads(mesh, out quads);

        for (int i = 0; i < vertices.Length; i++)
        {
            halfEdgeMesh.vertices.Add(new Vertex(vertices[i], i));
        }
        //liste des faces voisines 
        Dictionary<Vector3, List<Face>> neighborsFaces = new Dictionary<Vector3, List<Face>>();
        int index = 0;
        for (int i = 0; i < quads.Length / 4; i++)
        {
            Vertex vertice1 = halfEdgeMesh.vertices[quads[index]];
            Vertex vertice2 = halfEdgeMesh.vertices[quads[index + 1]];
            Vertex vertice3 = halfEdgeMesh.vertices[quads[index + 2]];
            Vertex vertice4 = halfEdgeMesh.vertices[quads[index + 3]];

            HalfEdge halfEdge1 = new HalfEdge(index, vertice1);
            HalfEdge halfEdge2 = new HalfEdge(index + 1, vertice2);
            HalfEdge halfEdge3 = new HalfEdge(index + 2, vertice3);
            HalfEdge halfEdge4 = new HalfEdge(index + 3, vertice4);

            Face face = new Face(index / 4);
            face.edge = halfEdge1;
            halfEdgeMesh.faces.Add(face);

            halfEdge1.prevEdge = halfEdge4;
            halfEdge2.prevEdge = halfEdge1;
            halfEdge3.prevEdge = halfEdge2;
            halfEdge4.prevEdge = halfEdge3;

            halfEdge1.nextEdge = halfEdge2;
            halfEdge2.nextEdge = halfEdge3;
            halfEdge3.nextEdge = halfEdge4;
            halfEdge4.nextEdge = halfEdge1;

            halfEdge1.face = face;
            halfEdge2.face = face;
            halfEdge3.face = face;
            halfEdge4.face = face;

            halfEdgeMesh.edges.Add(halfEdge1);
            halfEdgeMesh.edges.Add(halfEdge2);
            halfEdgeMesh.edges.Add(halfEdge3);
            halfEdgeMesh.edges.Add(halfEdge4);

            if (!neighborsFaces.ContainsKey(halfEdge1.source.position))
                neighborsFaces.Add(halfEdge1.source.position, new List<Face>());
            neighborsFaces[halfEdge1.source.position].Add(face);

            if (!neighborsFaces.ContainsKey(halfEdge2.source.position))
                neighborsFaces.Add(halfEdge2.source.position, new List<Face>());
            neighborsFaces[halfEdge2.source.position].Add(face);

            if (!neighborsFaces.ContainsKey(halfEdge3.source.position))
                neighborsFaces.Add(halfEdge3.source.position, new List<Face>());
            neighborsFaces[halfEdge3.source.position].Add(face);

            if (!neighborsFaces.ContainsKey(halfEdge4.source.position))
                neighborsFaces.Add(halfEdge4.source.position, new List<Face>());
            neighborsFaces[halfEdge4.source.position].Add(face);

            index += 4;
        }
        halfEdgeMesh.neighborsFaces = neighborsFaces;
        halfEdgeMesh.SetDoubleEdges();
        return halfEdgeMesh;

    }
    /// <summary>
    /// convert halfEdge to VertexFace
    /// </summary>
    /// <param name="halfEdgeMesh"></param>
    /// <returns></returns>
    public static Mesh HalfEdgeToVertexFace(HalfEdgeMesh halfEdgeMesh)
    {
        Mesh newMesh = new Mesh();
        //liste des arêtes 
        List<HalfEdge> edges = halfEdgeMesh.edges;
        //liste des points 
        List<Vector3> vertices = halfEdgeMesh.vertices.ConvertAll(x => x.position);
        List<int> quads = new List<int>();

        foreach (var face in halfEdgeMesh.faces)
        {
            quads.Add(face.edge.source.index);
            quads.Add(face.edge.nextEdge.source.index);
            quads.Add(face.edge.nextEdge.nextEdge.source.index);
            quads.Add(face.edge.nextEdge.nextEdge.nextEdge.source.index);
        }

        newMesh.vertices = vertices.ToArray();
        newMesh.SetIndices(quads, MeshTopology.Quads, 0);
        newMesh.RecalculateBounds();
        newMesh.RecalculateNormals();

        return newMesh;
    }



    /// <summary>
    /// Moyenne FacePoint
    /// </summary>
    /// <param name="f"></param>
    /// <returns></returns>
    public static Vertex FacePoint(Face f)
    {
        Vector3 vertice1 = f.edge.source.position;
        Vector3 vertice2 = f.edge.nextEdge.source.position;
        Vector3 vertice3 = f.edge.nextEdge.nextEdge.source.position;
        Vector3 vertice4 = f.edge.nextEdge.nextEdge.nextEdge.source.position;
        Vector3 result = (vertice1 + vertice2 + vertice3 + vertice4);
        result /= 4;
        return new Vertex(result);
    }

    /// <summary>
    /// Moyenne Edge Point
    /// </summary>
    /// <param name="edge"></param>
    /// <returns></returns>
    public static Vertex EdgePoint(HalfEdge edge)
    {
        Vertex vertice1 = edge.source;
        Vertex vertice2 = edge.nextEdge.source;
        Vertex result;
        if (edge.doubleEdge != null)
        {
            Vertex vertice3 = FacePoint(edge.face);
            Vertex vertice4 = FacePoint(edge.doubleEdge.face);
            result = new Vertex((vertice1.position + vertice2.position + vertice3.position + vertice4.position) / 4);
        }
        else
        {
            result = new Vertex((vertice1.position + vertice2.position) / 2);
        }
        return result;
    }

    /// <summary>
    /// Nv vertexPoint  
    /// </summary>
    /// <param name="v"></param>
    /// <param name="halfEdgeMesh"></param>
    /// <returns></returns>
    public static Vertex VertexPoint(Vertex v, HalfEdgeMesh halfEdgeMesh)
    {
        Face[] faces;
        HalfEdge[] edges;

        halfEdgeMesh.GetNeighborsFacesEdges(v, out faces, out edges);

        int n = edges.Length;
        if (n >= 3)
        {
            Vector3 Q = new Vector3();
            Vector3 R = new Vector3();

            for (int i = 0; i < faces.Length; i++)
            {
                Q += FacePoint(faces[i]).position;
            }
            Q /= faces.Length;


            for (int i = 0; i < edges.Length; i++)
            {
                R += (edges[i].source.position + edges[i].nextEdge.source.position) / 2;
            }
            R /= edges.Length;

            return new Vertex(Q / n + 2 * R / n + (n - 3) * v.position / n, v.index);
        }
        else 
        {
            return v;
        }
    }

    /// <summary>
    /// Diviser une face en 2 
    /// </summary>
    /// <param name="e"></param>
    /// <param name="v"></param>
    /// <param name="halfEdgeMesh"></param>
    /// <returns></returns>
    public static HalfEdge SplitEdge(HalfEdge e, Vertex v, HalfEdgeMesh halfEdgeMesh)
    {
        HalfEdge newHalfEdge = new HalfEdge(v, e.face);
        newHalfEdge.nextEdge = e.nextEdge;
        newHalfEdge.prevEdge = e;
        e.nextEdge = newHalfEdge;

        halfEdgeMesh.AddHalfEdge(newHalfEdge);

        return newHalfEdge;
    }

    /// <summary>
    /// Déterminer nextEdge et prevEdge 
    /// </summary>
    /// <param name="startEdge"></param>
    /// <param name="endEdge"></param>
    public static void SetNextAndPrevEdge(HalfEdge startEdge, HalfEdge endEdge)
    {
        if (startEdge == endEdge) return;
        startEdge.nextEdge = endEdge;
        endEdge.prevEdge = startEdge;
    }

 
    public static void AssociateEdgeToPoint(HalfEdge startEdge, Face face, Vertex facePoint, int index, HalfEdgeMesh halfEdgeMesh)
    {
        
        if (index < 0 || index >= 4) return;

        HalfEdge[] halfEdges = new HalfEdge[4];

        halfEdges[index] = startEdge;
        halfEdges[(index + 1) % 4] = new HalfEdge(startEdge.nextEdge.source, face);
        halfEdges[(index + 2) % 4] = new HalfEdge(facePoint, face);
        halfEdges[(index + 3) % 4] = startEdge.prevEdge;

        face.edge = halfEdges[0];

        SetNextAndPrevEdge(halfEdges[index], halfEdges[(index + 1) % 4]);
        SetNextAndPrevEdge(halfEdges[(index + 1) % 4], halfEdges[(index + 2) % 4]);
        SetNextAndPrevEdge(halfEdges[(index + 2) % 4], startEdge.prevEdge);

        halfEdgeMesh.AddHalfEdge(halfEdges[(index + 1) % 4]);
        halfEdgeMesh.AddHalfEdge(halfEdges[(index + 2) % 4]);
    }

    /// <summary>
    /// La subdivision  
    /// </summary>
    /// <param name="halfEdgeMesh"></param>
    public static void subdivision(HalfEdgeMesh halfEdgeMesh)
    {
        List<Face> faces = halfEdgeMesh.faces;
        List<HalfEdge> halfEdges = halfEdgeMesh.edges;
        List<Vertex> vertices = halfEdgeMesh.vertices;

        List<Vertex> facePoints = new List<Vertex>();
        List<Vertex> edgePoints = new List<Vertex>();

        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i] = VertexPoint(vertices[i], halfEdgeMesh);
        }

        foreach (Face face in faces)
        {
            Vertex facePoint = FacePoint(face);
            halfEdgeMesh.AddVertex(facePoint);
            facePoints.Add(facePoint);
        }

        foreach (HalfEdge halfEdge in halfEdges)
        {
            Vertex edgePoint = EdgePoint(halfEdge);
            halfEdgeMesh.AddVertex(edgePoint);
            edgePoints.Add(edgePoint);
        }


        int faceCount = faces.Count;
        for (int i = 0; i < faceCount; i++)
        {
            HalfEdge halfEdge1 = faces[i].edge;
            HalfEdge halfEdge2 = faces[i].edge.nextEdge;
            HalfEdge halfEdge3 = faces[i].edge.nextEdge.nextEdge;
            HalfEdge halfEdge4 = faces[i].edge.nextEdge.nextEdge.nextEdge;

            Face f1 = faces[i];
            Face f2 = new Face(halfEdgeMesh.faces.Count);
            Face f3 = new Face(halfEdgeMesh.faces.Count + 1);
            Face f4 = new Face(halfEdgeMesh.faces.Count + 2);

            halfEdgeMesh.AddFace(f2);
            halfEdgeMesh.AddFace(f3);
            halfEdgeMesh.AddFace(f4);

            HalfEdge halfEdgeFace2 = SplitEdge(halfEdge1, edgePoints[halfEdge1.index], halfEdgeMesh);
            HalfEdge halfEdgeFace3 = SplitEdge(halfEdge2, edgePoints[halfEdge2.index], halfEdgeMesh);
            HalfEdge halfEdgeFace4 = SplitEdge(halfEdge3, edgePoints[halfEdge3.index], halfEdgeMesh);
            HalfEdge halfEdgeFace1 = SplitEdge(halfEdge4, edgePoints[halfEdge4.index], halfEdgeMesh);

            halfEdge1.prevEdge = halfEdgeFace1;
            halfEdge2.prevEdge = halfEdgeFace2;
            halfEdge3.prevEdge = halfEdgeFace3;
            halfEdge4.prevEdge = halfEdgeFace4;

            halfEdgeFace1.face = f1;
            halfEdgeFace2.face = f2;
            halfEdgeFace3.face = f3;
            halfEdgeFace4.face = f4;

            halfEdge2.face = f2;
            halfEdge3.face = f3;
            halfEdge4.face = f4;

            Vertex facePoint = facePoints[faces[i].index];

            
            AssociateEdgeToPoint(halfEdge1, f1, facePoint, 0, halfEdgeMesh);
            AssociateEdgeToPoint(halfEdge2, f2, facePoint, 1, halfEdgeMesh);
            AssociateEdgeToPoint(halfEdge3, f3, facePoint, 2, halfEdgeMesh);
            AssociateEdgeToPoint(halfEdge4, f4, facePoint, 3, halfEdgeMesh);
        }
        halfEdgeMesh.neighborsFaces = null;
    }
    

    /// <summary>
    /// ALGO FINAL DE CATMULL CLARK (conversion de vertexFace à halfEdge + La subdivision + la conversion de halfEdge à vertexFace)
    /// </summary>
    /// <param name="mesh"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static Mesh Catmull_Clark(Mesh mesh, int count = 1)
    {
        Mesh newMesh = mesh;
        for (int i = 0; i < count; i++)
        {
            HalfEdgeMesh halfEdgeMesh = VertexFaceToHalfEdge(newMesh);
            subdivision(halfEdgeMesh);
            newMesh = HalfEdgeToVertexFace(halfEdgeMesh);
        }
        return newMesh;
    }
 
}
