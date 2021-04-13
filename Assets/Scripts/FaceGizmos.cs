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
    GameObject[] vertices;
    Vector3 centerPos;
    Vector3 normal;
    float detectLength = 0.1f;
    public float setDetectLength{set{detectLength = value;}}
    public event HitEventHandler OnHitSomething;
    public void Initialize(GameObject[] _vertices_object, float _detectLength, bool isShowTriangle){
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

        isInitiate = true;
    }
    void UpdateTriangleGizmos(){
        if (!isInitiate)return;

        Vector3[] _vertices = {
            vertices[0].transform.position,
            vertices[1].transform.position,
            vertices[2].transform.position};
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
        
        RaycastHit hit;
        Ray ray = new Ray(centerPos, normal * detectLength);
        if (Physics.Raycast(ray, out hit)){
            Gizmos.color = Color.red;
            if (OnHitSomething != null)
                OnHitSomething(this, new  HitEventArgs(hit.point, normal,Vector3.Distance(hit.point, centerPos)));
        }else{
             Gizmos.color = Color.white;
        }
        Gizmos.DrawRay(centerPos, normal * detectLength);
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