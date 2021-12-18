using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Vertex
{
    public int index;
    public Vector3 position;
    public Vertex(Vector3 v = default(Vector3), int i = -1)
    {
        this.index = i;
        this.position = v;
    }
}

public class Face
{
    public int index;
    public HalfEdge edge;

    public Face(int i)
    {
        this.index = i;
    }
}

public class HalfEdge
{
    public int index;

    public Vertex source;

    public HalfEdge prevEdge;
    public HalfEdge nextEdge;

    public HalfEdge twinEdge;

    public Face face;
    public HalfEdge(Vertex v, Face f)
    {
        this.source = v;
        this.face = f;
    }

    public HalfEdge(int i, Vertex source, HalfEdge prev = null, HalfEdge next = null, HalfEdge twin = null, Face face = null)
    {
        this.index = i;
        this.source = source;
        this.prevEdge = prev;
        this.nextEdge = next;
        this.twinEdge = twin;
        this.face = face;
    }
}

public class HalfEdgeMesh
{
    int data = 0;

    public string name;
    public List<Vertex> vertices = new List<Vertex>();
    public List<HalfEdge> edges = new List<HalfEdge>();
    public List<Face> faces = new List<Face>();
    public Dictionary<Vector3, List<Face>> neighborsFaces = new Dictionary<Vector3, List<Face>>();
    public bool hasNormal() { return true; }

    public bool hasUV() { return true; }

    public void Add(HalfEdge h)
    {
        h.index = edges.Count;
        edges.Add(h);
    }

    public void Add(Vertex v)
    {
        v.index = vertices.Count;
        vertices.Add(v);
    }

    public void Add(Face f)
    {
        f.index = faces.Count;
        faces.Add(f);
    }

    /// <summary>
    /// Set Twin Edges with neighbors informations
    /// </summary>
    public void SetTwinEdges()
    {
        List<HalfEdge> foundEdges = new List<HalfEdge>();
        foreach (var edge in edges)
        {
            if (foundEdges.Contains(edge)) continue;

            foreach (var face in neighborsFaces[edge.source.position])
            {
                HalfEdge twinEdge = face.edge;
                for (int i = 0; i < 4; i++)
                {
                    if (!(foundEdges.Contains(edge) || Equals(twinEdge, edge)))
                    {
                        Vector3 edgeSource = edge.source.position;
                        Vector3 edgeNext = edge.nextEdge.source.position;
                        Vector3 twinEdgeSource = twinEdge.source.position;
                        Vector3 twinEdgeNext = twinEdge.nextEdge.source.position;

                        if (edgeSource == twinEdgeNext && edgeNext == twinEdgeSource)
                        {
                            edge.twinEdge = twinEdge;
                            twinEdge.twinEdge = edge;
                            foundEdges.Add(twinEdge);
                            foundEdges.Add(edge);
                        }
                    }
                    twinEdge = twinEdge.nextEdge;
                }
            }
        }
    }

    public Face[] GetNeighborsFaces(Vertex v)
    {
        return neighborsFaces[v.position].ToArray();
    }

    public HalfEdge[] GetNeighborsEdges(Vertex v)
    {
        List<HalfEdge> edges = new List<HalfEdge>();
        foreach (var face in neighborsFaces[v.position])
        {
            HalfEdge edge = face.edge;
            for (int i = 0; i < 4; i++)
            {
                if (edge.source.position == v.position)
                    edges.Add(edge);
                edge = edge.nextEdge;
            }
        }
        return edges.ToArray();
    }

    public void GetNeighborsFacesEdges(Vertex v, out Face[] faces, out HalfEdge[] edges)
    {
        faces = GetNeighborsFaces(v);
        edges = GetNeighborsEdges(v);
    }
}







