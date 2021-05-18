using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using ThreeDModelProcessing;

public class EdgeReader : MonoBehaviour
{
    public string fileName = "edgeData.txt";
    public string graphFileName = "edgeGraph.txt";
    public string readFrom = "";
    // public List<Edge> edgeList = new List<Edge>();
    public EdgeRawData edgeList;
    public EdgeGraph edgeGraph;
    [Range(0.001f, 0.01f)]public float scale = 0.01f;
    public Color color;
    void ReadEdge(string path)
    {

        if (path == "")
        {
            path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/" + fileName;
        }
        StreamReader sr = new StreamReader(path);
        string jsonContent = sr.ReadToEnd();
        edgeList = JsonUtility.FromJson<EdgeRawData>(jsonContent);

        // while (sr.Peek() >= 0)
        // {
        //     edgeList.Add(JsonUtility.FromJson<Edge>(sr.ReadLine()));
        // }


    }

    void ConnectEdgeGraph(string path){
        if (path == "")
        {
            path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/" + graphFileName;
        }
        edgeGraph = EdgeGraph.ConvertEdgeRawDataToGraph(edgeList);
        edgeGraph.OutputDictGraph(path);
    }


    void OnGUI()
    {
        Rect btn = new Rect(50, 150, 150, 50);
        if (GUI.Button(btn, "Read Edge"))
        {
            ReadEdge(readFrom);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = color;
        foreach (var edge in edgeList.edges)
        {
            Vector3 head = edgeList.vertices[edge.x];
            Vector3 tail = edgeList.vertices[edge.y];

            Gizmos.DrawLine(transform.TransformPoint(head), transform.TransformPoint(tail));

            Gizmos.DrawSphere(transform.TransformPoint(head), scale);
            Gizmos.DrawSphere(transform.TransformPoint(tail), scale);
        }
    }
}
