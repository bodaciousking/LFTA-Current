using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileFlipping : MonoBehaviour
{
    public Transform target;

    public void Start()
    {
    }
    private void Update()
    {
        transform.LookAt(target);
    }
}
