using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
// using System.Text;
namespace ThreeDModelProcessing
{
    [Serializable]
    public class EdgeGraph
    {
        public int nodeNumber = 0;
        public List<int>[] graph;
        public List<int>[] getGraph { get { return graph; } }
        public EdgeGraph(int number)
        {
            InitEdgeGraph(number);
            nodeNumber = number;
        }

        public void InitEdgeGraph(int number)
        {
            
            nodeNumber = number;
            graph = new List<int>[number];

            for (int i = 0; i < number; i++)
            {
                graph[i] = new List<int>();
            }
        }

        public void addEdge(int u, int v)
        {
            if (!graph[u].Contains(v))
                graph[u].Add(v);
                
            if (!graph[v].Contains(u))
                graph[v].Add(u);
        }
        public void OutputGraph(string path){
            StreamWriter sw = new StreamWriter(path);
            for(int i=0; i<nodeNumber; i++){
                string content =  i + " | ";
                foreach (var node in graph[i]){
                    content += node.ToString() + " ";
                }
                sw.WriteLine(content);
            }
            sw.Close();
        }
    }

    [Serializable]
    public class EdgeRawData
    {
        public List<Vector3> vertices;
        public List<Vector2Int> edges;
        public int getVertexNumber{get{return vertices.Count;}}
        public int getEdgeNumber{get{return edges.Count;}}

        public EdgeRawData()
        {
            vertices = new List<Vector3>();
            edges = new List<Vector2Int>();
        }
        public Vector2Int getEdge(int index){
            return edges[index];
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
            edgeGraph = new EdgeGraph(_rawData.getVertexNumber);
        }


        public void AddEdge (int u, int v){
            edgeGraph.addEdge(u, v);
        }

    }
}
