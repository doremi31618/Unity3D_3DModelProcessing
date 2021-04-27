using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRenderMode : MonoBehaviour
{
    [Tooltip("Currently only support Lines, Points, Triangles")]
    public MeshTopology renderMode;
    public Material pointMaterial;

    void SettingLineRenderMode(){
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        GetComponent<MeshRenderer>().material = pointMaterial;
        Vector3[] old_vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        Vector3[] new_vertices = new Vector3[triangles.Length];
        
        int[] indices = new int[new_vertices.Length*2];
        //use triangle to re-write
        for(int i=0; i<triangles.Length; i+=3){
            new_vertices[i] = old_vertices[triangles[i]];
            new_vertices[i+1] = old_vertices[triangles[i+1]];
            new_vertices[i+2] = old_vertices[triangles[i+2]];

            indices[i*2] = triangles[i];
            indices[i*2+1] = triangles[i+1];

            indices[i*2+2] = triangles[i+1];
            indices[i*2+3] = triangles[i+2];

            indices[i*2+4] = triangles[i+2];
            indices[i*2+5] = triangles[i];
        }

        mesh.SetIndices(indices, MeshTopology.Lines, 0);
        mesh.RecalculateBounds();
        // mesh.RecalculateNormals();
    }

    void SettingPointRenderMode(){
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        GetComponent<MeshRenderer>().material = pointMaterial;
        int vertexCount = mesh.vertexCount;
        int[] indices = new int[vertexCount];
        for(int i=0; i<vertexCount; i++){
            indices[i] = i;
        }
        mesh.SetIndices(indices, MeshTopology.Points, 0, true, 0);
    }

    //also low poly effect 
    // reference from : https://www.sohu.com/a/379511390_120511628
    void SettingTriangleRenderMode(){
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        Vector3[] new_vertices = new Vector3[triangles.Length];
        for(int i=0; i<triangles.Length; i++){
            new_vertices[i] = vertices[triangles[i]];
            triangles[i] = i;
        }
        mesh.vertices = new_vertices;
        mesh.triangles = triangles;
        mesh.SetIndices(triangles, MeshTopology.Triangles, 0, true, 0);

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    void SettingRenderMode(MeshTopology _renderMode){
        switch(_renderMode){
            case MeshTopology.Points:
                SettingPointRenderMode();
                break;
            case MeshTopology.Triangles:
                SettingTriangleRenderMode();
                break;
            case MeshTopology.Lines:
                SettingLineRenderMode();
                break;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        SettingRenderMode(renderMode);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
