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
        public bool isUseGUI = false;
        public Color color = Color.white;
        public string path = "";
        public string extension = ".txt";
        public string fileName = "edgeData";
        public string graphName = "edgeGraph";
        int[] _triangle;
        Vector3[] vertices;


        //attributes
        MeshFilter meshFilter;
        Mesh mesh;
        // List<Edge> edgeList = new List<Edge>();
        public EdgeRawData edgeCollection = new EdgeRawData();
        EdgeGraph edgeGraph;
        public EdgeGraph getEdgeGraph { get { return edgeGraph; } }

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
            // print("============GetTriangleNormal Input Vector===========");
            // print(vertices[0]*10000);
            // print(vertices[1]*10000);
            // print(vertices[2]*10000);
            Vector3 vert1 = vertices[1] * 10000 - vertices[0] * 10000;
            Vector3 vert2 = vertices[2] * 10000 - vertices[0] * 10000;
            // print("============substract===========");
            // print(vert1);
            // print(vert2);
            Vector3 crossProduct = Vector3.Cross(vert1.normalized, vert2.normalized).normalized;
            // print("Cross"+crossProduct);
            return crossProduct;
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
        #region method 2
        IEnumerator method_2_optimize_coroutine()
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
            // print("vertex count" + vertexCount);
            // print("triangle count" + triangleCount);
            edgeGraph = new EdgeGraph(vertexCount, true);
            edgeGraph.InitEdgeGraphDict(vertexCount);
            for (int i = 0; i < triangleCount / 3; i++)
            {
                percentage = (float)i / (float)triangleCount / 6;
                int triangleIndex = i * 3;

                Triangle newTriangle = new Triangle();
                newTriangle.vertex1 = vertices[triangles[triangleIndex]];
                newTriangle.vertex2 = vertices[triangles[triangleIndex + 1]];
                newTriangle.vertex3 = vertices[triangles[triangleIndex + 2]];

                edgeGraph.addTriangle(vertices[triangles[triangleIndex]], newTriangle);
                edgeGraph.addTriangle(vertices[triangles[triangleIndex + 1]], newTriangle);
                edgeGraph.addTriangle(vertices[triangles[triangleIndex + 2]], newTriangle);

                edgeGraph.addEdge(
                    vertices[triangles[triangleIndex]],
                    vertices[triangles[triangleIndex + 1]]);
                edgeGraph.addEdge(
                    vertices[triangles[triangleIndex]],
                    vertices[triangles[triangleIndex + 2]]);
                edgeGraph.addEdge(
                    vertices[triangles[triangleIndex + 1]],
                    vertices[triangles[triangleIndex + 2]]);

                if (systemTimer.ElapsedMilliseconds > nextTime)
                {
                    nextTime = systemTimer.ElapsedMilliseconds + 500;
                    yield return new WaitForEndOfFrame();
                }
            }

            if (path == "")
                path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            edgeGraph.OutputDictGraph(path + "/" + graphName + extension);

            //clear origin data 
            edgeCollection.Clear();

            //Build normal graph 
            for (int i = 0; i < vertexCount; i++)
            {
                percentage = ((float)i / (float)vertexCount) / 4 + 0.5f;
                //Get Vertex
                Vector3 vertex = vertices[i];
                Vector3 averageNormal = edgeGraph.caculateVertexAverageNoraml(vertex);
                edgeGraph.addNormal(vertex, averageNormal);

                if (systemTimer.ElapsedMilliseconds > nextTime)
                {
                    nextTime = systemTimer.ElapsedMilliseconds + 500;
                    yield return new WaitForEndOfFrame();
                }
            }

            for (int i = 0; i < vertexCount; i++)
            {
                percentage = ((float)i / (float)vertexCount) / 4 + 0.75f;
                Vector3 vertex = vertices[i];
                Vector3 averageNormal = Vector3.zero;
                List<Triangle> adjacentTriangle = edgeGraph.getAdjacentTriangleList(vertex);
                Vector3 normal = edgeGraph.getVertexAverageNormal(vertex);

                //check each adjacent triangle of vertex
                foreach (var triangle in adjacentTriangle)
                {
                    //find common edge 
                    Vector3[] commonEdge = triangle.edgeExcludeVertex(vertex);
                    Triangle adjacentCell = null;
                    //find adjacent triangle 
                    if (commonEdge != null)
                    {
                        Triangle[] vert0_adjacentTriangleList = edgeGraph.getAdjacentTriangleArray(commonEdge[0]);
                        Triangle[] vert1_adjacentTriangleList = edgeGraph.getAdjacentTriangleArray(commonEdge[1]);
                        adjacentCell = FindSameTriangleInGivenArrays(
                            vert0_adjacentTriangleList,
                            vert1_adjacentTriangleList,
                            triangle
                        );
                    }
                    else
                    {
                        print("vertex not in the edge");
                        continue;
                    }

                    //compare 2 normal (if so add edge to list)
                    if (adjacentCell == null) continue;
                    float dotProduct = Vector3.Dot(adjacentCell.normal.normalized, normal.normalized);
                    if (dotProduct < Mathf.Cos(angle * Mathf.Deg2Rad))
                    {
                        edgeCollection.AddEdge(commonEdge[0], commonEdge[1]);
                    }
                }

                if (systemTimer.ElapsedMilliseconds > nextTime)
                {
                    nextTime = systemTimer.ElapsedMilliseconds + 500;
                    yield return new WaitForEndOfFrame();
                }
            }
            edgeCollection.SaveFile(path + "/" + fileName + extension);
            edgeGraph = EdgeGraph.ConvertEdgeRawDataToGraph(edgeCollection, true);
        }

        #endregion //method 2 end
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
            // print("vertex count" + vertexCount);
            // print("triangle count" + triangleCount);
            edgeGraph = new EdgeGraph(vertexCount, true);
            edgeGraph.InitEdgeGraphDict(vertexCount);
            for (int i = 0; i < triangleCount / 3; i++)
            {
                percentage = (float)i / (float)triangleCount / 6;
                int triangleIndex = i * 3;

                Triangle newTriangle = new Triangle();
                newTriangle.vertex1 = vertices[triangles[triangleIndex]];
                newTriangle.vertex2 = vertices[triangles[triangleIndex + 1]];
                newTriangle.vertex3 = vertices[triangles[triangleIndex + 2]];

                edgeGraph.addTriangle(vertices[triangles[triangleIndex]], newTriangle);
                edgeGraph.addTriangle(vertices[triangles[triangleIndex + 1]], newTriangle);
                edgeGraph.addTriangle(vertices[triangles[triangleIndex + 2]], newTriangle);

                edgeGraph.addEdge(
                    vertices[triangles[triangleIndex]],
                    vertices[triangles[triangleIndex + 1]]);
                edgeGraph.addEdge(
                    vertices[triangles[triangleIndex]],
                    vertices[triangles[triangleIndex + 2]]);
                edgeGraph.addEdge(
                    vertices[triangles[triangleIndex + 1]],
                    vertices[triangles[triangleIndex + 2]]);

                if (systemTimer.ElapsedMilliseconds > nextTime)
                {
                    nextTime = systemTimer.ElapsedMilliseconds + 500;
                    yield return new WaitForEndOfFrame();
                }
            }

            //Build normal graph 
            for (int i = 0; i < vertexCount; i++)
            {
                percentage = ((float)i / (float)vertexCount) / 4 + 0.5f;
                //Get Vertex
                Vector3 vertex = vertices[i];
                Vector3 averageNormal = edgeGraph.caculateVertexAverageNoraml(vertex);
                edgeGraph.addNormal(vertex, averageNormal);

                if (systemTimer.ElapsedMilliseconds > nextTime)
                {
                    nextTime = systemTimer.ElapsedMilliseconds + 500;
                    yield return new WaitForEndOfFrame();
                }
            }

            if (path == "")
                path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            edgeGraph.OutputDictGraph(path + "/" + graphName + extension);

            //clear origin data 
            edgeCollection.Clear();

            // retrive every triangle (triangleCount / 3
            for (int i = 0; i < triangleCount; i += 3)
            {

                percentage = (float)i / (float)triangleCount / 2 + 0.5f;
                int triangleIndex = i;
                int vert_0 = triangles[triangleIndex];
                int vert_1 = triangles[triangleIndex + 1];
                int vert_2 = triangles[triangleIndex + 2];

                Triangle currTriangle = new Triangle();
                currTriangle.vertex1 = vertices[vert_0];
                currTriangle.vertex2 = vertices[vert_1];
                currTriangle.vertex3 = vertices[vert_2];
                Vector3 centerNormal = GetTriangleNormal(currTriangle.toArray());

                Triangle[] vert_0_adjacentTriList = edgeGraph.getAdjacentTriangleArray(vertices[vert_0]);
                Triangle[] vert_1_adjacentTriList = edgeGraph.getAdjacentTriangleArray(vertices[vert_1]);
                Triangle[] vert_2_adjacentTriList = edgeGraph.getAdjacentTriangleArray(vertices[vert_2]);

                //Edge 0 - 1
                Triangle adjacentTriangle1 = FindSameTriangleInGivenArrays(vert_0_adjacentTriList, vert_1_adjacentTriList, currTriangle);
                if (adjacentTriangle1 != null)
                {
                    Vector3 adjacentEdgeNormal = GetTriangleNormal(
                        new Vector3[]{
                            adjacentTriangle1.vertex1,
                            adjacentTriangle1.vertex2,
                            adjacentTriangle1.vertex3
                        }
                    );
                    float dot_result = Vector3.Dot(adjacentEdgeNormal, centerNormal);
                    // print("adjacentEdgeNormal"+adjacentEdgeNormal);
                    bool isEdge = dot_result <= Mathf.Cos(angle * Mathf.Deg2Rad);
                    if (isEdge)
                    {
                        // print (dot_result +　" cos : " + Mathf.Cos(angle * Mathf.PI / 180));
                        edgeCollection.AddEdge(currTriangle.vertex1, currTriangle.vertex2);
                    }
                }

                //Edge 0 - 2
                Triangle adjacentTriangle2 = FindSameTriangleInGivenArrays(vert_0_adjacentTriList, vert_2_adjacentTriList, currTriangle);
                if (adjacentTriangle2 != null)
                {
                    Vector3 adjacentEdgeNormal = GetTriangleNormal(
                        new Vector3[]{
                            adjacentTriangle2.vertex1,
                            adjacentTriangle2.vertex2,
                            adjacentTriangle2.vertex3
                        }
                    );
                    float dot_result = Vector3.Dot(adjacentEdgeNormal, centerNormal);
                    // print("adjacentEdgeNormal"+adjacentEdgeNormal);
                    bool isEdge = dot_result <= Mathf.Cos(angle * Mathf.Deg2Rad);
                    if (isEdge)
                    {
                        // print (dot_result +　" cos : " + Mathf.Cos(angle * Mathf.PI / 180));
                        // edgeCollection.AddEdge(triangleVertices[0], triangleVertices[1]);
                        edgeCollection.AddEdge(currTriangle.vertex1, currTriangle.vertex3);
                    }
                }

                //Edge 1 - 2
                Triangle adjacentTriangle3 = FindSameTriangleInGivenArrays(vert_1_adjacentTriList, vert_2_adjacentTriList, currTriangle);
                if (adjacentTriangle3 != null)
                {
                    Vector3 adjacentEdgeNormal = GetTriangleNormal(
                        new Vector3[]{
                            adjacentTriangle3.vertex1,
                            adjacentTriangle3.vertex2,
                            adjacentTriangle3.vertex3
                        }
                    );
                    float dot_result = Vector3.Dot(adjacentEdgeNormal, centerNormal);
                    // print("adjacentEdgeNormal"+adjacentEdgeNormal);
                    bool isEdge = dot_result <= Mathf.Cos(angle * Mathf.Deg2Rad);
                    if (isEdge)
                    {
                        edgeCollection.AddEdge(currTriangle.vertex2, currTriangle.vertex3);
                    }
                }

                // // if over run time pass for end of frame
                if (systemTimer.ElapsedMilliseconds > nextTime)
                {
                    nextTime = systemTimer.ElapsedMilliseconds + 500;
                    yield return new WaitForEndOfFrame();
                }
            }
            List<Vector3> _normals = new List<Vector3>();
            foreach (var edgeVertex in edgeCollection.vertices)
            {
                _normals.Add(edgeGraph.getVertexAverageNormal(edgeVertex));
                if (systemTimer.ElapsedMilliseconds > nextTime)
                {
                    nextTime = systemTimer.ElapsedMilliseconds + 500;
                    yield return new WaitForEndOfFrame();
                }
            }
            edgeCollection.AddNormal(_normals);

            if (fileName == "")
            {
                fileName = transform.parent.gameObject.name + extension;
            }
            edgeCollection.SaveFile(path + "/" + fileName + extension);
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

        static Triangle FindSameTriangleInGivenArrays(Triangle[] triArr1, Triangle[] triArr2, Triangle exclude)
        {
            var hashSet = new HashSet<Triangle>(triArr1);
            if (triArr1.Length <= triArr2.Length)
            {
                hashSet = new HashSet<Triangle>(triArr2);
                for (int i = 0; i < triArr1.Length; i++)
                {
                    if (hashSet.Contains(triArr1[i]) && !triArr1[i].Equals(exclude))
                        return triArr1[i];
                }
            }
            else
            {
                hashSet = new HashSet<Triangle>(triArr1);
                for (int i = 0; i < triArr2.Length; i++)
                {
                    if (hashSet.Contains(triArr2[i]) && !triArr2[i].Equals(exclude))
                        return triArr2[i];
                }
            }
            return null;
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
            if (!isUseGUI) return;
            Rect btn = new Rect(50, 50, 150, 50);
            if (GUI.Button(btn, "method 1"))
            {
                // method_1();
                // StartCoroutine(method_1_coroutine());
                StartCoroutine(method_1_optimize_coroutine());
            }

            Rect btn2 = new Rect(250, 50, 150, 50);
            if (GUI.Button(btn2, "method 2 "))
            {
                // method_1();
                StartCoroutine(method_2_optimize_coroutine());
            }

            GUI.Label(new Rect(50, 100, 150, 50), "Loading : " + percentage * 100 + "%");
        }
    }

}
