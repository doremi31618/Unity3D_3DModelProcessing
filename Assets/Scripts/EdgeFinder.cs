using System;
using System.Linq;
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
        public Vector3[] edgeVertex = new Vector3[2];
        public Edge(int[] _edgeData){
            edgeData = _edgeData;
        }
        public Edge(Vector3[] _edgeVertex){
            edgeVertex = _edgeVertex;
        }
        public Edge(){
        }
        public override string ToString(){
            return "Edge point 1 : " + edgeVertex[0] + "Edge point 2 : " + edgeVertex[1];
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
            // print(triangleNormal[i] .normalized);
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

    void GenerateProceduralMesh(){
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[]{
            new Vector3(0, 0, 0),
            new Vector3(0,-1, 0),
            new Vector3(1, 0, 0),
            new Vector3(0, 0, 1)
        };
        int[] indices = new int[]{0, 1, 3, 3, 1, 2};
        mesh.vertices = vertices;
        mesh.triangles = indices;

        GetComponent<MeshFilter>().mesh = mesh;

    }
    void PrintMeshData(){
        print("Dot product"+Vector3.Dot(new Vector3(0,0,0), new Vector3(0, 1, 0)));
        print(String.Join(",", mesh.vertices));
        print(String.Join(",", mesh.triangles));
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
            Vector3[] triVertex = GetTriangleVertex(sortIndex);
            // Array.Sort(sortIndex);

            //finding adjacent faces in mesh
            int adjacent_triangle_num = 0;
            Vector3 normal = triangleNormal[i].normalized;

            //traverse all triangle to find the adjacent triangle
            for (int j = i+1; j < triangleCount; j++)
            {
                if (adjacent_triangle_num == 3 )break;
                
                int[] adjacent_index_clone = GetTriangle(j);
                Vector3[] adjacentTriVertex = GetTriangleVertex(adjacent_index_clone);

                //find duplicate numbers in two array
                IEnumerable<Vector3> sameEdge = adjacentTriVertex.Intersect(adjacentTriVertex);
                
                if(sameEdge.Count() <2)continue;
                Vector3 adjacent_triangle_normal =  triangleNormal[j].normalized;
                Vector3 center1 = GetTriangleCenter(triVertex);
                Vector3 center2 = GetTriangleCenter(adjacentTriVertex);
                //if there are edges of triangle, add edge data to the data structure 
                bool isEdge = Mathf.Abs(Vector3.Dot(adjacent_triangle_normal, normal)) <= Mathf.Cos(25 * Mathf.PI / 180);
                if(isEdge){
                    print("normal1 : " + adjacent_triangle_normal);
                    print("normal2 : " + normal);
                    print("dot product : " + Vector3.Dot(adjacent_triangle_normal, normal));
                    Edge edge = new Edge(sameEdge.ToArray());
                    Edge edge_center1 = new Edge(new Vector3[]{
                        center1,center1 + normal
                    });
                    Edge edge_center2 = new Edge(new Vector3[]{
                        center2,center2 + adjacent_triangle_normal
                    });
                    edgeList.Add(edge);
                    edgeList.Add(edge_center1);
                    edgeList.Add(edge_center2);
                }else{
                    print("its not a edge");
                }
            }

            
        }
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
        // GenerateProceduralMesh();
        mesh = meshFiler.mesh;
        
    }
    // Start is called before the first frame update
    void Start()
    {
        // PrintMeshData();
        method_1();
    }

    // Update is called once per frame
    void Update()
    {
        // method_1();
    }
    void OnDrawGizmos(){
        Gizmos.color = Color.red;
        foreach(var e in edgeList){
            Vector3 direction = e.edgeVertex[1] - e.edgeVertex[0];
            Gizmos.DrawRay(e.edgeVertex[0], direction);
            
            Gizmos.DrawSphere(transform.TransformPoint(e.edgeVertex[0]), 0.01f);
            Gizmos.DrawSphere(transform.TransformPoint(e.edgeVertex[1]), 0.01f);
        }
    }
}
