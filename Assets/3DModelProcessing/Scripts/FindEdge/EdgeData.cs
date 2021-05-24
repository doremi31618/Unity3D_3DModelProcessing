using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThreeDModelProcessing;
namespace ThreeDModelProcessing.Edge
{
    public class EdgeData : MonoBehaviour
    {
        public string wholeFilePah;
        public string wholeGraphPath;
        public string folderPath = "";
        public string extension = ".txt";
        public string fileName = "edgeData";
        public string graphName = "edgeGraph";
        public  bool isDrawPreview;
        public bool isUseGUI;
        public bool isDrawGizmos;
        public EdgeRawData edgeCollection = new EdgeRawData();
        public EdgeGraph edgeGraph;

        void OnRenderObject()
        {
            if (Time.time == 0 || !isDrawPreview) return;
            DrawEdge(edgeCollection);
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
    }
}

