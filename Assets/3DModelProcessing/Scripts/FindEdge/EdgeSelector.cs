using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThreeDModelProcessing.Algorithm;

namespace ThreeDModelProcessing.Edge
{
    public class EdgeSelector : MonoBehaviour
    {
        
        public bool isSelected = true;
        int clickCount = 0;
        public Vector3[] selectedEdgeVertices;
        public Color gizmosColor = Color.white;
        [Range(0.001f, 0.01f)] public float scale = 0.005f;
        EdgeRawData edgeCollection;
        EdgeData edgeData {get{return GetComponent<EdgeData>();}}
        EdgeGraph edgeGraph {get{return edgeData.edgeGraph;}}

        Vector3 GetClosestVertex(Vector3 hitPoint)
        {
            Vector3 objectSpacePoint = transform.InverseTransformPoint(hitPoint);
            Vector3 closestPoint = edgeCollection.GetClosestVertex(objectSpacePoint);
            // var adjacentList = hit.transform.GetComponent<EdgeFinder>().getEdgeGraph.getAdjacentVertexArray(cloesestPoint);
            // foreach(var adjPoint in adjacentList){
            //     print("adj : " + adjPoint);
            // }
            if (closestPoint == Vector3.zero)return Vector3.zero;
            Debug.Log("Closest point : "+closestPoint);
            clickCount %= 2;
            if (clickCount == 0){
                selectedEdgeVertices = new Vector3[2];
                edgeData.ClearShortestPath();
            }
            selectedEdgeVertices[clickCount++] = closestPoint;
            if (clickCount == 2){
                edgeData.UpdateShortestPath(selectedEdgeVertices[0], selectedEdgeVertices[1]);
                
            }
            return transform.TransformPoint(closestPoint);
        }

        

        void SelectVertex()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                // point = hit.point;
                Debug.DrawLine(transform.position, hit.point, Color.red, 1f);
                // print("hit point (world space) : " +hit.point);
                if (hit.transform.GetComponent<EdgeFinder>().edgeCollection != null)
                {
                    edgeCollection = hit.transform.GetComponent<EdgeFinder>().edgeCollection;
                    if (edgeCollection == null) return;
                    Vector3 objectSpacePoint = hit.transform.InverseTransformPoint(hit.point);
                    GetClosestVertex(hit.point);
                    
                }
                else
                {
                    print("Haven't build edge data");
                }
            }

            
        }


        void OnMouseDown()
        {
            if (isSelected)
            {
                SelectVertex();
            }
        }

        void OnDrawGizmos()
        {
            if (Time.time == 0 && selectedEdgeVertices.Length == 2)return;
            Gizmos.color = gizmosColor;
            foreach(var point in selectedEdgeVertices){
                if (point != Vector3.zero)
                {
                    Gizmos.DrawSphere(transform.TransformPoint(point), scale);
                }
            }
            
        }
    }

}
