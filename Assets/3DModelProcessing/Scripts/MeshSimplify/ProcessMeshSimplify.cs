using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
/// <summary>
/// reference : https://github.com/Whinarn/UnityMeshSimplifier
/// </summary>
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
    void ExportObjFile(){
        ObjExporter.MeshToFile(GetComponent<MeshFilter>(), Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Simplify");
    }

    //On Test use
    void OnGUI(){
            // Rect btn = new Rect(50, 50, 150, 50);
            // if (GUI.Button(btn, "Export model"))
            // {
            //     ExportObjFile();
            // }
    }
}
