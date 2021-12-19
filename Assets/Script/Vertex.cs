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
