using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralMeshGenerator : MonoBehaviour
{
    public MeshTopology renderMode = MeshTopology.Points;
    public bool isUseCustomMesh = false;
    [Header("Custom Mesh")]
    public Mesh customMesh;

    [Header("Procedural Mesh")]
    public int vertexNumber;
    public float minInterval;
    public Material material;

    Vector3[] ProceduralPoint(int num){
        Vector3[] points = new Vector3[num];
        //write generator rule

        return points;
    }
    Mesh ProceduralGeneratePointCloud(int _vertexNumber){
        //first create basic 
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[_vertexNumber];
        int[] indices = new int[_vertexNumber];

        //determin how to render 3d model
        //if you choose to render 
        // 1. point : the "Index buffer" In most of use cases, mesh index buffer should be "identity"
        mesh.vertices = vertices;
        mesh.SetIndices(indices, MeshTopology.Points, 0);
        return mesh;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
