using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CatmullClark : MonoBehaviour
{

    void explications()
    {
        /*Subdivision
         * 
         * 1) calcul de nouveaux points ( ! ne fonctionne que pour des vertices intérieures différentes des boundaries vertices)
         * * 
         *  -FacePoints : les isobarycentres des faces/centroïdes
         *      isobarycentre = moyenne des vertices qui définissent la face (somme des positions vertices divisés par le nombre)
         *      
         *  -EdgePoints = moyenne des extémités de l'edge et des deux FacePoints des faces adjacentes
         *      ! le point obtenu n'est pas forcément sur l'edge concerné mais dans son voisinnage 
         *      
         * FacePoints et EdgePoints sont des nouvelles vertices (points)
         * VertexPoints c'est la nouvelle position des vertices existantes (maj)
         *      
         *  -VertexPoints :  newPos = Q/n + 2R/n + (n-3)*currPos/n
         *      Q = moyenne des FacePoints de la vertice (faces adjacentes)
         *      R = moyenne des mid-Points de la vertice 
         *          mid-points : centre des edges = (startPosEdge + endPosEdge)/2
         *      n = Valence de la vertice : nombre d'edges arrivant sur la vertice
         * 
         * Cas bordure : (1ère version)
         *  -FacePoints même calcul
         *  -EdgePoints = mid_points
         *  -VertexPoints ne bouge (pas de changements de positions des points sur la bordure)
         * 
         *si la valence = nombre de faces d'adjacences le point est en bordure
         *
         *2) création des nouvelles faces
         *
         *  -Connexion des FacePoints et des EdgePoints
         *  -Connexion des EdgePoints et des Vertices (qui ont changés de position)
         *  
         *  
         *Pseudo Code
         *  1) création des nouveaux points : facePoints et edgePoints
         *      Rajouter dans les structure edge/ face les facepoints et edgePoints
         *  2) update de la position des vertices (vertex points)
         *  3) split des edges en y insérant l'edge point
         *      conserver l'edge avant split et modifier une de ses positions + insérer les nouvelles edges (perfo)
         *      methode SplitEdge(Edge, vertex)
         *  4) split des faces (seulement pour des faces ayant un nombre de vertices pair)
         *      - on ajoute le FacePoint
         *      - on créée k/2 nouvelles faces si k vertices(y compris les vertices issues de l'étape 3)
         *      methode splitFace(Face,vertex)
         * 
         * 
         * 
         */
    }

    #region conversion
    public static void TrianglesToQuads(Mesh mesh, out int[] quads)
    {
        Vector3[] triVertices = mesh.vertices;
        int[] triIndices = mesh.triangles;

        quads = new int[triIndices.Length / 6 * 4];

        int index = 0;
        for (int i = 0; i < triIndices.Length; i++)
        {
            quads[index++] = triIndices[i++];
            quads[index++] = triIndices[i++];
            quads[index++] = triIndices[i++];
            i += 2;
            quads[index++] = triIndices[i];
        }
    }

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

        //store neighbors faces for later
        Dictionary<Vector3, List<Face>> neighborsFaces = new Dictionary<Vector3, List<Face>>();

        int index = 0;

        for (int i = 0; i < quads.Length / 4; i++)
        {
            Vertex vertex1 = halfEdgeMesh.vertices[quads[index]];
            Vertex vertex2 = halfEdgeMesh.vertices[quads[index + 1]];
            Vertex vertex3 = halfEdgeMesh.vertices[quads[index + 2]];
            Vertex vertex4 = halfEdgeMesh.vertices[quads[index + 3]];

            HalfEdge halfEdge1 = new HalfEdge(index, vertex1);
            HalfEdge halfEdge2 = new HalfEdge(index + 1, vertex2);
            HalfEdge halfEdge3 = new HalfEdge(index + 2, vertex3);
            HalfEdge halfEdge4 = new HalfEdge(index + 3, vertex4);

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
        halfEdgeMesh.SetTwinEdges();
        return halfEdgeMesh;

    }

    public static Mesh HalfEdgeToVertexFace(HalfEdgeMesh halfEdgeMesh)
    {
        Mesh newMesh = new Mesh();
        List<HalfEdge> edges = halfEdgeMesh.edges;
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
    #endregion

    #region Catmull Clark services

    /// <summary>
    /// Compute Face Point
    /// 
    /// Average of each point in face
    /// </summary>
    /// <param name="f"></param>
    /// <returns></returns>
    public static Vertex FacePoint(Face f)
    {
        Vector3 vertex1 = f.edge.source.position;
        Vector3 vertex2 = f.edge.nextEdge.source.position;
        Vector3 vertex3 = f.edge.nextEdge.nextEdge.source.position;
        Vector3 vertex4 = f.edge.nextEdge.nextEdge.nextEdge.source.position;
        Vector3 result = (vertex1 + vertex2 + vertex3 + vertex4);
        result /= 4;
        return new Vertex(result);
    }

    /// <summary>
    /// Compute Edge Point
    /// 
    /// Average of Face Point of adjacent faces and the center of the edge
    /// 
    /// In case of boundary edge the Edge Point is the center of the edge
    /// </summary>
    /// <param name="edge"></param>
    /// <returns></returns>
    public static Vertex EdgePoint(HalfEdge edge)
    {
        Vertex vertex1 = edge.source;
        Vertex vertex2 = edge.nextEdge.source;
        Vertex result;
        if (edge.twinEdge != null)
        {
            Vertex vertex3 = FacePoint(edge.face);
            Vertex vertex4 = FacePoint(edge.twinEdge.face);
            result = new Vertex((vertex1.position + vertex2.position + vertex3.position + vertex4.position) / 4);
        }
        else
        {
            result = new Vertex((vertex1.position + vertex2.position) / 2);
        }
        return result;
    }

    /// <summary>
    /// Compute new Vertex Point
    /// 
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

            //Compute Q : Average of Face Points
            for (int i = 0; i < faces.Length; i++)
            {
                Q += FacePoint(faces[i]).position;
            }
            Q /= faces.Length;


            //Compute R : Average of mid Edge
            for (int i = 0; i < edges.Length; i++)
            {
                R += (edges[i].source.position + edges[i].nextEdge.source.position) / 2;
            }
            R /= edges.Length;

            return new Vertex(Q / n + 2 * R / n + (n - 3) * v.position / n, v.index);
        }
        else //Boundary
        {
            return v;
        }
    }

    /// <summary>
    /// split an edge in 2
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

        halfEdgeMesh.Add(newHalfEdge);

        return newHalfEdge;
    }

    /// <summary>
    /// Set previous and next for a pair of edges
    /// </summary>
    /// <param name="startEdge"></param>
    /// <param name="endEdge"></param>
    public static void SetNextPrevEdge(HalfEdge startEdge, HalfEdge endEdge)
    {
        if (startEdge == endEdge) return;
        startEdge.nextEdge = endEdge;
        endEdge.prevEdge = startEdge;
    }

    /// <summary>
    /// Connect and create edges in the face
    /// </summary>
    /// <param name="startEdge"></param>
    /// <param name="face"></param>
    /// <param name="facePoint"></param>
    /// <param name="index">index of the start edge</param>
    /// <param name="halfEdgeMesh"></param>
    public static void ConnectEdgesToFacePoint(HalfEdge startEdge, Face face, Vertex facePoint, int index, HalfEdgeMesh halfEdgeMesh)
    {
        /*  index of the start edge:
         *           2
         *       __________
         *      |          |
         *      |          |
         *     1|          |3      e.g. : start edge index = 2 ==> we need to create edge 3 and 4
         *      |          |
         *      |__________|
         *            0
         * 
         */
        if (index < 0 || index >= 4) return;

        HalfEdge[] halfEdges = new HalfEdge[4];

        //2 new edges after the start edge
        halfEdges[index] = startEdge;
        halfEdges[(index + 1) % 4] = new HalfEdge(startEdge.nextEdge.source, face);
        halfEdges[(index + 2) % 4] = new HalfEdge(facePoint, face);
        halfEdges[(index + 3) % 4] = startEdge.prevEdge;

        face.edge = halfEdges[0];

        SetNextPrevEdge(halfEdges[index], halfEdges[(index + 1) % 4]);
        SetNextPrevEdge(halfEdges[(index + 1) % 4], halfEdges[(index + 2) % 4]);
        SetNextPrevEdge(halfEdges[(index + 2) % 4], startEdge.prevEdge);

        halfEdgeMesh.Add(halfEdges[(index + 1) % 4]);
        halfEdgeMesh.Add(halfEdges[(index + 2) % 4]);
    }

    /// <summary>
    /// Subdivision algorithm
    /// </summary>
    /// <param name="halfEdgeMesh"></param>
    public static void Subdivide(HalfEdgeMesh halfEdgeMesh)
    {
        //get faces / edges / vertices
        List<Face> faces = halfEdgeMesh.faces;
        List<HalfEdge> halfEdges = halfEdgeMesh.edges;
        List<Vertex> vertices = halfEdgeMesh.vertices;

        //List of new points
        List<Vertex> facePoints = new List<Vertex>();
        List<Vertex> edgePoints = new List<Vertex>();

        //Compute new vertex poisition
        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i] = VertexPoint(vertices[i], halfEdgeMesh);
        }

        //Compute face points
        foreach (Face face in faces)
        {
            Vertex facePoint = FacePoint(face);
            halfEdgeMesh.Add(facePoint);
            facePoints.Add(facePoint);
        }

        //compute edge points
        foreach (HalfEdge halfEdge in halfEdges)
        {
            Vertex edgePoint = EdgePoint(halfEdge);
            halfEdgeMesh.Add(edgePoint);
            edgePoints.Add(edgePoint);
        }

        //for each face:
        //create new faces from face point
        //split every edges in 2 (old edge to edge point and new edge to old edge end point)
        //connect face point to edge point
        int faceCount = faces.Count;
        for (int i = 0; i < faceCount; i++)
        {
            //get all edges in the face
            HalfEdge halfEdge1 = faces[i].edge;
            HalfEdge halfEdge2 = faces[i].edge.nextEdge;
            HalfEdge halfEdge3 = faces[i].edge.nextEdge.nextEdge;
            HalfEdge halfEdge4 = faces[i].edge.nextEdge.nextEdge.nextEdge;

            //create 3 new faces
            Face f1 = faces[i];
            Face f2 = new Face(halfEdgeMesh.faces.Count);
            Face f3 = new Face(halfEdgeMesh.faces.Count + 1);
            Face f4 = new Face(halfEdgeMesh.faces.Count + 2);

            halfEdgeMesh.Add(f2);
            halfEdgeMesh.Add(f3);
            halfEdgeMesh.Add(f4);

            //Split every edges in 2
            HalfEdge halfEdgeFace2 = SplitEdge(halfEdge1, edgePoints[halfEdge1.index], halfEdgeMesh);
            HalfEdge halfEdgeFace3 = SplitEdge(halfEdge2, edgePoints[halfEdge2.index], halfEdgeMesh);
            HalfEdge halfEdgeFace4 = SplitEdge(halfEdge3, edgePoints[halfEdge3.index], halfEdgeMesh);
            HalfEdge halfEdgeFace1 = SplitEdge(halfEdge4, edgePoints[halfEdge4.index], halfEdgeMesh);

            halfEdge1.prevEdge = halfEdgeFace1;
            halfEdge2.prevEdge = halfEdgeFace2;
            halfEdge3.prevEdge = halfEdgeFace3;
            halfEdge4.prevEdge = halfEdgeFace4;

            //set "bottom" edge in respective face
            halfEdgeFace1.face = f1;
            halfEdgeFace2.face = f2;
            halfEdgeFace3.face = f3;
            halfEdgeFace4.face = f4;

            halfEdge2.face = f2;
            halfEdge3.face = f3;
            halfEdge4.face = f4;

            Vertex facePoint = facePoints[faces[i].index];

            //connect new edges
            ConnectEdgesToFacePoint(halfEdge1, f1, facePoint, 0, halfEdgeMesh);
            ConnectEdgesToFacePoint(halfEdge2, f2, facePoint, 1, halfEdgeMesh);
            ConnectEdgesToFacePoint(halfEdge3, f3, facePoint, 2, halfEdgeMesh);
            ConnectEdgesToFacePoint(halfEdge4, f4, facePoint, 3, halfEdgeMesh);
        }
        halfEdgeMesh.neighborsFaces = null;
    }
    #endregion

    /// <summary>
    /// Catmull clark subdivision surface algorithm
    /// </summary>
    /// <param name="mesh">mesh tto subdivide</param>
    /// <param name="count">number of iteration</param>
    /// <returns></returns>
    public static Mesh Catmull_Clark(Mesh mesh, int count = 1)
    {
        Mesh newMesh = mesh;
        for (int i = 0; i < count; i++)
        {
            HalfEdgeMesh halfEdgeMesh = VertexFaceToHalfEdge(newMesh);
            Subdivide(halfEdgeMesh);
            newMesh = HalfEdgeToVertexFace(halfEdgeMesh);
        }
        return newMesh;
    }
 
}
