using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThreeDModelProcessing;

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

        Vector3 GetClosestEdge()
        {

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 cloesestPoint = Vector3.zero;
            if (Physics.Raycast(ray, out hit))
            {
                // point = hit.point;
                Debug.DrawLine(transform.position, hit.point, Color.red, 1f);
                // print("hit point (world space) : " +hit.point);
                if (hit.transform.GetComponent<EdgeFinder>().edgeCollection != null)
                {
                    edgeCollection = hit.transform.GetComponent<EdgeFinder>().edgeCollection;
                    if (edgeCollection == null) return cloesestPoint;
                    Vector3 objectSpacePoint = hit.transform.InverseTransformPoint(hit.point);
                    cloesestPoint = GetClosestVertex(hit.point);
                }
                else
                {
                    print("Haven't build edge data");
                }
            }

            return cloesestPoint;

        }

        List<Vector3> GetShortestPath(Vector3 head, Vector3 tail){
            List<Vector3> path = new List<Vector3>();
            
            return path;
        }

        Vector3 GetClosestVertex(Vector3 hitPoint)
        {
            Vector3 objectSpacePoint = transform.InverseTransformPoint(hitPoint);
            Vector3 cloesestPoint = edgeCollection.getClosestVertex(objectSpacePoint);
            // var adjacentList = hit.transform.GetComponent<EdgeFinder>().getEdgeGraph.getAdjacentVertexArray(cloesestPoint);
            // foreach(var adjPoint in adjacentList){
            //     print("adj : " + adjPoint);
            // }
            return transform.TransformPoint(cloesestPoint);
        }

        

        void SelectVertex()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 closestPoint = Vector3.zero;
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
                    closestPoint = GetClosestVertex(hit.point);
                }
                else
                {
                    print("Haven't build edge data");
                }
            }

            if (closestPoint == Vector3.zero)return;

            clickCount %= 2;
            if (clickCount == 0){
                selectedEdgeVertices = new Vector3[2];
            }
            selectedEdgeVertices[clickCount++] = closestPoint;
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
                    Gizmos.DrawSphere(point, scale);
                }
            }
            
        }
    }

}
