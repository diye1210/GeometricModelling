using UnityEngine;
[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    delegate Vector3 ComputePositionFromKxKz(float kX, float kZ);

    MeshFilter m_Mf;

    private void Awake()
    {
        m_Mf = GetComponent<MeshFilter>();
        //Mesh mesh = CreateTriangle();
        //Mesh mesh = CreateQuadXZ(new Vector3(4,0,2));
        //Mesh mesh = CreateStripXZ(new Vector3(4, 0, 2),200);

        //Mesh mesh = CreatePlaneXZ(new Vector3(4, 0, 2), 20,10);
        //Mesh mesh = CreateQuadXZ(new Vector3(4, 0, 2));
        //Mesh mesh = WrapNormalizePlane( 20, 10, (kX,kZ)=>new Vector3((kX-0.5f)*4,0,(kZ-0.5f)*2) );
        /*
        Mesh mesh = WrapNormalizePlane(20, 100,
        (kX, kZ) =>{
        float rho = 2*(1+.25f*Mathf.Sin(kZ*Mathf.PI*2*4));
        float theta = kX * 2 * Mathf.PI;
        float y = kZ * 4;
        return new Vector3(rho*Mathf.Cos(theta),y, rho * Mathf.Sin(theta));
        }
        );*/


        //Mesh mesh = 
        //    WrapNormalizePlane(200, 100,
        //        (kX, kZ) =>
        //        {
        //            float rho = 2 * (1 + .25f * Mathf.Sin(kZ * Mathf.PI * 2 * 4));
        //            float theta = kX * 2 * Mathf.PI;
        //            float phi = (1 - kZ) * Mathf.PI;
        //            return rho * new Vector3(Mathf.Cos(theta) * Mathf.Sin(phi), Mathf.Cos(phi), Mathf.Sin(theta) * Mathf.Sin(phi));
        //        }
        //    );



        //Mesh mesh = WrapNormalizePlaneQuads(20, 10, (kX, kZ) => new Vector3( (kX - 0.5f) * 4, Mathf.Sin(kX * Mathf.PI * 2 * 3), (kZ - .5f) * 2));
        //Mesh mesh = WrapNormalizePlaneQuads(20, 10, (kX, kZ) =>
        //{
        //    float rho = 2;
        //    float theta = kX * 2 * Mathf.PI;
        //    float phi = (1 - kZ) * Mathf.PI;
        //    return new Vector3(rho * Mathf.Cos(theta), rho * Mathf.Cos(phi), rho * Mathf.Sin(theta) * Mathf.Sin(phi));
        //    //return new Vector3(rho * Mathf.Sin(theta) * Mathf.Sin(phi), rho * Mathf.Cos(phi), rho * Mathf.Cos(theta));
        //    //return new Vector3(rho * Mathf.Cos(theta), rho * Mathf.Cos(phi), rho * Mathf.Sin(theta) * Mathf.Sin(phi));
        //});
        //m_Mf.sharedMesh = CreateStripXZQuads(new Vector3(4, 0, 2), 5);
        Mesh mesh = CreateRegularPolygonXZQuads(1, 3);
        m_Mf.sharedMesh = mesh;
        gameObject.AddComponent<MeshCollider>();
    }
    #region Triangles
    Mesh CreateTriangle()
    {
        Mesh newMesh = new Mesh();
        newMesh.name = "triangle";

        Vector3[] vertices = new Vector3[3];
        int[] triangles = new int[1 * 3];

        vertices[0] = Vector3.right;
        vertices[1] = Vector3.up;
        vertices[2] = Vector3.forward;

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;

        newMesh.vertices = vertices;
        newMesh.triangles = triangles;
        newMesh.RecalculateBounds();
        return newMesh;
    }

    Mesh CreateStripXZ(Vector3 size, int nSegments)
    {
        Vector3 halfSize = size * .5f;

        Mesh newMesh = new Mesh();
        newMesh.name = "strip";

        Vector3[] vertices = new Vector3[(nSegments + 1) * 2];
        int[] triangles = new int[nSegments * 2 * 3];

        //Vertices
        for (int i = 0; i < nSegments + 1; i++)
        {
            float k = (float)i / nSegments;
            float y = .25f * Mathf.Sin(k * Mathf.PI * 2 * 3);
            vertices[i] = new Vector3(Mathf.Lerp(-halfSize.x, halfSize.x, k), y, -halfSize.z);
            vertices[nSegments + 1 + i] = new Vector3(Mathf.Lerp(-halfSize.x, halfSize.x, k), y, halfSize.z);
        }

        //Triangles
        int index = 0;
        for (int i = 0; i < nSegments; i++)
        {
            triangles[index++] = i;
            triangles[index++] = i + nSegments + 1;
            triangles[index++] = i + nSegments + 2;

            triangles[index++] = i;
            triangles[index++] = i + nSegments + 2;
            triangles[index++] = i + 1;
        }

        newMesh.vertices = vertices;
        newMesh.triangles = triangles;
        newMesh.RecalculateBounds();
        return newMesh;
    }

    Mesh CreatePlaneXZ(Vector3 size, int nSegmentsX, int nSegmentsZ)
    {
        Vector3 halfSize = size * .5f;

        Mesh newMesh = new Mesh();
        newMesh.name = "plane";

        Vector3[] vertices = new Vector3[(nSegmentsX + 1) * (nSegmentsZ + 1)];
        int[] triangles = new int[nSegmentsX * nSegmentsZ * 2 * 3];

        //Vertices
        int index = 0;
        for (int i = 0; i < nSegmentsX + 1; i++)
        {
            float kX = (float)i / nSegmentsX;
            float x = Mathf.Lerp(-halfSize.x, halfSize.x, kX);
            for (int j = 0; j < nSegmentsZ + 1; j++)
            {
                float kZ = (float)j / nSegmentsZ;
                vertices[index++] = new Vector3(x, 0, Mathf.Lerp(-halfSize.z, halfSize.z, kZ));
            }
        }

        //Triangles
        index = 0;
        //double boucle également
        for (int i = 0; i < nSegmentsX; i++)
        {
            for (int j = 0; j < nSegmentsZ; j++)
            {
                triangles[index++] = i * (nSegmentsZ + 1) + j;
                triangles[index++] = i * (nSegmentsZ + 1) + j + 1;
                triangles[index++] = (i + 1) * (nSegmentsZ + 1) + j + 1;

                triangles[index++] = i * (nSegmentsZ + 1) + j;
                triangles[index++] = (i + 1) * (nSegmentsZ + 1) + j + 1;
                triangles[index++] = (i + 1) * (nSegmentsZ + 1) + j;
            }
        }

        newMesh.vertices = vertices;
        newMesh.triangles = triangles;
        newMesh.RecalculateBounds();
        newMesh.RecalculateNormals();
        return newMesh;
    }

    Mesh WrapNormalizePlane(int nSegmentsX, int nSegmentsZ, ComputePositionFromKxKz computePosition)
    {
        Mesh newMesh = new Mesh();
        newMesh.name = "plane";
        Vector3[] vertices = new Vector3[(nSegmentsX + 1) * (nSegmentsZ + 1)];
        int[] triangles = new int[nSegmentsX * nSegmentsZ * 2 * 3];
        //Vertices
        int index = 0;
        for (int i = 0; i < nSegmentsX + 1; i++)
        {
            float kX = (float)i / nSegmentsX;
            for (int j = 0; j < nSegmentsZ + 1; j++)
            {
                float kZ = (float)j / nSegmentsZ;
                vertices[index++] = computePosition(kX, kZ);
            }
        }

        //Triangles
        index = 0;
        //double boucle également
        for (int i = 0; i < nSegmentsX; i++)
        {
            for (int j = 0; j < nSegmentsZ; j++)
            {
                triangles[index++] = i * (nSegmentsZ + 1) + j;
                triangles[index++] = i * (nSegmentsZ + 1) + j + 1;
                triangles[index++] = (i + 1) * (nSegmentsZ + 1) + j + 1;

                triangles[index++] = i * (nSegmentsZ + 1) + j;
                triangles[index++] = (i + 1) * (nSegmentsZ + 1) + j + 1;
                triangles[index++] = (i + 1) * (nSegmentsZ + 1) + j;


            }
        }

        newMesh.vertices = vertices;
        newMesh.triangles = triangles;
        newMesh.RecalculateBounds();
        newMesh.RecalculateNormals();
        return newMesh;
    }
    #endregion

    #region Quad

    Mesh CreateQuadXZ(Vector3 size)
    {
        Vector3 halfSize = size * .5f;

        Mesh newMesh = new Mesh();
        newMesh.name = "quad";

        Vector3[] vertices = new Vector3[4];
        int[] triangles = new int[2 * 3];

        vertices[0] = new Vector3(-halfSize.x, 0, -halfSize.z);
        vertices[1] = new Vector3(-halfSize.x, 0, halfSize.z);
        vertices[2] = new Vector3(halfSize.x, 0, halfSize.z);
        vertices[3] = new Vector3(halfSize.x, 0, -halfSize.z);

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;

        triangles[3] = 0;
        triangles[4] = 2;
        triangles[5] = 3;

        newMesh.vertices = vertices;
        newMesh.triangles = triangles;
        newMesh.RecalculateBounds();
        return newMesh;
    }

    Mesh CreateStripXZQuads(Vector3 size, int nSegments)
    {
        Vector3 halfSize = size * .5f;

        Mesh newMesh = new Mesh();
        newMesh.name = "stripQuads";

        Vector3[] vertices = new Vector3[(nSegments + 1) * 2];
        int[] quads = new int[nSegments * 4];

        //Vertices
        for (int i = 0; i < nSegments + 1; i++)
        {
            float k = (float)i / nSegments;
            float y = .25f * Mathf.Sin(k * Mathf.PI * 2 * 3);
            vertices[i] = new Vector3(Mathf.Lerp(-halfSize.x, halfSize.x, k), y, -halfSize.z);
            vertices[nSegments + 1 + i] = new Vector3(Mathf.Lerp(-halfSize.x, halfSize.x, k), y, halfSize.z);
        }

        //Triangles
        int index = 0;
        for (int i = 0; i < nSegments; i++)
        {
            quads[index++] = i;
            quads[index++] = i + nSegments + 1;
            quads[index++] = i + nSegments + 2;
            quads[index++] = i + 1;
        }

        newMesh.vertices = vertices;
        newMesh.SetIndices(quads, MeshTopology.Quads, 0);
        newMesh.RecalculateBounds();
        newMesh.RecalculateNormals();
        return newMesh;
    }

    Mesh WrapNormalizePlaneQuads(int nSegmentsX, int nSegmentsZ, ComputePositionFromKxKz computePosition)
    {
        Mesh newMesh = new Mesh();
        newMesh.name = "plane";
        Vector3[] vertices = new Vector3[(nSegmentsX + 1) * (nSegmentsZ + 1)];
        int[] quads = new int[nSegmentsX * nSegmentsZ * 4];
        //Vertices
        int index = 0;
        for (int i = 0; i < nSegmentsX + 1; i++)
        {
            float kX = (float)i / nSegmentsX;
            for (int j = 0; j < nSegmentsZ + 1; j++)
            {
                float kZ = (float)j / nSegmentsZ;
                vertices[index++] = computePosition(kX, kZ);
            }
        }

        //Quads
        index = 0;
        //double boucle également
        for (int i = 0; i < nSegmentsX; i++)
        {
            for (int j = 0; j < nSegmentsZ; j++)
            {
                quads[index++] = i * (nSegmentsZ + 1) + j;
                quads[index++] = i * (nSegmentsZ + 1) + j + 1;
                quads[index++] = (i + 1) * (nSegmentsZ + 1) + j + 1;
                quads[index++] = (i + 1) * (nSegmentsZ + 1) + j;
            }
        }

        newMesh.vertices = vertices;
        newMesh.SetIndices(quads, MeshTopology.Quads, 0);
        newMesh.RecalculateBounds();
        newMesh.RecalculateNormals();
        return newMesh;
    }

    Mesh CreateRegularPolygonXZQuads(float radius, int nQuads)
    {
        Mesh newMesh = new Mesh();
        newMesh.name = "RegularPolygonQuads";

        Vector3[] vertices = new Vector3[nQuads * 2 + 1];
        int[] quads = new int[nQuads * 4];

        vertices[0] = Vector3.zero;


        Vector3[] points = new Vector3[nQuads];
        //Points
        for (int i = 0; i < nQuads; i++)
        {
            float x = Mathf.Sin(((float)i / nQuads) * 2 * Mathf.PI);
            float z = Mathf.Cos(((float)i / nQuads) * 2 * Mathf.PI);
            points[i] = new Vector3(x, 0, z);
        }


        //Vertices
        int index = 1;
        for (int i = 0; i < nQuads; i++)
        {
            vertices[index++] = points[i];
            int idx = (i + 1) % nQuads;
            Vector3 a = points[i];
            Vector3 b = points[idx];
            Vector3 c = (a + b) / 2;
            vertices[index++] = c;
        }

        //Quads
        index = 0;
        for (int i = 0; i < nQuads - 1; i++)
        {
            quads[index++] = 0;
            quads[index++] = (2 * (i + 1));
            quads[index++] = (2 * (i + 1) + 1);
            quads[index++] = (2 * (i + 1) + 2);
        }
        quads[index++] = 0;
        quads[index++] = nQuads * 2;
        quads[index++] = 1;
        quads[index++] = 2;



        newMesh.vertices = vertices;
        newMesh.SetIndices(quads, MeshTopology.Quads, 0);
        newMesh.RecalculateBounds();
        newMesh.RecalculateNormals();
        return newMesh;
    }
    #endregion
}