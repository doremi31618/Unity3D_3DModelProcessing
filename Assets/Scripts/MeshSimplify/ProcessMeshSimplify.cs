using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessMeshSimplify : MonoBehaviour
{

    [Range(0,1)]public float quality = 0.05f;
    void SimplifyMesh(){
        Mesh sourceMesh = GetComponent<MeshFilter>().mesh;
        var meshSimplifier = new UnityMeshSimplifier.MeshSimplifier();
        meshSimplifier.Initialize(sourceMesh);
        meshSimplifier.SimplifyMesh(quality);
        GetComponent<MeshFilter>().mesh = meshSimplifier.ToMesh();
    }
    // Start is called before the first frame update
    void Awake(){
        SimplifyMesh();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
