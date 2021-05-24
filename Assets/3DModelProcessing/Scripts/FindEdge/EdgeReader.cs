using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using ThreeDModelProcessing;

namespace ThreeDModelProcessing.Edge
{
    [RequireComponent(typeof(EdgeData))]
    public class EdgeReader : MonoBehaviour
    {
        EdgeData edgeData
        {
            get
            {
                return GetComponent<EdgeData>();
            }
        }
        public string extension { get { return edgeData.extension; } }
        public string fileName { get { return edgeData.fileName; } set { edgeData.fileName = value; } }
        public string graphName { get { return edgeData.graphName; } set { edgeData.graphName = value; } }
        public string readFrom = "";
        public EdgeRawData edgeCollection
        {
            get{
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
        
        [Range(0.001f, 0.01f)] public float scale = 0.01f;
        [Range(0.001f, 0.1f)] public float normalLength = 0.01f;
        public Color edgeColor;

        public bool isUseGUI {get{return edgeData.isUseGUI;}}
        public bool isDrawGizmos {get{return edgeData.isDrawGizmos;}}
        public bool isDrawNormals = false;
        void ReadEdge(string path)
        {
            string wholePath = "";
            if (GetComponent<EdgeFinder>() != null)
            {
                readFrom = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                path = "Exist";
            }

            if (path == "")
            {
                readFrom = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                wholePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/" + fileName + extension;
            }
            else
            {
                wholePath = readFrom + "/" + fileName + extension;
                edgeData.wholeFilePah = wholePath;
            }


            edgeCollection = EdgeRawData.ReadFile(wholePath);

        }

        void ConnectEdgeGraph(string path)
        {
            if (path == "")
            {
                path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/" + graphName + extension;
            }
            edgeGraph = EdgeGraph.ConvertEdgeRawDataToGraph(edgeCollection, true);
            edgeGraph.OutputDictGraph(path);
        }


        void OnGUI()
        {
            if (!isUseGUI) return;
            Rect btn = new Rect(50, 150, 150, 50);
            if (GUI.Button(btn, "Read Edge"))
            {
                ReadEdge(readFrom);
            }
        }

        void OnDrawGizmos()
        {
            if (!isDrawGizmos) return;

            Gizmos.color = edgeColor;
            foreach (var edge in edgeCollection.edges)
            {
                Vector3 head = transform.TransformPoint(edgeCollection.vertices[edge.x]);
                Vector3 tail = transform.TransformPoint(edgeCollection.vertices[edge.y]);

                Gizmos.DrawLine(head, tail);

                Gizmos.DrawSphere(head, scale);
                Gizmos.DrawSphere(tail, scale);

                if (!isDrawNormals) continue;
                Gizmos.DrawLine(head, transform.TransformPoint(edgeCollection.vertices[edge.x] + edgeCollection.normals[edge.x] * normalLength));
                Gizmos.DrawLine(tail, transform.TransformPoint(edgeCollection.vertices[edge.y] + edgeCollection.normals[edge.y] * normalLength));
            }


        }
    }

}
