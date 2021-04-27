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

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
