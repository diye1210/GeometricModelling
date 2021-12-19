using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCClark : MonoBehaviour
{
    MeshFilter mF;
    [Header("Parameters")]
    [SerializeField] int nb_iter;
    void Start()
    {
        mF = GetComponent<MeshFilter>();

        if (nb_iter == 0)
        {
            Debug.Log("Mesh");
            mF.sharedMesh = CatmullClark.HalfEdgeToVertexFace(CatmullClark.VertexFaceToHalfEdge(mF.sharedMesh));
        }
        if (nb_iter != 0)
        {
            Debug.Log("Catmull " + nb_iter);
            mF.sharedMesh = CatmullClark.Catmull_Clark(mF.sharedMesh, nb_iter);
        }
        mF.sharedMesh.name = gameObject.name + "Catmull Clark x" + nb_iter;
        Debug.Log(MeshDisplayInfo.ExportMeshCSV(mF.sharedMesh));
    }
}
