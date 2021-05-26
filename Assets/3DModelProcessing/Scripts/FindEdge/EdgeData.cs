using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThreeDModelProcessing.Algorithm;
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
        public Color gizmosColor = Color.white;
        [Range(0.001f, 0.01f)] public float scale = 0.005f;
        public bool isDrawPreview;
        public bool isUseGUI;
        public bool isDrawGizmos;
        public List<Vector3> selectedEdge;
        public EdgeRawData edgeCollection = new EdgeRawData();
        public EdgeGraph edgeGraph;

        void OnRenderObject()
        {
            if (Time.time == 0 || !isDrawPreview) return;
           
            if (selectedEdge.Count == 0) {
                 DrawEdge(edgeCollection);
            }else{
                DrawShortestPath();
            }
            
        }
        public void UpdateShortestPath(Vector3 head, Vector3 tail)
        {
            AStartSearch astarSearch = new AStartSearch(head, tail, edgeCollection.edgeGraph);
            selectedEdge =  astarSearch.getShortestPath;
            if (selectedEdge.Count == 0){
                print("there isn't any edge");
            }else{
                print("update shortest path");
            }
            
        }
        public void ClearShortestPath(){
            selectedEdge.Clear();
            print("Clear Shortest path");
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

        void DrawShortestPath()
        {
            GL.PushMatrix();
            
            
            for(int i=0; i<selectedEdge.Count; i++)
            {
                GL.Begin(GL.LINES);
                GL.Color(Color.white);
                Vector3 head = transform.TransformPoint(selectedEdge[i]);
                GL.Vertex3(head.x, head.y, head.z);

                if( i+1 <= selectedEdge.Count-1){
                    Vector3 tail = transform.TransformPoint(selectedEdge[i+1]);
                    GL.Vertex3(tail.x, tail.y, tail.z);
                    GL.Color(Color.white);
                }
                GL.End();
            }
            
            GL.PopMatrix();
        }
    }
}

