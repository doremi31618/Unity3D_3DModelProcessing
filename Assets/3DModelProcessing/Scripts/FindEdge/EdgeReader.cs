using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using ThreeDModelProcessing;

public class EdgeReader : MonoBehaviour
{
    public string extension = ".txt";
    public string fileName = "edgeData.txt";
    public string graphName = "edgeGraph.txt";
    public string readFrom = "";
    // public List<Edge> edgeList = new List<Edge>();
    [HideInInspector]public EdgeRawData edgeList;
    [HideInInspector]public EdgeGraph edgeGraph;
    public bool isUseGUI = false;
    public bool isShowGizmos = true;
    public bool isDrawNormals = false;
    [Range(0.001f, 0.01f)] public float scale = 0.01f;
    [Range( 0.001f,0.1f)] public float normalLength = 0.01f;
    public Color edgeColor;
    void ReadEdge(string path)
    {
        string wholePath = "";
        if (GetComponent<EdgeFinder>() != null){
            fileName = GetComponent<EdgeFinder>().fileName;
            graphName = GetComponent<EdgeFinder>().graphName;
            extension = GetComponent<EdgeFinder>().extension;
            readFrom = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            path = "Exist";
        }

        if (path == "")
        {
            readFrom = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            wholePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/" + fileName + extension;
        }
        else{
            wholePath = readFrom + "/" + fileName + extension;
        }

        
        edgeList = EdgeRawData.ReadFile(wholePath);

        // while (sr.Peek() >= 0)
        // {
        //     edgeList.Add(JsonUtility.FromJson<Edge>(sr.ReadLine()));
        // }


    }

    void ConnectEdgeGraph(string path)
    {
        if (path == "")
        {
            path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/" + graphName + extension;
        }
        edgeGraph = EdgeGraph.ConvertEdgeRawDataToGraph(edgeList, true);
        edgeGraph.OutputDictGraph(path);
    }


    void OnGUI()
    {
        if (!isUseGUI)return;
        Rect btn = new Rect(50, 150, 150, 50);
        if (GUI.Button(btn, "Read Edge"))
        {
            ReadEdge(readFrom);
        }
    }

    void OnDrawGizmos()
    {
        if (!isShowGizmos)return;
        
        Gizmos.color = edgeColor;
        foreach (var edge in edgeList.edges)
        {
            Vector3 head = transform.TransformPoint(edgeList.vertices[edge.x]);
            Vector3 tail = transform.TransformPoint(edgeList.vertices[edge.y]);

            Gizmos.DrawLine(head, tail);

            Gizmos.DrawSphere(head, scale);
            Gizmos.DrawSphere(tail, scale);

            if (!isDrawNormals)continue;
            Gizmos.DrawLine(head, transform.TransformPoint(edgeList.vertices[edge.x] + edgeList.normals[edge.x] * normalLength));
            Gizmos.DrawLine(tail, transform.TransformPoint(edgeList.vertices[edge.y] + edgeList.normals[edge.y] * normalLength));
        }

       
    }
}
