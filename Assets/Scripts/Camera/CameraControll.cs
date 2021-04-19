using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[AddComponentMenu("Camera-Control/3dsMax Camera Style")]
public class CameraControll : MonoBehaviour
{
    public Transform target;
    public Vector3 targetOffset;
    public float distance = 5.0f;
    public float maxDistance = 20;
    public float minDistance = .6f;
    public float xSpeed = 200.0f;
    public float ySpeed = 200.0f;
    public int yMinLimit = -80;
    public int yMaxLimit = 80;
    public int zoomRate = 100;
    public float panSpeed = 0.3f;
    public float moveSpeed = 0.2f;
    public float minYLocation = 0.0f;
    public float maxYLocation = 0.0f;
    public float zoomDampening = 5.0f;
    public LayerMask obstacleMask;


    private float xDeg = 0.0f;
    private float yDeg = 0.0f;
    public float desiredDistance;
    private Quaternion currentRotation;
    private Quaternion desiredRotation;
    private Quaternion rotation;
    private Vector3 position;
    private bool canMove = true;

    void Start() { Init(); }
    void OnEnable() { Init(); }


    public void Init()
    {
        //If there is no target, create a temporary target at 'distance' from the cameras current viewpoint
        if (!target)
        {
            GameObject go = new GameObject("Cam Target");
            go.transform.position = transform.position + (transform.forward * distance);
            target = go.transform;
        }

        distance = Vector3.Distance(transform.position, target.position);
        desiredDistance = distance;

        //be sure to grab the current rotations as starting points.
        position = transform.position;
        rotation = transform.rotation;
        currentRotation = transform.rotation;
        desiredRotation = transform.rotation;

        xDeg = Vector3.Angle(Vector3.right, transform.right);
        yDeg = yMaxLimit;
        Rotate();
        yDeg = Vector3.Angle(Vector3.up, transform.up);
    }

    /*
     * Camera logic on LateUpdate to only update after all character movement logic has been handled. 
     */
    void Rotate()
    {
        xDeg += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
        yDeg -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
        Cursor.visible = false;
        desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);

        yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);

        // Clamp the vertical axis for the orbit & set camera rotation 
        currentRotation = transform.rotation;

        rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime);

        transform.rotation = rotation;
    }

    void LateUpdate()
    {
        Debug.DrawRay(transform.position, transform.forward * 500000f, Color.blue);
        {/*
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.forward, out hit, 500000f, obstacleMask))
        {
            Debug.Log("T2");
            GameObject obs = hit.transform.gameObject;
            Renderer rend = obs.GetComponent<Renderer>();
            rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            ObstacleCam oC = obs.GetComponent<ObstacleCam>();
            oC.Delay();
            Debug.Log("Test");
        }
        */
        }
        // If Alt and Middle button? ZOOM!
        if (Input.GetMouseButton(2) && Input.GetKey(KeyCode.LeftAlt))
        {
            desiredDistance -= Input.GetAxis("Mouse Y") * Time.deltaTime * zoomRate * 0.125f * Mathf.Abs(desiredDistance);
        }

        // Right Click Orbit 
        if (Input.GetMouseButton(1))
        { 
            Cursor.visible = false;

            xDeg += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            yDeg -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

            yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
            desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);


            // Clamp the vertical axis for the orbit & set camera rotation 
            currentRotation = transform.rotation;

            rotation = Quaternion.Lerp(currentRotation, desiredRotation, 5);

            transform.rotation = rotation;
        }
        else
            Cursor.visible = true;



        ////////Orbit Position

        //clamp the zoom min/max
        desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
        
        position = target.position - (rotation * Vector3.forward * desiredDistance + targetOffset);
        transform.position = new Vector3(Mathf.Lerp(transform.position.x, position.x, 2), Mathf.Lerp(transform.position.y, position.y, 2), Mathf.Lerp(transform.position.z, position.z, 2));

        /////Inputs
        if (canMove)
        {
            // Middle mouse Pan
            if (Input.GetMouseButton(2))
            {
                //grab the rotation of the camera so we can move in a psuedo local XY space
                target.rotation = transform.rotation;
                target.Translate(Vector3.right * -Input.GetAxis("Mouse X") * panSpeed);
                target.Translate(transform.forward * -Input.GetAxis("Mouse Y") * panSpeed, Space.World);
            }

            ////////Move with WASD
            if (Input.GetKey(KeyCode.W))
            {
                Vector3 dirToMove = transform.forward;
                dirToMove = new Vector3(dirToMove.x, 0, dirToMove.z);
                target.transform.position += dirToMove * moveSpeed / 5;
            }
            if (Input.GetKey(KeyCode.S))
            {
                Vector3 dirToMove = transform.forward;
                dirToMove = new Vector3(dirToMove.x, 0, dirToMove.z);
                target.transform.position += dirToMove * -moveSpeed / 5;
            }
            if (Input.GetKey(KeyCode.A))
            {
                target.transform.position += transform.right * -1f * moveSpeed / 5;
            }
            if (Input.GetKey(KeyCode.D))
            {
                target.transform.position += transform.right * moveSpeed / 5;
            }
            if (Input.GetKey(KeyCode.Space))
            {
                if (target.position.y <= minYLocation + 10)
                    target.transform.position += Vector3.up * moveSpeed / 5;
            }
            if (Input.GetKey(KeyCode.LeftAlt))
            {
                if (target.position.y >= minYLocation)
                    target.transform.position += Vector3.up * -moveSpeed / 5;
            }
            // affect the desired Zoom distance if we roll the scrollwheel
            desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate * Mathf.Abs(desiredDistance);
        }
        
       
        if (target.position.y <= maxYLocation)
            target.transform.position = new Vector3(target.transform.position.x, maxYLocation, target.transform.position.z);

        target.position = new Vector3(target.position.x, 0.7f, target.position.z);
        
    }
    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }

    private void OnDrawGizmos()
    {
        if (target)
        {
            Gizmos.DrawWireCube(target.position, new Vector3(0.5f, 0.5f, 0.5f));

            Gizmos.color = Color.green;
            Gizmos.DrawCube(target.position, new Vector3(0.5f, 0.5f, 0.5f));
        }

    }

    public float GetDistanceToPos(Vector3 pos)
    {
        float currentDesiredDistance = Vector3.Distance(transform.position, pos + Vector3.up * 0.7f);
        return currentDesiredDistance;
    }

    public void GoToPos (Vector3 pos)
    {
        StopCoroutine(SwitchPos(pos));
        StartCoroutine(SwitchPos(pos));
    }

    IEnumerator SwitchPos(Vector3 pos)
    {
        canMove = false;
        Vector3 startPos = target.position;
        int frameCount = 40;
        float currentDesiredDistance = GetDistanceToPos(pos);
        float oldDistance = desiredDistance;

        for (int i = 0; i < frameCount; i++)
        {
            target.position = Vector3.Lerp(startPos, pos + Vector3.up * 0.7f, (float)i / (float)frameCount);
            if(currentDesiredDistance < oldDistance)
            desiredDistance = Mathf.Lerp(oldDistance, currentDesiredDistance, (float)i / (float)frameCount);
            yield return null;
        }

        canMove = true;
    }
}

