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
    public List<GameObject> handles = new List<GameObject>();

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
        mesh = GetComponent<MeshFilter>().sharedMesh;
        vertices = mesh.vertices;

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
