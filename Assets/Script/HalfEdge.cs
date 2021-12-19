using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalfEdge
{
    public int index;
    public Vertex source;
    public HalfEdge prevEdge;
    public HalfEdge nextEdge;
    public HalfEdge doubleEdge;

    public Face face;
    public HalfEdge(Vertex v, Face f)
    {
        this.source = v;
        this.face = f;
    }

    public HalfEdge(int i, Vertex source, HalfEdge prev = null, HalfEdge next = null, HalfEdge doubleEdge = null, Face face = null)
    {
        index = i;
        this.source = source;
        prevEdge = prev;
        nextEdge = next;
        this.doubleEdge = doubleEdge;
        this.face = face;
    }
}







