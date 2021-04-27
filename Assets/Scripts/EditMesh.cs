using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
public class EditMesh : MonoBehaviour
{
    [Range(0.01f, 0.5f)]public float gizmosSize = 0.1f;
    float _oldGizmosSize;
    Mesh mesh;
    Vector3[] vertices;
    Vector3[] normals;
    int[] triangles;
    public List<GameObject> handles = new List<GameObject>();
    public Vector3[] getVertices{get{return vertices;}}
    public Vector3[] getNormals{get{return normals;}}
    public int[] getTriangles{get{return triangles;}}
    public int vertexCount{get{return vertices.Length;}}
    public int triangleCount{get{
        if (triangles!=null)
            return triangles.Length/3;
        else
            return 0;
        }}
    public GameObject[] GetTriangleVertices(int index){
        GameObject[] triangleVertices = {
            handles[triangles[index*3]], 
            handles[triangles[index*3+1]],
            handles[triangles[index*3+2]]};
        return triangleVertices;
    }

    void OnValueChange(){
        if (_oldGizmosSize != gizmosSize){
            foreach(var handle in handles){
                handle.GetComponent<VertexGizmos>().Initialize(gizmosSize);
                _oldGizmosSize = gizmosSize;
            }
        }
    }

    void UpdateModel(){
        Debug.Log("Update model");
        Vector3[] _vertices = new Vector3[handles.Count];
        int index = 0;
        foreach(var handle in handles){
            _vertices[index ++] = handle.transform.localPosition;
        }
        mesh.vertices = _vertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    void OnEnable(){
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
        triangles = mesh.triangles;
        normals = mesh.normals;
        Debug.Log(vertices.Length);
        
        if(handles.Count != 0)return;
        foreach(var p in vertices){
            GameObject handle = new GameObject("MeshHandle");
            handle.transform.parent = this.transform;
            handle.transform.position = transform.localToWorldMatrix.MultiplyPoint(p);
            VertexGizmos vertexEditor = handle.AddComponent<VertexGizmos>();
            vertexEditor.Initialize(gizmosSize);
            vertexEditor.OnEditPosition += UpdateModel;
            handles.Add(handle);
        }
    }

    void OnDisable(){
        foreach(var handle in handles){
            DestroyImmediate(handle);
        }

        handles.Clear();
        vertices = null;
        triangles= null;
        normals  = null;
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        OnValueChange();
    }
}

#endif
