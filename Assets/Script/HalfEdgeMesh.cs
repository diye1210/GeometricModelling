using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalfEdgeMesh
{
    public string name;
    public Dictionary<Vector3, List<Face>> neighborsFaces = new Dictionary<Vector3, List<Face>>();
    public List<Vertex> vertices = new List<Vertex>();
    public List<HalfEdge> edges = new List<HalfEdge>();
    public List<Face> faces = new List<Face>();

    public void AddFace(Face face)
    {
        face.index = faces.Count;
        faces.Add(face);
    }

    public void AddHalfEdge(HalfEdge halfedge)
    {
        halfedge.index = edges.Count;
        edges.Add(halfedge);
    }

    public void AddVertex(Vertex vertex)
    {
        vertex.index = vertices.Count;
        vertices.Add(vertex);
    }

    public void GetNeighborsFacesEdges(Vertex vertex, out Face[] faces, out HalfEdge[] edgesGet)
    {
        faces = neighborsFaces[vertex.position].ToArray();

        List<HalfEdge> edges = new List<HalfEdge>();
        foreach (var face in neighborsFaces[vertex.position])
        {
            HalfEdge edge = face.edge;
            for (int i = 0; i <= 3; i++)
            {
                if (edge.source.position == vertex.position)
                    edges.Add(edge);
                edge = edge.nextEdge;
            }
        }
        edgesGet = edges.ToArray();
    }

    public void SetDoubleEdges()
    {
        List<HalfEdge> newEdges = new List<HalfEdge>();
        
        foreach (var edge in edges)
        {
            if (newEdges.Contains(edge))
            {
                continue;
            }

            foreach (var face in neighborsFaces[edge.source.position])
            {
                HalfEdge doubleEdge = face.edge;
                for (int i = 0; i <= 3; i++)
                {
                    if (!(newEdges.Contains(edge) || Equals(doubleEdge, edge)))
                    {
                        Vector3 edgeSource = edge.source.position;
                        Vector3 edgeNext = edge.nextEdge.source.position;
                        Vector3 doubleEdgeSource = doubleEdge.source.position;
                        Vector3 doubleEdgeNext = doubleEdge.nextEdge.source.position;

                        if (edgeSource == doubleEdgeNext && edgeNext == doubleEdgeSource)
                        {
                            edge.doubleEdge = doubleEdge;
                            doubleEdge.doubleEdge = edge;
                            newEdges.Add(doubleEdge);
                            newEdges.Add(edge);
                        }
                    }


                    doubleEdge = doubleEdge.nextEdge;
                }
            }
        }
    }
 

}
