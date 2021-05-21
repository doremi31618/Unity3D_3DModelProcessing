using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
// using System.Text;
namespace ThreeDModelProcessing
{
    public class Triangle
    {
        public Vector3 vertex1;
        public Vector3 vertex2;
        public Vector3 vertex3;

        public Vector3[] toArray()
        {
            return new Vector3[] { vertex1, vertex2, vertex3 };
        }

        public Vector3 normal
        {
            get
            {
                return Vector3.Cross((vertex2 - vertex1).normalized, (vertex3 - vertex1).normalized);

            }
        }

        public Vector3[] edgeExcludeVertex(Vector3 vertex)
        {
            if (vertex == vertex1)
                return new Vector3[] { vertex2, vertex3 };
            else if (vertex == vertex2)
                return new Vector3[] { vertex1, vertex3 };
            else if (vertex == vertex3)
                return new Vector3[] { vertex1, vertex2 };
            else
                return null;
        }
    }
    [Serializable]
    public class EdgeGraph
    {
        public int nodeNumber = 0;
        public List<int>[] graph;
        public List<int>[] getGraph { get { return graph; } }
        public Dictionary<Vector3, Vector3> normalGraph;
        public Dictionary<Vector3, List<Vector3>> vertexGraph;
        public Dictionary<Vector3, List<Triangle>> triangleGraph;

        public EdgeGraph(int number, bool isUseDict)
        {
            if (isUseDict)
            {
                InitEdgeGraphDict(number);
            }
            else
            {
                InitEdgeGraph(number);
            }

            nodeNumber = number;
        }

        public void InitEdgeGraph(int number)
        {

            nodeNumber = number;
            graph = new List<int>[number];
            vertexGraph = new Dictionary<Vector3, List<Vector3>>();

            for (int i = 0; i < number; i++)
            {
                graph[i] = new List<int>();
            }

        }

        public void InitEdgeGraphDict(int number)
        {
            nodeNumber = number;
            vertexGraph = new Dictionary<Vector3, List<Vector3>>();
            triangleGraph = new Dictionary<Vector3, List<Triangle>>();
            normalGraph = new Dictionary<Vector3, Vector3>();
        }

        public void addEdge(Vector3 vertex, Vector3 adjacentPoint)
        {

            //if isn't contains key ,create one and add ajacent point to list
            if (!vertexGraph.ContainsKey(vertex))
            {
                vertexGraph.Add(vertex, new List<Vector3>());
                vertexGraph[vertex].Add(adjacentPoint);
            }
            else
            {
                if (!vertexGraph[vertex].Contains(adjacentPoint))
                {
                    vertexGraph[vertex].Add(adjacentPoint);
                }
            }

            if (!vertexGraph.ContainsKey(adjacentPoint))
            {
                vertexGraph.Add(adjacentPoint, new List<Vector3>());
                vertexGraph[adjacentPoint].Add(vertex);
            }
            else
            {
                if (!vertexGraph[adjacentPoint].Contains(vertex))
                {
                    vertexGraph[adjacentPoint].Add(vertex);
                }
            }

        }
        public void addTriangle(Vector3 vertex, Triangle triangle)
        {
            if (!triangleGraph.ContainsKey(vertex))
            {
                triangleGraph.Add(vertex, new List<Triangle>());
                triangleGraph[vertex].Add(triangle);
            }
            else if (!triangleGraph[vertex].Contains(triangle))
            {
                triangleGraph[vertex].Add(triangle);
            }
        }

        public void addNormal(Vector3 vertex, Vector3 normal)
        {
            if (!normalGraph.ContainsKey(vertex))
            {
                normalGraph[vertex] = normal;
            }
        }

        public void addEdge(int u, int v)
        {
            if (!graph[u].Contains(v))
                graph[u].Add(v);

            if (!graph[v].Contains(u))
                graph[v].Add(u);
        }

        public void OutputGraph(string path)
        {
            StreamWriter sw = new StreamWriter(path);
            for (int i = 0; i < nodeNumber; i++)
            {
                string content = i + " | ";
                foreach (var node in graph[i])
                {
                    content += node.ToString() + " ";
                }
                sw.WriteLine(content);
            }
            sw.Close();
        }

        public void OutputDictGraph(string path)
        {
            StreamWriter sw = new StreamWriter(path);
            foreach (var node in vertexGraph)
            {
                string content = node.Key + " | ";
                foreach (var adj in node.Value)
                {
                    content += adj.ToString() + " ";
                }

                sw.WriteLine(content);
            }


            sw.Close();
        }

        public int[] getAdjacentIndexArray(int index)
        {
            return graph[index].ToArray();
        }
        public List<int> getAdjacentIndexList(int index)
        {
            return graph[index];
        }

        public Vector3[] getAdjacentVertexArray(Vector3 vertex)
        {
            return vertexGraph[vertex].ToArray();
        }

        public List<Vector3> getAdjacentVertexList(Vector3 vertex)
        {
            return vertexGraph[vertex];
        }

        public Triangle[] getAdjacentTriangleArray(Vector3 vertex)
        {
            return triangleGraph[vertex].ToArray();
        }

        public List<Triangle> getAdjacentTriangleList(Vector3 vertex)
        {
            return triangleGraph[vertex];
        }


        public Vector3 getVertexAverageNormal(Vector3 vertex)
        {
            return normalGraph[vertex];
        }

