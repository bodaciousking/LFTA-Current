using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWithGameObject : MonoBehaviour
{
    public GameObject owner;
   

    private void Update()
    {
        transform.SetPositionAndRotation(new Vector3(owner.transform.position.x, owner.transform.position.y + 0.02f, owner.transform.position.z), transform.rotation);
    }


}
