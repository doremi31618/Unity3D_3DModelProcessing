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
        public Color gizmosColor { get { return edgeData.gizmosColor; } }
        public float scale { get { return edgeData.scale; } }
        public bool isDrawPreview { get { return edgeData.isDrawPreview; } }
        EdgeRawData edgeCollection;
        EdgeData edgeData { get { return GetComponent<EdgeData>(); } }
        EdgeGraph edgeGraph { get { return edgeData.edgeGraph; } }

        public GameObject[] SpherePreview;

        Vector3 GetClosestVertex(Vector3 hitPoint)
        {
            if (edgeCollection.getVertexNumber == 0)return Vector3.zero;
            Vector3 objectSpacePoint = transform.InverseTransformPoint(hitPoint);
            Vector3 closestPoint = edgeCollection.GetClosestVertex(objectSpacePoint);
            if (closestPoint == Vector3.zero) return Vector3.zero;
            // Debug.Log("Closest point : " + closestPoint);
            clickCount %= 3;
            if (clickCount == 0)
            {
                selectedEdgeVertices = new Vector3[2];
                edgeData.ClearShortestPath();
            }
            // print("Click count " + clickCount);
            if (clickCount < 2)
            {
                selectedEdgeVertices[clickCount] = closestPoint;
                
                if (clickCount == 1)
                {
                    edgeData.UpdateShortestPath(selectedEdgeVertices[0], selectedEdgeVertices[1]);

                }
            }
            UpdatePreviewSphere();
            clickCount++;
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
            if (Time.time <= 0 && selectedEdgeVertices.Length == 2 || selectedEdgeVertices == null) return;
            Gizmos.color = gizmosColor;
            foreach (var point in selectedEdgeVertices)
            {
                if (point != Vector3.zero)
                {
                    Gizmos.DrawSphere(transform.TransformPoint(point), scale);
                }
            }

        }

        void UpdatePreviewSphere()
        {
            if (SpherePreview.Length == 0)
            {
                SpherePreview = new GameObject[2];
                for (int i = 0; i < 2; i++)
                {
                    SpherePreview[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    SpherePreview[i].transform.localScale = Vector3.one * scale;
                    SpherePreview[i].GetComponent<MeshRenderer>().material.SetColor("_Color", gizmosColor);
                    SpherePreview[i].GetComponent<Collider>().enabled = false;
                    SpherePreview[i].SetActive(false);
                }
            }

            if (clickCount < 2)
            {
                SpherePreview[clickCount].SetActive(true);
                SpherePreview[clickCount].transform.position = transform.TransformPoint(selectedEdgeVertices[clickCount]);
            }

             if ((edgeData.selectedEdge.Count == 0 && clickCount == 1) || clickCount == 2){
                 
                 edgeData.selectedEdge.Clear();
                 for (int i = 0; i < 2; i++)
                    SpherePreview[i].SetActive(false);
             }
                

        }
    }

}
