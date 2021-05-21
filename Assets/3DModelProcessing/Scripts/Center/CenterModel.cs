using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterModel : MonoBehaviour
{
    MeshFilter meshFilter;
    void Center(){
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

    public static void CenterObject(Mesh mesh){
        Vector3[] vertices = mesh.vertices;
        Vector3 meanPosition = Vector3.zero;

        foreach(var p in vertices){
            meanPosition += p / vertices.Length;
        }

        for(int i=0; i<vertices.Length; i++){
            vertices[i] += meanPosition;
        }
        print(meanPosition);
        mesh.vertices = vertices;
    }

    public static void CenterObject(MeshFilter meshFilter, MeshRenderer renderer){
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

    void Awake(){
        meshFilter = GetComponent<MeshFilter>();
        // Center();
    }
    void OnEnable(){
        // Center();
        CenterObject(GetComponent<MeshFilter>().mesh);
        print(GetComponent<MeshRenderer>().bounds.center);
    }
}
