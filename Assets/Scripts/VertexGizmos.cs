using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
public class VertexGizmos : MonoBehaviour
{
    float size;
    Vector3 oldPosition;
    public Action OnEditPosition;
    public void Initialize(float _size){
        size = _size;
        oldPosition = transform.localPosition;
    }

    /// <summary>
    /// Callback to draw gizmos that are pickable and always drawn.
    /// </summary>
    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, size);
    }

    void Update(){

        //there's a problem when you scaling the gameObject 
        //all the gizmos will trigger this at same time, and this would cost 
        // a lot of efficacy
        if(oldPosition != transform.localPosition){
            OnEditPosition();
            oldPosition = transform.localPosition;
        }
    }

}

#endif