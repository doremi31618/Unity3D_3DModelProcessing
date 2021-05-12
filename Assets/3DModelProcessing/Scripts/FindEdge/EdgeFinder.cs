using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
namespace ThreeDModelProcessing
{
    

    /// <summary>
    /// the mehtod of this script is based from paper below: 
    /// 1. [Extraction of blufflines from 2.5 dimensional Delaunay triangle mesh using LiDAR data]
    ///    https://etd.ohiolink.edu/apexprod/rws_olink/r/1501/10?clear=10&p10_accession_num=osu1251138890
    /// 2. [Comparing efficient data structures to represent geometric models for three-dimensional virtual medical training]
    ///    https://www.sciencedirect.com/science/article/pii/S153204641630096X
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    public class EdgeFinder : MonoBehaviour
    {
        [Tooltip("Catch edge when angle between two normal is more bigger than this")]
        [Range(10, 90)]public float angle = 25f;
        [Range(0.005f, 0.01f)]public float scale = 0.01f;
        public string path = "";
        public int[] _triangle;
        public Vector3[] vertices;

        struct Vertex3
        {
            public float x;
            public float y;
            public float z;

        }

        //attributes
        MeshFilter meshFilter;
        Mesh mesh;
        // List<Edge> edgeList = new List<Edge>();
        EdgeRawData edgeCollection = new EdgeRawData();

        float percentage = 0;

        //could encapsulation to a class later
        //tool method 
        int GetTrianglesArrayLength
        {
            get
            {
                return mesh.triangles.Length / 3;
            }
        }
        Vector3 GetVertex(int index)
        {
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
            for (int i = 0; i < length; i++)
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
            return Vector3.Cross(vertices[1] - vertices[0], vertices[2] - vertices[0]);
        }

        Vector3 GetTriangleCenter(Vector3[] vertices)
        {
            Vector3 center = Vector3.zero;
            foreach (var v in vertices)
            {
                center += v / 3;
            }
            return center;

        }

        void GenerateProceduralMesh()
        {
            Mesh mesh = new Mesh();
            Vector3[] vertices = new Vector3[]{
            new Vector3(1,-1, 1),
            new Vector3(1,-1,-1),
            new Vector3(1, 1,-1),
            new Vector3(1, 1, 1),
            new Vector3(-1, -1, 1),
            new Vector3(-1, -1, -1),
            new Vector3(-1, 1, -1),
            new Vector3(-1, 1, 1)
            };
            int[] triangle = new int[]{
            4,0,3,
            4,3,7,
            0,1,2,
            0,2,3,
            1,5,6,
            1,6,2,
            5,4,7,
            5,7,6,
            7,3,2,
            7,2,6,
            0,5,1,
            0,4,5
            };
            mesh.vertices = vertices;
            mesh.triangles = triangle;

            GetComponent<MeshFilter>().mesh = mesh;

        }
        void PrintMeshData()
        {
            print("Dot product" + Vector3.Dot(new Vector3(0, 0, 0), new Vector3(0, 1, 0)));
            print(String.Join(",", mesh.vertices));
            print(String.Join(",", mesh.triangles));
        }
        IEnumerator method_2_optimize_coroutine(){
            Stopwatch systemTimer = new Stopwatch();
            float nextTime = 500;
            systemTimer.Start();

            //transform model to graph
            int vertexCount = meshFilter.mesh.vertexCount;
            int triangleCount = meshFilter.mesh.triangles.Length;
            int[] triangles = meshFilter.mesh.triangles;
            _triangle = triangles;
            vertices = meshFilter.mesh.vertices;
            print("vertex count"+ vertexCount);
            print("triangle count" + triangleCount);
            EdgeGraph edgeGraph = new EdgeGraph(vertexCount);
            for(int i=0; i<triangleCount/3; i++){
                percentage = (float)i / (float)triangleCount/3;
                int triangleIndex = i*3;
                edgeGraph.addEdge(triangles[triangleIndex],  triangles[triangleIndex+1]);
                edgeGraph.addEdge(triangles[triangleIndex+1],triangles[triangleIndex+2]);
                edgeGraph.addEdge(triangles[triangleIndex+2],  triangles[triangleIndex]);
                if (systemTimer.ElapsedMilliseconds > nextTime){
                    nextTime = systemTimer.ElapsedMilliseconds + 500;
                    yield return new WaitForEndOfFrame();
                }
            }

            edgeGraph.OutputGraph(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/" + "EdgeGraph.txt");
            // retrive every vertex
            for (int i=0; i<vertexCount; i++){
                Vector3 averageNormal = Vector3.zero;
                
            }
            
        }
        IEnumerator method_1_optimize_coroutine(){
            Stopwatch systemTimer = new Stopwatch();
            float nextTime = 500;
            systemTimer.Start();

            //transform model to graph
            int vertexCount = meshFilter.mesh.vertexCount;
            int triangleCount = meshFilter.mesh.triangles.Length;
            int[] triangles = meshFilter.mesh.triangles;
            _triangle = triangles;
            vertices = meshFilter.mesh.vertices;
            print("vertex count"+ vertexCount);
            print("triangle count" + triangleCount);
            EdgeGraph edgeGraph = new EdgeGraph(vertexCount);
            for(int i=0; i<triangleCount/3; i++){
                percentage = (float)i / (float)triangleCount/3;
                int triangleIndex = i*3;
                edgeGraph.addEdge(triangles[triangleIndex],  triangles[triangleIndex+1]);
                edgeGraph.addEdge(triangles[triangleIndex+1],triangles[triangleIndex+2]);
                edgeGraph.addEdge(triangles[triangleIndex+2],  triangles[triangleIndex]);
                if (systemTimer.ElapsedMilliseconds > nextTime){
                    nextTime = systemTimer.ElapsedMilliseconds + 500;
                    yield return new WaitForEndOfFrame();
                }
            }

            edgeGraph.OutputGraph(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/" + "EdgeGraph.txt");
            // 
            
        }

        

        IEnumerator method_1_coroutine()
        {
            if (path == "")
                path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/" + "edgeData.txt";
            Stopwatch timer = new Stopwatch();
            StreamWriter sw = new StreamWriter(path);

            timer.Start();
            long nextTime = timer.ElapsedMilliseconds + 1000;
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

                percentage = (float)i / (float)triangleCount;

                //traverse all triangle to find the adjacent triangle
                for (int j = i + 1; j < triangleCount; j++)
                {
                    if (adjacent_triangle_num == 3) break;

                    int[] adjacent_index_clone = GetTriangle(j);
                    Vector3[] adjacentTriVertex = GetTriangleVertex(adjacent_index_clone);

                    List<Vector3> sameEdge = new List<Vector3>();
                    for (int x = 0; x < 3; x++)
                    {
                        for (int y = 0; y < 3; y++)
                        {
                            if (triVertex[x] == adjacentTriVertex[y])
                            {
                                sameEdge.Add(triVertex[x]);
                            }
                        }
                    }

                    if (sameEdge.Count() < 2) continue;
                    Vector3 adjacent_triangle_normal = triangleNormal[j].normalized;
                    bool isEdge = Mathf.Abs(Vector3.Dot(adjacent_triangle_normal, normal)) <= Mathf.Cos(angle * Mathf.PI / 180);
                    if (isEdge)
                    {
                        edgeCollection.AddEdge(sameEdge[0], sameEdge[1]);
                        // Edge edge = new Edge(sameEdge.ToArray());
                        // edgeList.Add(edge);

                        
                    }
                    if (timer.ElapsedMilliseconds > nextTime)
                    {
                        print("Process time : " + timer.ElapsedMilliseconds);
                        yield return new WaitForEndOfFrame();
                        nextTime = timer.ElapsedMilliseconds + 200;
                    }

                }
            }

            string edgeJSON = JsonUtility.ToJson(edgeCollection);
            sw.Write(edgeJSON);

            sw.Close();
            timer.Stop();
            
            print("vertices count : " + mesh.vertexCount + " triangle count : " + mesh.triangles.Length);
            print( timer.Elapsed.Minutes +", " + timer.Elapsed.Seconds + ", " + timer.Elapsed.Milliseconds);
            yield return new WaitForEndOfFrame();
        }

        void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            // GenerateProceduralMesh();
            mesh = meshFilter.mesh;

        }

        void DrawEdge(EdgeRawData edgeList){
            foreach (var edge in edgeList.edges){
                Vector3 head = edgeList.vertices[edge.x];
                Vector3 tail = edgeList.vertices[edge.y];

                Gizmos.DrawLine(transform.TransformPoint(head), transform.TransformPoint(tail));

                Gizmos.DrawSphere(transform.TransformPoint(head), scale);
                Gizmos.DrawSphere(transform.TransformPoint(tail), scale);
            }
        }
        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            DrawEdge(edgeCollection);
        }

        void OnGUI()
        {
            Rect btn = new Rect(50, 50, 150, 50);
            if (GUI.Button(btn, "method 1"))
            {
                // method_1();
                StartCoroutine(method_1_coroutine());
            }

            Rect btn2 = new Rect(250, 50, 150, 50);
            if (GUI.Button(btn2, "method 1 optimize"))
            {
                // method_1();
                StartCoroutine(method_1_optimize_coroutine());
            }

            GUI.Label(new Rect(50, 100, 150, 50), "Loading : " + percentage * 100 + "%");
        }
    }

}
