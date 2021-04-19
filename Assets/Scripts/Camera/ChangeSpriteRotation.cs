using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSpriteRotation : MonoBehaviour
{
    Camera mainCam;
    public GameObject mainSprite, head;
    public float clamp;
    

    void Start()
    {
        mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(mainCam.transform.rotation.x);
        if(mainCam.transform.rotation.x > clamp || mainCam.transform.forward.y <= -clamp)
        {
            mainSprite.SetActive(false);
            head.SetActive(true);
        }
        else
        {
            mainSprite.SetActive(true);
            head.SetActive(false);
        }
    }
}
