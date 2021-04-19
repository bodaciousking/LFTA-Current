using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookTowardsCam : MonoBehaviour
{
    Camera mainCamera;
    float ySpot;
    Quaternion prevPos;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        ySpot = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        //if(mainCamera.transform.rotation.x > prevPos.x)
        //{
        //    //Camera moved up
        //    transform.position -= new Vector3(0, 0, transform.forward.z) * (mainCamera.transform.rotation.x - prevPos.x);
        //}
        //if (mainCamera.transform.rotation.x < prevPos.x)
        //{
        //    //Camera moved down
        //    transform.position += new Vector3(0, 0, transform.forward.z) * (prevPos.x- mainCamera.transform.rotation.x); 
        //}
       
        //prevPos = mainCamera.transform.rotation;
        //CameraControll camC = mainCamera.GetComponent<CameraControll>();
        //float percentToMove = camC.yMaxLimit / camC.transform.rotation.x;

        //transform.position = new Vector3(transform.position.x, ySpot, transform.position.z);


        transform.forward = mainCamera.transform.forward;
        Vector3 myDirection = this.transform.forward;
        //Debug.Log(gameObject.name + "! My direction is X:" + myDirection.x + " Y:" + myDirection.y + " Z:" + myDirection.z);
    }
}
