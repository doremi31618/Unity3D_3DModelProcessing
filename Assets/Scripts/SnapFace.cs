using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
[ExecuteInEditMode]
[RequireComponent(typeof(EditMesh), typeof(Rigidbody))]
public class SnapFace : MonoBehaviour
{
    [Range(0.05f, 0.5f)]public float rayLength = 1f;
    [Range(0.1f, 1)]public float speed = 0.1f;
    float _oldRayLength;
    private float minDistance = 0.01f;
    public List<GameObject> triangleHandle;
    public EditMesh editMesh;
    Rigidbody rigidBody;

    void DetectFaceNormal(object o, HitEventArgs e){
        if (e.distance <= minDistance)return;
        Debug.Log("Detect Face");
        rigidBody.MovePosition(transform.position + e.direction.normalized * Time.deltaTime * speed);
    }

    void OnValueChange(){
        if(_oldRayLength != rayLength){
            foreach(var t in triangleHandle){
                t.GetComponent<FaceGizmos>().setDetectLength = rayLength;
                _oldRayLength = rayLength;
            }
        }
    }

    void OnEnable(){
        triangleHandle = new List<GameObject>();
        if (rigidBody == null)
            rigidBody = GetComponent<Rigidbody>();

        if(editMesh == null)
            editMesh = GetComponent<EditMesh>();
        
        if (!editMesh.enabled)
            editMesh.enabled = true;

        int triangle_count = editMesh.triangleCount;
        for(int i=0; i<triangle_count; i++){
            GameObject triangle = new GameObject("triangle_handle");
            GameObject[] vertices = editMesh.GetTriangleVertices(i);
            triangle.transform.parent = this.transform;

            FaceGizmos faceGizmos = triangle.AddComponent<FaceGizmos>();
            faceGizmos.OnHitSomething += new HitEventHandler(DetectFaceNormal);
            faceGizmos.Initialize(vertices, rayLength);
            triangleHandle.Add(triangle);
        }

        //assign init value
        _oldRayLength = rayLength;

    }

    void OnDisable(){
        foreach(var t in triangleHandle){
            DestroyImmediate(t);
        }
        triangleHandle = null;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        OnValueChange();
    }
}
#endif