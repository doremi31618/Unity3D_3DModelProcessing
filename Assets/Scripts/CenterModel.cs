using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterModel : MonoBehaviour
{
    MeshFilter meshFilter;

    void Awake(){
        meshFilter = GetComponent<MeshFilter>();
    }
    void OnEnable(){
        Vector3[] vertices = meshFilter.mesh.vertices;
        Vector3 meanPosition = Vector3.zero;

        foreach(var p in vertices){
            meanPosition += p / vertices.Length;
        }

        for(int i=0; i<vertices.Length; i++){
            vertices[i] -= meanPosition;
        }

        meshFilter.mesh.vertices = vertices;
    }
}
