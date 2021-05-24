using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MoveCamera : MonoBehaviour
{
    public Transform TrackingPointTransform;
    private Vector3 _cameraOffset;

    public float SmoothFactor = 0.5f;

    public float RotationSpeed = 5.0f;
    public float VerticalSpeed = 1.0f;
    public float ZoomSpeed = 3.0f;
    public float MoveSpeed = 3.0f;

    private bool RightMouseButtonDown = false;
    private bool MiddleMouseButtonDown = false;
    private bool MouseWheelScroll = false;


    // Start is called before the first frame update
    void Start()
    {
        transform.LookAt(TrackingPointTransform);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RightMouseButtonDown = true;
            MiddleMouseButtonDown = false;
        }
        if (Input.GetMouseButtonUp(1))
        {
            RightMouseButtonDown = false;
        }
        if (Input.GetMouseButtonDown(2))
        {
            RightMouseButtonDown = false;
            MiddleMouseButtonDown = true;
        }
        if (Input.GetMouseButtonUp(2))
        {
            MiddleMouseButtonDown = false;
        }


        if (RightMouseButtonDown)
        {
            //Debug.Log(transform.localEulerAngles.x.ToString());
            _cameraOffset = transform.position - TrackingPointTransform.position;
            Quaternion camTurnAngle = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * RotationSpeed, Vector3.up);
            //Debug.Log(Input.GetAxis("Mouse Y"));
            //float angle = 0.0f;
            //Vector3 axis = Vector3.up;
            //transform.rotation.ToAngleAxis(out angle, out axis);
            //Debug.Log(angle.ToString());
            //Debug.Log(axis.ToString());
            //Debug.Log(transform.localEulerAngles.x);

            if (transform.localEulerAngles.x > 270 || transform.localEulerAngles.x < 86 || Input.GetAxis("Mouse Y") > 0)
            //if ((transform.localRotation.x > 89 & Input.GetAxis("Mouse Y") < 0) || transform.localRotation.x < 89)
            {

                Quaternion camVTurnAngle = Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * VerticalSpeed * -1, transform.right);
                //Debug.Log(camVTurnAngle.eulerAngles.x);
                //Debug.Log(camVTurnAngle.eulerAngles.y);
                //Debug.Log(camVTurnAngle.eulerAngles.z);

                _cameraOffset = camTurnAngle * camVTurnAngle * _cameraOffset;
            }
            else
            {
                _cameraOffset = camTurnAngle * _cameraOffset;
            }

            //_cameraOffset.y += Input.GetAxis("Mouse Y") * VerticalSpeed;       

            Vector3 newPos = TrackingPointTransform.position + _cameraOffset;
            transform.position = Vector3.Slerp(transform.position, newPos, SmoothFactor);
            transform.LookAt(TrackingPointTransform);
        }

        if (MiddleMouseButtonDown)
        {
            /*
            Debug.Log("MouseX"+Input.GetAxis("Mouse X").ToString());
            Debug.Log("Speed" + MoveSpeed.ToString());
            Debug.Log("Time" + Time.deltaTime.ToString());
            Debug.Log("ALL" + (Input.GetAxis("Mouse X") * MoveSpeed * Time.deltaTime).ToString());
            */

            Vector3 tempposition = transform.position + Vector3.MoveTowards(Vector3.zero, transform.right, Input.GetAxis("Mouse X") * MoveSpeed * -1);// Time.deltaTime);
            tempposition = tempposition + Vector3.MoveTowards(Vector3.zero, transform.up, Input.GetAxis("Mouse Y") * MoveSpeed * -1);// Time.deltaTime);
            //transform.position += Vector3.MoveTowards(Vector3.zero, transform.right*100, Input.GetAxis("Mouse X") * MoveSpeed);
            //new Vector3(transform.position.x + Input.GetAxis("Mouse X") * MoveSpeed, transform.position.y, transform.position.z);

            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer

            if (Physics.Raycast(tempposition, transform.forward, out hit, Mathf.Infinity))//, layerMask))transform.TransformDirection(Vector3.forward)
            {
                Debug.DrawRay(tempposition, transform.forward * hit.distance, Color.yellow);
                //Debug.Log("Did Hit");
                //Debug.Log(hit.point.ToString());
                transform.position = tempposition;
                TrackingPointTransform.position = hit.point;
            }
            else
            {
                //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
                //Debug.Log("Did not Hit");
            }

        }
        if (Application.isFocused)
        {
            transform.position += Vector3.MoveTowards(Vector3.zero, transform.forward, Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed);
        }


    }
}
