using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public delegate void HitEventHandler(object sender, HitEventArgs e);
public class HitEventArgs : EventArgs{
    public Vector3 hitPosition;
    public float distance;
    public Vector3 direction;
    public HitEventArgs(Vector3 _hitPosition, Vector3 _direction, float _distance){
        hitPosition = _hitPosition;
        distance = _distance;
        direction = _direction;
    }
}

#if UNITY_EDITOR
[ExecuteInEditMode]
public class FaceGizmos : MonoBehaviour
{
    bool isInitiate = false;
    public GameObject[] vertices;
    public Vector3[] vertex_positions;
    Vector3 centerPos;
    Vector3 normal;
    float detectLength = 0.1f;
    public float setDetectLength{set{detectLength = value;}}
    public event HitEventHandler OnHitSomething;
    public bool isUseGameObject = true;

    Vector3[] getVerticies{
        get{
            Vector3[] _vertices;
            if (isUseGameObject){
                _vertices = new Vector3[] {vertices[0].transform.position, vertices[1].transform.position, vertices[2].transform.position};
            }
            else{
                _vertices = vertex_positions;
            }
            return _vertices;
        }
    }

    public void Initialize(GameObject[] _vertices_object, float _detectLength){
        vertices = _vertices_object;
        Vector3[] _vertices = {
            vertices[0].transform.position,
            vertices[1].transform.position,
            vertices[2].transform.position};
        Vector3 p1_p2 = _vertices[1] - _vertices[0];
        Vector3 p1_p3 = _vertices[2] - _vertices[0];
        normal = Vector3.Cross(p1_p2, p1_p3).normalized;
        detectLength = _detectLength;
        centerPos = Vector3.zero;

        foreach(var v in _vertices){
            centerPos += v/3;
        }
        isUseGameObject = true;
        isInitiate = true;
    }
    public void Initialize(Vector3[] _vertex_vector){
        vertex_positions = _vertex_vector;
        Vector3 p1_p2 = _vertex_vector[1] - _vertex_vector[0];
        Vector3 p1_p3 = _vertex_vector[2] - _vertex_vector[0];
        normal = Vector3.Cross(p1_p2, p1_p3).normalized;
        detectLength = 0.1f;
        centerPos = Vector3.zero;

        foreach(var v in _vertex_vector){
            centerPos += v/3;
        }

        isUseGameObject = false;
        isInitiate = true;
    }
    public void Initialize(Vector3[] _vertex_vector, float _detectLength ){
        vertex_positions = _vertex_vector;
        Vector3 p1_p2 = _vertex_vector[1] - _vertex_vector[0];
        Vector3 p1_p3 = _vertex_vector[2] - _vertex_vector[0];
        normal = Vector3.Cross(p1_p2, p1_p3).normalized;
        detectLength = _detectLength;
        centerPos = Vector3.zero;

        foreach(var v in _vertex_vector){
            centerPos += v/3;
        }

        isUseGameObject = false;
        isInitiate = true;
    }

    void UpdateTriangleGizmos(){
        if (!isInitiate)return;

        Vector3[] _vertices = getVerticies;

        
        
        Vector3 p1_p2 = _vertices[1] - _vertices[0];
        Vector3 p1_p3 = _vertices[2] - _vertices[0];
        normal = Vector3.Cross(p1_p2, p1_p3).normalized;
        centerPos = Vector3.zero;

        foreach(var v in _vertices){
            centerPos += v/3;
        }
    }

    /// <summary>
    /// Callback to draw gizmos that are pickable and always drawn.
    /// </summary>
    void OnDrawGizmos()
    {
        Vector3[] _vertices = getVerticies;
        RaycastHit hit;
        Ray ray = new Ray(centerPos, normal * detectLength);
        if (Physics.Raycast(ray, out hit)){
            Gizmos.color = Color.red;
            if (OnHitSomething != null)
                OnHitSomething(this, new  HitEventArgs(hit.point, normal,Vector3.Distance(hit.point, centerPos)));
        }else{
             Gizmos.color = Color.white;
        }
        //draw normal 
        Gizmos.DrawRay(transform.TransformPoint(centerPos) , normal * detectLength);
        
        //draw triangle wire frame
        Gizmos.DrawRay(transform.TransformPoint(_vertices[0]) ,  _vertices[1] - _vertices[0]);
        Gizmos.DrawRay(transform.TransformPoint(_vertices[1]) ,  _vertices[2] - _vertices[1]);
        Gizmos.DrawRay(transform.TransformPoint(_vertices[2]) ,  _vertices[0] - _vertices[2]);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTriangleGizmos();
    }

}
#endif