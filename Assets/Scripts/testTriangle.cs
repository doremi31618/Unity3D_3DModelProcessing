using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class testTriangle : MonoBehaviour
{
    void GenerateMesh(){
        Mesh mesh = new Mesh();
        Vector3[] vertex = new Vector3[4];
        vertex[0] = new Vector3(-1, 1, 0);
        vertex[1] = new Vector3(-1,-1, 0);
        vertex[2] = new Vector3( 1,-1, 0);
        vertex[3] = new Vector3( 1, 1, 0);
        int[] triangle = new int[]{1,0,2,2,0,3};
        mesh.vertices = vertex;
        mesh.triangles =  triangle;
        mesh.uv = new Vector2[]{
            new Vector2(0,1),
            new Vector2(0,0),
            new Vector2(1,0),
            new Vector2(1,1)
        };
        foreach(var t in GetComponent<MeshFilter>().mesh.triangles){
            print(t);
        }
        GetComponent<MeshFilter>().mesh = mesh;
    }
    // Start is called before the first frame update
    void Start()
    {
        GenerateMesh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
