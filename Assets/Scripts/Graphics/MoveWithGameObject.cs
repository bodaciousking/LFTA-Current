using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWithGameObject : MonoBehaviour
{
    public GameObject owner;
    public GameObject shadow;
    RaycastHit hit;
    int layerMask = 1 << 11;
    private Vector3 targetNormal;
    Quaternion toRotation = new Quaternion(0,0,1,0);

    private void Start()
    {
        shadow = transform.GetChild(1).gameObject;
    }
    private void Update()
    {
        if (Physics.Raycast(transform.position + new Vector3(0, 0.1f, 0), Vector3.down, out hit, 0.5f, layerMask))
        {
            if (hit.normal == transform.up) 
            {
                return; 
            }
            if (hit.normal != targetNormal)
            {
                targetNormal = hit.normal;
                toRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            }
        }
        shadow.transform.SetPositionAndRotation(new Vector3(owner.transform.position.x, owner.transform.position.y + 0.02f, owner.transform.position.z), toRotation);
        transform.SetPositionAndRotation(owner.transform.position, transform.rotation);
    }
}
