using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexNormalVisualizer : MonoBehaviour
{
    [Range(0.001f, 1)]public float length = 0.1f;
    Vector3[] normals;
        Vector3[] vertices ;
    // Start is called before the first frame update
    void Start()
    {
        normals = GetComponent<MeshFilter>().mesh.normals;
        vertices = GetComponent<MeshFilter>().mesh.vertices;
        
        print("vertex : " + vertices.Length);
        print("normals : " + normals.Length);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDrawGizmos(){
        normals = GetComponent<MeshFilter>().mesh.normals;
        vertices = GetComponent<MeshFilter>().mesh.vertices;
        
        for (int i=0; i<vertices.Length; i++){
            Vector3 normal = (normals[i] - vertices[i]) * length + vertices[i];
            Vector3 normal_color = (Vector3.one + normal.normalized)/2;
            Gizmos.color = new Color(normal_color.x, normal_color.y, normal_color.z, 1);
            Gizmos.DrawLine(transform.localToWorldMatrix * vertices[i], (transform.localToWorldMatrix * (normal)) );
            }
        


    }
}
