using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThreeDModelProcessing;

public class EdgeSelector : MonoBehaviour
{
    public bool isUseSelect = true;
    public Vector3 point;
    public Color gizmosColor = Color.white;
    [Range(0.001f, 0.01f)] public float scale = 0.005f;
    EdgeRawData edgeCollection;

    void GetClosestEdge()
    {

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            // point = hit.point;
            Debug.DrawLine(transform.position, hit.point, Color.red, 1f);
            print("hit point (world space) : " +hit.point);
            if (hit.transform.GetComponent<EdgeFinder>().edgeCollection != null)
            {
                edgeCollection = hit.transform.GetComponent<EdgeFinder>().edgeCollection;
                if (edgeCollection == null) return;
                Vector3 objectSpacePoint =  hit.transform.InverseTransformPoint(hit.point);//
                print("hit point (local space) : " +objectSpacePoint);
                // Transform hitTransform = hit.transform.parent;
                // while(hitTransform != null){
                //     objectSpacePoint = hitTransform.worldToLocalMatrix * objectSpacePoint;
                //     if (hitTransform.parent == null)break;
                //     hitTransform = hitTransform.parent;
                // }

                point = hit.transform.TransformPoint( edgeCollection.getClosestVertex(objectSpacePoint));//edgeCollection.getClosestVertex(
                    print("hit point (transform world space) : " +point);
                // hitTransform = hit.transform.parent;
                // while(hitTransform != null){
                //     point = hitTransform.localToWorldMatrix *point;
                //     if (hitTransform.parent == null)break;
                //     hitTransform = hitTransform.parent;
                // }
                // point = hit.transform.TransformPoint(edgeGraph.GetClosestVertex(objectSpacePoint));
            }
        }






    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && isUseSelect)
        {
            GetClosestEdge();
        }
    }

    // void OnMouseDown()
    // {
    //     // print("MouseDown");
    //     Vector3[] vertices = GetComponent<MeshFilter>().mesh.vertices;
    //     Vector3 mouseClickPosition = mousePoint();
    //     // GetClosestEdge();

    //     float distance = Mathf.Infinity;
    //     foreach (var vertex in vertices)
    //     {
    //         float _distance = Vector3.Distance(mouseClickPosition, transform.TransformPoint(vertex));
    //         if (_distance < distance)
    //         {
    //             point = transform.TransformPoint(vertex);
    //             distance = _distance;
    //         }
    //     }
    // }

    Vector3 mousePoint()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            point = hit.point;
        }

        return point;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = gizmosColor;
        if (point != Vector3.zero)
        {
            Gizmos.DrawSphere(point, scale);
        }
    }
}
