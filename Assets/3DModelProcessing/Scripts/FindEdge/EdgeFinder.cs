using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Diagnostics;
using ThreeDModelProcessing.Utility;
using System.IO;
namespace ThreeDModelProcessing.Edge
{
    /// <summary>
    /// the mehtod of this script is based from paper below: 
    /// 1. [Extraction of blufflines from 2.5 dimensional Delaunay triangle mesh using LiDAR data]
    ///    https://etd.ohiolink.edu/apexprod/rws_olink/r/1501/10?clear=10&p10_accession_num=osu1251138890
    /// 2. [Comparing efficient data structures to represent geometric models for three-dimensional virtual medical training]
    ///    https://www.sciencedirect.com/science/article/pii/S153204641630096X
    /// </summary>
    [RequireComponent(typeof(MeshFilter), typeof(EdgeData))]
    public class EdgeFinder : MonoBehaviour
    {
        [Tooltip("Catch edge when angle between two normal is more bigger than this")]
        [Range(10, 90)] public float angle = 25f;
        [Range(0.001f, 0.01f)] public float scale = 0.01f;

        public Color color = Color.white;
        public string path = "";
        public bool isUseGUI {get{return edgeData.isUseGUI;}}
        public bool isDrawGizmos {get{return edgeData.isDrawGizmos;}}
        public string extension { get { return edgeData.extension; } set { edgeData.extension = value; } }
        public string fileName { get { return edgeData.fileName; } set { edgeData.fileName = value; } }
        public string graphName { get { return edgeData.graphName; } set { edgeData.graphName = value; } }
        int[] _triangle;
        Vector3[] vertices;


        //attributes
        EdgeData edgeData
        {
            get
            {
                return GetComponent<EdgeData>();
            }
        }
        MeshFilter meshFilter;
        Mesh mesh;
        // List<Edge> edgeList = new List<Edge>();
        public EdgeRawData edgeCollection
        {
            get
            {
                if (edgeData.edgeCollection == null)
                    edgeData.edgeCollection = new EdgeRawData();
                return edgeData.edgeCollection;
            }
            set { edgeData.edgeCollection = value; }
        }
        public EdgeGraph edgeGraph
        {
            get { return edgeData.edgeGraph; }
            set { edgeData.edgeGraph = value; }
        }
        // public EdgeGraph getEdgeGraph { get { return edgeGraph; } }

        float percentage = 0;
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
            edgeData.wholeGraphPath = path + "/" + graphName + extension;
            edgeGraph.OutputDictGraph(edgeData.wholeGraphPath);

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
                Vector3 centerNormal = MeshUtility.GetTriangleNormal(mesh, currTriangle.toArray());

                Triangle[] vert_0_adjacentTriList = edgeGraph.getAdjacentTriangleArray(vertices[vert_0]);
                Triangle[] vert_1_adjacentTriList = edgeGraph.getAdjacentTriangleArray(vertices[vert_1]);
                Triangle[] vert_2_adjacentTriList = edgeGraph.getAdjacentTriangleArray(vertices[vert_2]);

                //Edge 0 - 1
                Triangle adjacentTriangle1 = FindSameTriangleInGivenArrays(vert_0_adjacentTriList, vert_1_adjacentTriList, currTriangle);
                if (adjacentTriangle1 != null)
                {
                    Vector3 adjacentEdgeNormal = MeshUtility.GetTriangleNormal(
                        mesh,
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
                    Vector3 adjacentEdgeNormal = MeshUtility.GetTriangleNormal(
                        mesh,
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
                    Vector3 adjacentEdgeNormal = MeshUtility.GetTriangleNormal(
                        mesh,
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
            edgeData.wholeFilePah = path + "/" + fileName + extension;
            edgeCollection.SaveFile(edgeData.wholeFilePah);
            systemTimer.Stop();

            print("vertices count : " + mesh.vertexCount + " triangle count : " + mesh.triangles.Length);
            //print(systemTimer.Elapsed.Minutes + ", " + systemTimer.Elapsed.Seconds + ", " + systemTimer.Elapsed.Milliseconds);

            yield return new WaitForEndOfFrame();

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
            GL.PushMatrix();
            foreach (var edge in edgeList.edges)
            {
                Vector3 head = transform.TransformPoint(edgeList.vertices[edge.x]);
                Vector3 tail = transform.TransformPoint(edgeList.vertices[edge.y]);
                GL.Begin(GL.LINES);
                GL.Vertex3(head.x, head.y, head.z);
                GL.Vertex3(tail.x, tail.y, tail.z);
                GL.End();
            }
            GL.PopMatrix();
        }
        void OnDrawGizmos()
        {
            Gizmos.color = color;
            if (Time.time == 0 || !isDrawGizmos) return;
            foreach (var edge in edgeCollection.edges)
            {
                Vector3 head = edgeCollection.vertices[edge.x];
                Vector3 tail = edgeCollection.vertices[edge.y];

                Gizmos.DrawLine(transform.TransformPoint(head), transform.TransformPoint(tail));

                Gizmos.DrawSphere(transform.TransformPoint(head), scale);
                Gizmos.DrawSphere(transform.TransformPoint(tail), scale);
            }
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
