using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using ThreeDModelProcessing;

public class EdgeReader : MonoBehaviour
{
    public static string fileName = "edgeData.txt";
    public string readFrom = "";
    // public List<Edge> edgeList = new List<Edge>();
    public EdgeRawData edgeList;
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
        // if (edgeList.Count < 1) return;
        // foreach (var e in edgeList)
        // {
        //     Vector3 direction = e.edgeVertex[1] - e.edgeVertex[0];
        //     Gizmos.DrawRay(e.edgeVertex[0], direction);

        //     Gizmos.DrawSphere(transform.TransformPoint(e.edgeVertex[0]), 0.01f);
        //     Gizmos.DrawSphere(transform.TransformPoint(e.edgeVertex[1]), 0.01f);
        // }
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
