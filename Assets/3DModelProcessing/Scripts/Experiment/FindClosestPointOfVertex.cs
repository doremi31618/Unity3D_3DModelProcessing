using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThreeDModelProcessing;

[RequireComponent(typeof(Collider))]
public class FindClosestPointOfVertex : MonoBehaviour
{
    public Vector3 point;
    [Range(0.001f, 0.01f)] public float scale = 0.005f;
    EdgeGraph edgeGraph;

    void GetClosestEdge(){

        if (Input.GetMouseButtonDown(0)){
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit)){
                point = hit.point;

                if (hit.transform.GetComponent<EdgeFinder>() != null){
                    edgeGraph = GetComponent<EdgeFinder>().getEdgeGraph;
                }
            }

        }
        
        if ( edgeGraph == null)return;


        
        
    }
    void OnMouseDown()
    {
        // print("MouseDown");
        Vector3[] vertices = GetComponent<MeshFilter>().mesh.vertices;
        Vector3 mouseClickPosition = mousePoint();

        
        float distance = Mathf.Infinity;
        foreach (var vertex in vertices)
        {
            float _distance = Vector3.Distance(mouseClickPosition, transform.TransformPoint(vertex));
            if (_distance < distance)
            {
                point = transform.TransformPoint(vertex);
                distance = _distance;
            }
        }
    }

    Vector3 mousePoint()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit)){
            point = hit.point;
        }

        return point;
    }

    void OnDrawGizmos()
    {
        if (point != Vector3.zero)
        {
            Gizmos.DrawSphere(point, scale);
        }
    }
}
