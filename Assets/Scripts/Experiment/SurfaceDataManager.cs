using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceDataManager : MonoBehaviour
{
    public float length = 1;
    SurfaceData data;
    #region Surface Processor 
    public void Initialize(SurfaceData _data)
    {
        data = _data;
    }
    
    #endregion
    #region Unity Funcitom
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnEnable(){

    }

    void OnDrawGizmos()
    {
        //show all normal gizmos in edit mode
        for(int i=0; i<data.triangleCount; i++){
            TriangleData triangle = data.GetTriangleData(i);
            Gizmos.DrawRay(transform.TransformPoint(triangle.center), triangle.normal * length);
        }
    }
    #endregion
}