        public Vector3 caculateVertexAverageNoraml(Vector3 vertex)
        {
            List<Triangle> adjacentTriangleList = getAdjacentTriangleList(vertex);
            Vector3 averageNormal = Vector3.zero;
            int triangleNumber = adjacentTriangleList.Count;
            foreach (var tri in adjacentTriangleList)
            {
                averageNormal += tri.normal / triangleNumber;
            }
            return averageNormal;
        }



        public static EdgeGraph ConvertEdgeRawDataToGraph(EdgeRawData rawData)
        {
            int nodeNum = rawData.getVertexNumber;
            EdgeGraph newGraph = new EdgeGraph(nodeNum, true);
            newGraph.InitEdgeGraphDict(nodeNum);
            for (int i = 0; i < rawData.getEdgeNumber; i++)
            {
                //get edge data 
                Vector2Int edgeIndex = rawData.getEdge(i);

                //get edge vertex   
                Vector3 edgeVertex1 = rawData.getVertex(edgeIndex.x);
                Vector3 edgeVertex2 = rawData.getVertex(edgeIndex.y);

                //save to graph
                newGraph.addEdge(edgeVertex1, edgeVertex2);

            }
            return newGraph;
        }

        public static EdgeGraph ConvertEdgeRawDataToGraph(EdgeRawData rawData, bool isSaveNormal)
        {
            if (!isSaveNormal || (rawData.normals == null))
            {
                return ConvertEdgeRawDataToGraph(rawData);
            }
            else if (rawData.normals.Count == 0)
                return ConvertEdgeRawDataToGraph(rawData);

            int nodeNum = rawData.getVertexNumber;
            EdgeGraph newGraph = new EdgeGraph(nodeNum, true);
            newGraph.InitEdgeGraphDict(nodeNum);
            for (int i = 0; i < rawData.getEdgeNumber; i++)
            {
                //get edge data 
                Vector2Int edgeIndex = rawData.getEdge(i);

                //get edge vertex   
                Vector3 edgeVertex1 = rawData.getVertex(edgeIndex.x);
                Vector3 edgeVertex2 = rawData.getVertex(edgeIndex.y);

                Vector3 vertexNormal1 = rawData.normals[edgeIndex.x];
                Vector3 vertexNormal2 = rawData.normals[edgeIndex.y];

                //save to graph
                newGraph.addEdge(edgeVertex1, edgeVertex2);
                newGraph.addNormal(edgeVertex1, vertexNormal1);
                newGraph.addNormal(edgeVertex2, vertexNormal2);

            }
            return newGraph;
        }
    }


    [Serializable]
    public class EdgeRawData
    {

        public List<Vector3> vertices;
        public List<Vector2Int> edges;
        public int getVertexNumber { get { return vertices.Count; } }
        public int getEdgeNumber { get { return edges.Count; } }
        // public List<Vector3> getNormals {get {return normals;}}
        public List<Vector3> normals;

        public EdgeRawData()
        {

            vertices = new List<Vector3>();
            edges = new List<Vector2Int>();
        }
        public void Clear()
        {
            vertices.Clear();
            edges.Clear();
        }
        public Vector2Int getEdge(int index)
        {
            return edges[index];
        }

        public Vector3 getVertex(int index)
        {
            return vertices[index];
        }

        public void AddEdge(Vector3 head, Vector3 tail)
        {
            int headIndex = 0, tailIndex = 0;
            bool isContainHead = false, isContainTail = false;
            if (!vertices.Contains(head))
            {
                vertices.Add(head);
                headIndex = vertices.Count - 1;
            }
            else
            {
                headIndex = vertices.FindIndex(x => x == head);
                isContainHead = true;
            }

            if (!vertices.Contains(tail))
            {
                vertices.Add(tail);
                tailIndex = vertices.Count - 1;
            }
            else
            {
                tailIndex = vertices.FindIndex(x => x == tail);
                isContainTail = true;
            }

            if (!(isContainTail && isContainHead))
            {
                edges.Add(new Vector2Int(headIndex, tailIndex));
            }

            if (isContainTail && isContainHead)
            {
                edges.Add(new Vector2Int(headIndex, tailIndex));
            }
        }

        public void AddNormal(List<Vector3> _normals)
        {
            if (normals != null)
                normals.Clear();
            else
                normals = new List<Vector3>();
            normals.AddRange(_normals);
        }

        public void SaveFile(string path)
        {
            StreamWriter sw = new StreamWriter(path);
            string edgeJSON = JsonUtility.ToJson(this);
            sw.Write(edgeJSON);
            sw.Close();

            Debug.Log("save edge raw data to " + path);
        }

        public static EdgeRawData ReadFile(string path)
        {
            StreamReader sr = new StreamReader(path);
            string jsonContent = sr.ReadToEnd();
            sr.Close();
            return JsonUtility.FromJson<EdgeRawData>(jsonContent);
        }
    }


    [Serializable]
    public class SimplifyModelData
    {
        public EdgeGraph edgeGraph;
        // public EdgeRawData rawData;

        public SimplifyModelData(EdgeRawData _rawData)
        {
            // rawData = _rawData;
            edgeGraph = new EdgeGraph(_rawData.getVertexNumber, false);
        }


        public void AddEdge(int u, int v)
        {
            edgeGraph.addEdge(u, v);
        }

    }
}
