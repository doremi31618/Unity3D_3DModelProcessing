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
        [Range(10, 90)] public float angle = 25f;
        [Range(0.001f, 0.01f)] public float scale = 0.01f;
        public Color color = Color.white;
        public string path = "";
        public string fileName = "edgeData.txt";
        public string graphName = "edgeGraph.txt";
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
            Vector3 vert1 = vertices[1] - vertices[0];
            Vector3 vert2 = vertices[2] - vertices[0];
            return Vector3.Cross(vert1.normalized, vert2.normalized);
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
        IEnumerator method_1_optimize_coroutine()
        {
            Stopwatch systemTimer = new Stopwatch();
            float nextTime = 500;
            systemTimer.Start();

            //Build graph first
            meshFilter.mesh.Optimize();
            int vertexCount = meshFilter.sharedMesh.vertexCount;
            int triangleCount = meshFilter.sharedMesh.triangles.Length;
            int[] triangles = meshFilter.sharedMesh.triangles;
            _triangle = triangles;
            vertices = meshFilter.mesh.vertices;
            print("vertex count" + vertexCount);
            print("triangle count" + triangleCount);
            EdgeGraph edgeGraph = new EdgeGraph(vertexCount, true);
            edgeGraph.InitEdgeGraphDict(vertexCount);
            for (int i = 0; i < triangleCount / 3; i++)
            {
                percentage = (float)i / (float)triangleCount / 3 - 0.5f;
                int triangleIndex = i * 3;

                // edgeGraph.addEdge(triangles[triangleIndex], triangles[triangleIndex + 1]);
                // edgeGraph.addEdge(triangles[triangleIndex + 1], triangles[triangleIndex + 2]);
                // edgeGraph.addEdge(triangles[triangleIndex + 2], triangles[triangleIndex]);

                edgeGraph.addEdge(vertices[triangles[triangleIndex]], vertices[triangles[triangleIndex + 1]]);
                edgeGraph.addEdge(vertices[triangles[triangleIndex + 1]], vertices[triangles[triangleIndex + 2]]);
                edgeGraph.addEdge(vertices[triangles[triangleIndex + 2]], vertices[triangles[triangleIndex]]);

                if (systemTimer.ElapsedMilliseconds > nextTime)
                {
                    nextTime = systemTimer.ElapsedMilliseconds + 500;
                    yield return new WaitForEndOfFrame();
                }
            }

            if (path == "")
                path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            edgeGraph.OutputDictGraph(path + "/" + graphName);

            //clear origin data 
            edgeCollection.Clear();

            // retrive every triangle (triangleCount / 3
            for (int i = 0; i < triangleCount; i += 3)
            {
                // for (int i = 0; i < 300; i += 3)
                // {
                int triangleIndex = i;
                int vert_0 = triangles[triangleIndex];
                int vert_1 = triangles[triangleIndex + 1];
                int vert_2 = triangles[triangleIndex + 2];

                Vector3[] triangleVertices = new Vector3[]{
                            vertices[vert_0],
                            vertices[vert_1],
                            vertices[vert_2]
                };

                Vector3 centerNormal = GetTriangleNormal(triangleVertices);
                if (centerNormal == Vector3.zero)
                {
                    print(triangleIndex + " " + (triangleIndex + 1) + " " + (triangleIndex + 2) + " ");
                    print(vertices[vert_0].x + "," + vertices[vert_0].y + "," + vertices[vert_0].z + " " +
                          vertices[vert_1].x + "," + vertices[vert_1].y + "," + vertices[vert_1].z + " " +
                          vertices[vert_2].x + "," + vertices[vert_2].y + "," + vertices[vert_2].z + " ");
                }

                Vector3[] vert_0_adjacentList = edgeGraph.getAdjacentVertexArray(vertices[vert_0]);
                Vector3[] vert_1_adjacentList = edgeGraph.getAdjacentVertexArray(vertices[vert_1]);
                Vector3[] vert_2_adjacentList = edgeGraph.getAdjacentVertexArray(vertices[vert_2]);

                //compare edge1 (ver0 & vert1)
                Vector3 edge1_vert = FindSameNumberInGivenArrays(
                    vert_0_adjacentList, vert_1_adjacentList, triangleVertices[2], vertices[vert_1], vertices[vert_0]);
                // print(edge1_vert);
                if (edge1_vert != Vector3.zero)
                {
                    Vector3 adjacentEdgeNormal = GetTriangleNormal(
                        new Vector3[]{
                            vertices[vert_1],
                            vertices[vert_0],
                            edge1_vert
                        }
                    );

                    float dot_result = Vector3.Dot(adjacentEdgeNormal, centerNormal);

                    bool isEdge = dot_result <= Mathf.Cos(angle * Mathf.Deg2Rad);
                    if (isEdge)
                    {
                        // print (dot_result +　" cos : " + Mathf.Cos(angle * Mathf.PI / 180));
                        edgeCollection.AddEdge(triangleVertices[0], triangleVertices[1]);
                    }
                }

                //compare edge2 (ver0 & vert2)
                // Vector3 edge2_vert = FindSameNumberInGivenArrays(
                //     vert_0_adjacentList, vert_2_adjacentList, triangleVertices[1], vertices[vert_0], vertices[vert_2]);
                // // print(edge2_vert);
                // if (edge2_vert != Vector3.zero)
                // {
                //     Vector3 adjacentEdgeNormal = GetTriangleNormal(
                //         new Vector3[]{
                //             vertices[vert_0],
                //             vertices[vert_2],
                //             edge2_vert
                //         }
                //     );
                //     if (Vector3.Dot(adjacentEdgeNormal, centerNormal) <= Mathf.Cos(angle * Mathf.PI / 180))
                //     {
                //         edgeCollection.AddEdge(triangleVertices[0], triangleVertices[2]);
                //     }
                // }

                // //compare edge3 (ver1 & vert2)
                // Vector3 edge3_vert = FindSameNumberInGivenArrays(
                //     vert_1_adjacentList, vert_2_adjacentList, triangleVertices[0], vertices[vert_2], vertices[vert_1]);
                // // print(edge3_vert);
                // if (edge3_vert != Vector3.zero)
                // {
                //     Vector3 adjacentEdgeNormal = GetTriangleNormal(
                //         new Vector3[]{
                //             vertices[vert_2],
                //             vertices[vert_1],
                //             edge3_vert
                //         }
                //     );
                //     if (Vector3.Dot(adjacentEdgeNormal, centerNormal) <= Mathf.Cos(angle * Mathf.PI / 180))
                //     {
                //         edgeCollection.AddEdge(triangleVertices[2], triangleVertices[1]);
                //     }
                // }

                // if over run time pass for end of frame
                if (systemTimer.ElapsedMilliseconds > nextTime)
                {
                    nextTime = systemTimer.ElapsedMilliseconds + 500;
                    yield return new WaitForEndOfFrame();
                }
            }

            StreamWriter sw = new StreamWriter(path + "/" + fileName);
            string edgeJSON = JsonUtility.ToJson(edgeCollection);
            sw.Write(edgeJSON);

            sw.Close();
            systemTimer.Stop();

            print("vertices count : " + mesh.vertexCount + " triangle count : " + mesh.triangles.Length);
            print(systemTimer.Elapsed.Minutes + ", " + systemTimer.Elapsed.Seconds + ", " + systemTimer.Elapsed.Milliseconds);

            yield return new WaitForEndOfFrame();

        }

        static int FindSameNumberInGivenArrays(int[] arr1, int[] arr2)
        {
            if (arr1.Length <= arr2.Length)
            {
                var hashSet = new HashSet<int>(arr1);
                for (int i = 0; i < arr1.Length; i++)
                {
                    if (hashSet.Contains(arr2[i]))
                        return arr2[i];
                }
            }
            else
            {
                var hashSet = new HashSet<int>(arr2);
                for (int i = 0; i < arr2.Length; i++)
                {
                    if (hashSet.Contains(arr1[i]))
                        return arr1[i];
                }
            }


            //if can't find same number in given arrays , return -1 
            //(Beacause there won't be a negetive number in triangle index)
            return -1;
        }

        static Vector3 FindSameNumberInGivenArrays(Vector3[] arr1, Vector3[] arr2, Vector3 exclude, Vector3 ver1, Vector3 ver2)
        {
            var hashSet = new HashSet<Vector3>(arr1);
            if (arr1.Length <= arr2.Length)
            {
                hashSet = new HashSet<Vector3>(arr1);
                for (int i = 0; i < arr1.Length; i++)
                {
                    if (hashSet.Contains(arr2[i]) && arr2[i] != exclude)
                        hashSet.Add(arr2[i]);
                }
            }
            else
            {
                hashSet = new HashSet<Vector3>(arr2);
                for (int i = 0; i < arr2.Length; i++)
                {
                    if (hashSet.Contains(arr1[i]) && arr1[i] != exclude)
                        hashSet.Add(arr1[i]);
                }
            }

            //bug here 
            Vector3 minVector = Vector3.zero;
            float min = Mathf.Infinity;
            foreach (var v in hashSet)
            {
                float distance = Vector3.Distance(v, ver1) + Vector3.Distance(v, ver2);
                if (distance < min)
                {
                    minVector = v;
                    min = distance;
                }

            }

            return minVector;
        }



        IEnumerator method_1_coroutine()
        {
            if (path == "")
                path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/" + fileName;
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
                    bool isEdge = Vector3.Dot(adjacent_triangle_normal, normal) <= Mathf.Cos(angle * Mathf.PI / 180);
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
            print(timer.Elapsed.Minutes + ", " + timer.Elapsed.Seconds + ", " + timer.Elapsed.Milliseconds);
            yield return new WaitForEndOfFrame();
        }

        void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            // GenerateProceduralMesh();
            mesh = meshFilter.mesh;

        }

        void DrawEdge(EdgeRawData edgeList)
        {
            foreach (var edge in edgeList.edges)
            {
                Vector3 head = edgeList.vertices[edge.x];
                Vector3 tail = edgeList.vertices[edge.y];

                Gizmos.DrawLine(transform.TransformPoint(head), transform.TransformPoint(tail));

                Gizmos.DrawSphere(transform.TransformPoint(head), scale);
                Gizmos.DrawSphere(transform.TransformPoint(tail), scale);
            }
        }
        void OnDrawGizmos()
        {
            Gizmos.color = color;
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
