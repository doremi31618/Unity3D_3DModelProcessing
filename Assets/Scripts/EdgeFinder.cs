using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// the mehtod of this script is based from paper : https://etd.ohiolink.edu/apexprod/rws_olink/r/1501/10?clear=10&p10_accession_num=osu1251138890
/// </summary>
[RequireComponent(typeof(MeshFilter))]
public class EdgeFinder : MonoBehaviour
{
    class Edge {
        public int[] edgeData = new int[2];
        public Edge(int[] _edgeData){
            edgeData = _edgeData;
        }

        public Edge(){
        }
        public override string ToString(){
            return "Edge point 1 : " + edgeData[0] + "Edge point 2 : " + edgeData[1];
        } 
    }

    //attributes
    MeshFilter meshFiler;
    Mesh mesh;
    List<Edge> edgeList = new List<Edge>();

    //could encapsulation to a class later
    //tool method 
    int GetTrianglesArrayLength
    {
        get
        {
            return mesh.triangles.Length / 3;
        }
    }
    Vector3 GetVertex(int index){
        return mesh.vertices[index];
    }

    int[] GetTriangle(int index)
    {
        return new int[]{
            mesh.triangles[index*3],
            mesh.triangles[index*3+1],
            mesh.triangles[index*3+2]};
    }

    Vector3[] GetTriangleVertex(int[] indices)
    {
        return new Vector3[]{
            mesh.vertices[indices[0]],
            mesh.vertices[indices[1]],
            mesh.vertices[indices[2]]};
    }

    Vector3[] GetTriangleNormalCollection(int length)
    {
        Vector3[] triangleNormal = new Vector3[length];
        //traverse all the triangle
        for (int i = 0; i<length; i++)
        {
            //calculate triangle data
            int[] triangleIndex = GetTriangle(i);
            Vector3[] triangleVertex = GetTriangleVertex(triangleIndex);
            triangleNormal[i] = GetTriangleNormal(triangleVertex);
        }
        return triangleNormal;
    }

    Vector3 GetTriangleNormal(Vector3[] vertices)
    {
        return Vector3.Cross( vertices[1] - vertices[0],vertices[2] - vertices[0]);
    }

    Vector3 GetTriangleCenter(Vector3[] vertices)
    {
        Vector3 center = Vector3.zero;
        foreach(var v in vertices){
            center += v/3;
        }
        return center;
    }


    //brutal methods
    //find normal in each triangle
    void method_1()
    {
        int triangleCount = GetTrianglesArrayLength;
        //Step1 find triangle normal list 
        Vector3[] triangleNormal = GetTriangleNormalCollection(triangleCount);

        //!!! very time comsuming 
        //absolutely need to optimize
        //Step2 find edge list
        // List<Edge> edgeList = new List<Edge>();
        for (int i = 0; i < triangleCount; i++)
        {
            int[] sortIndex = GetTriangle(i);
            Array.Sort(sortIndex);

            //finding adjacent faces in mesh
            int adjacent_triangle_num = 0;
            Vector3 normal = triangleNormal[i];
            // Vector3 center = GetTriangleCenter(GetTriangleVertex(GetTriangle(i)));
            // Debug.DrawLine(center, center+normal);
            //traverse all triangle to find the adjacent triangle
            for (int j = i+1; j < triangleCount; j++)
            {
                if (adjacent_triangle_num == 3)break;
                
                int[] adjacent_index_clone = GetTriangle(j);
                Array.Sort(adjacent_index_clone);

                int[] edgeIndex = new int[2];
                int sameNumber = 0;
            
                
                for(int a=0; a<3; a++){

                    for(int b=0; b<3; b++){
                        
                        if(sortIndex[a] == adjacent_index_clone[b])
                        {
                            // print(sortIndex[k] == adjacent_index_clone[k]);
                            edgeIndex[sameNumber] = sortIndex[b];
                            sameNumber ++;
                            if (sameNumber == 2)break;
                        }
                    }
                    // print(k + "sortIndex[k] : " + sortIndex[k] + " adjacent_index_clone[k] : " + adjacent_index_clone[k]);
                    
                }
                
                if(sameNumber<2)continue;

                //debug to here
                Vector3 adjacent_triangle_normal =  triangleNormal[j].normalized;
                print(Mathf.Acos(Vector3.Dot(adjacent_triangle_normal, normal)));
                //if there are edges of triangle, add edge data to the data structure 
                if(Mathf.Acos(Vector3.Dot(adjacent_triangle_normal, normal)) >= 85.0f * Mathf.Deg2Rad){
                    Edge edge = new Edge(edgeIndex);
                    print(edge.ToString());
                    edgeList.Add(edge);
                    
                }else{
                    print("its not a edge");
                }
            }

            
        }
        // print("EdgeList length : "+ edgeList.Count);
        //step3 arrange edge data 

        //step4 build a simplify model data

    }

    //find normal of each vertex based on the surrounding triangle
    void method_2()
    {

    }

    void Awake()
    {
        meshFiler = GetComponent<MeshFilter>();
        mesh = meshFiler.mesh;
    }
    // Start is called before the first frame update
    void Start()
    {
        // method_1();
    }

    // Update is called once per frame
    void Update()
    {
        method_1();
    }
    void OnDrawGizmos(){
        Gizmos.color = Color.red;
        foreach(var e in edgeList){
            Vector3 direction =GetVertex(e.edgeData[1]) - GetVertex(e.edgeData[0]);
            Gizmos.DrawRay(transform.TransformPoint(GetVertex(e.edgeData[0])), direction);
            
            Gizmos.DrawSphere(transform.TransformPoint(GetVertex(e.edgeData[0])), 0.01f);
            Gizmos.DrawSphere(transform.TransformPoint(GetVertex(e.edgeData[1])), 0.01f);
        }
    }
}
